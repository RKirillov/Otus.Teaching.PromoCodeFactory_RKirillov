using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Otus.Teaching.PromoCodeFactory.Core.Domain.PromoCodeManagement;
using Otus.Teaching.PromoCodeFactory.DataAccess.Data;
using Otus.Teaching.PromoCodeFactory.DataAccess.Repositories;
using Otus.Teaching.PromoCodeFactory.WebHost.Controllers;
using Otus.Teaching.PromoCodeFactory.WebHost.Models;
using Xunit;

namespace Otus.Teaching.PromoCodeFactory.IntegrationTests.Api.WebHost.Controllers
{
    public class CustomersControllerTests
    {
        private readonly CustomersController _customersController;
        private readonly EfRepository<Customer> _customerRepository;
        private readonly EfRepository<Preference> _preferenceRepository;
        
        public CustomersControllerTests()
        {
            var dbContext = new TestDataContext();
            
            new EfDbInitializer(dbContext).InitializeDb();
            
            _customerRepository = new EfRepository<Customer>(dbContext);
            _preferenceRepository = new EfRepository<Preference>(dbContext);
            
            _customersController = new CustomersController(
                _customerRepository, 
                _preferenceRepository);
        }
        
        [Fact]
        public async Task CreateCustomerAsync_CanCreateCustomer_ShouldCreateExpectedCustomer()
        {
            //Arrange 
            var preferenceId = Guid.Parse("ef7f299f-92d7-459f-896e-078ed53ef99c");
            var request = new CreateOrEditCustomerRequest()
            {
                Email = "some@mail.ru",
                FirstName = "Иван",
                LastName = "Петров",
                PreferenceIds = new List<Guid>()
                {
                    preferenceId
                }
            };

            //Act
            var result = await _customersController.CreateCustomerAsync(request);
            var actionResult = result.Result as CreatedAtActionResult;
            var id = (Guid)actionResult.Value;
            
            //Assert
            var actual = await _customerRepository.GetByIdAsync(id);
            
            actual.Email.Should().Be(request.Email);
            actual.FirstName.Should().Be(request.FirstName);
            actual.LastName.Should().Be(request.LastName);
            actual.Preferences.Should()
                .ContainSingle()
                .And
                .Contain(x => x.PreferenceId == preferenceId);
        }
    }
}