using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Otus.Teaching.PromoCodeFactory.Core.Abstractions.Repositories;
using Otus.Teaching.PromoCodeFactory.Core.Domain.PromoCodeManagement;
using Otus.Teaching.PromoCodeFactory.WebHost.Models;

namespace Otus.Teaching.PromoCodeFactory.WebHost.Controllers
{
    /// <summary>
    /// Клиенты
    /// </summary>
    [ApiController]
    [Route("api/v1/[controller]")]
    public class CustomersController
        : ControllerBase
    {
        private readonly IRepository<Customer> _customerRepository;
        private readonly IRepository<Preference> _preferenceRepository;

        public CustomersController(IRepository<Customer> customerRepository, 
            IRepository<Preference> preferenceRepository)
        {
            _customerRepository = customerRepository;
            _preferenceRepository = preferenceRepository;
        }
        
        [HttpGet]
        public async Task<ActionResult<CustomerShortResponse>> GetCustomersAsync()
        {
            var customers =  await _customerRepository.GetAllAsync();

            var response = customers.Select(x => new CustomerShortResponse()
            {
                Id = x.Id,
                Email = x.Email,
                FirstName = x.FirstName,
                LastName = x.LastName
            }).ToList();

            return Ok(response);
        }
        
        [HttpGet("{id:guid}")]
        public async Task<ActionResult<CustomerResponse>> GetCustomerAsync(Guid id)
        {
            var customer =  await _customerRepository.GetByIdAsync(id);

            var response = new CustomerResponse(customer);

            return Ok(response);
        }
        
        [HttpPost]
        public async Task<ActionResult<CustomerResponse>> CreateCustomerAsync(CreateOrEditCustomerRequest request)
        {
            //Получаем предпочтения из бд и сохраняем большой объект
            var preferences = new List<Preference>();
            foreach (var preferenceId in request.PreferencesId)
            {
                var preference = await _preferenceRepository.GetByIdAsync(preferenceId);
                if (preference == null)
                    return BadRequest();
                preferences.Add(preference);
            }

            var customer = new Customer()
            {
                Email = request.Email,
                FirstName = request.FirstName,
                LastName = request.LastName,
            };

            var customerPreferences = preferences.Select(x => new CustomerPreference()
            {
                Customer = customer,
                Preference = x
            }).ToList();

            customer.Preferences = customerPreferences;

            await _customerRepository.AddAsync(customer);

            return CreatedAtAction(nameof(GetCustomerAsync), new {id = customer.Id}, null);
        }
        
        [HttpPut("{id:guid}")]
        public async Task<IActionResult> EditCustomersAsync(Guid id, CreateOrEditCustomerRequest request)
        {
            //Получаем предпочтения из бд и сохраняем большой объект
            var preferences = new List<Preference>();
            foreach (var preferenceId in request.PreferencesId)
            {
                var preference = await _preferenceRepository.GetByIdAsync(preferenceId);
                if (preference == null)
                    return BadRequest();
                preferences.Add(preference);
            }

            var customer = new Customer()
            {
                Id = id,
                Email = request.Email,
                FirstName = request.FirstName,
                LastName = request.LastName,
            };

            var customerPreferences = preferences.Select(x => new CustomerPreference()
            {
                Customer = customer,
                Preference = x
            }).ToList();

            customer.Preferences = customerPreferences;
            
            await _customerRepository.UpdateAsync(customer);

            return NoContent();
        }
        
        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> DeleteCustomerAsync(Guid id)
        {
            var customer = await _customerRepository.GetByIdAsync(id);
            
            if (customer == null)
                return NotFound();

            await _customerRepository.DeleteAsync(customer);

            return NoContent();
        }
    }
}