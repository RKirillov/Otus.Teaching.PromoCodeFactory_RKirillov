﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Otus.Teaching.PromoCodeFactory.Core.Abstractions.Repositories;
using Otus.Teaching.PromoCodeFactory.Core.Domain.Administration;
using Otus.Teaching.PromoCodeFactory.WebHost.Models;

namespace Otus.Teaching.PromoCodeFactory.WebHost.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class EmployeesController
        : ControllerBase
    {
        private readonly IRepository<Employee> _employeeRepository;

        public EmployeesController(IRepository<Employee> employeeRepository)
        {
            _employeeRepository = employeeRepository;
        }
        
        [HttpGet]
        public async Task<List<EmployeeItemResponseModel>> GetEmployeesAsync()
        {
            var employees = await _employeeRepository.GetAllAsync();

            var employeesModelList = employees.Select(x => 
                new EmployeeItemResponseModel()
                    {
                        Id = x.Id,
                        Email = x.Email,
                        FullName = x.FullName,
                    }).ToList();

            return employeesModelList;
        }
        
        [HttpGet("{id:guid}")]
        public async Task<EmployeeResponse> GetEmployeeByIdAsync(Guid id)
        {
            var employee = await _employeeRepository.GetByIdAsync(id);

            var employeeModel = new EmployeeResponse()
            {
                Id = employee.Id,
                Email = employee.Email,
                Roles = employee.Roles.Select(x => new RoleItemResponse()
                {
                    Name = x.Name,
                    Description = x.Description
                }).ToList(),
                FullName = employee.FullName,
                AppliedPromocodesCount = employee.AppliedPromocodesCount
            };

            return employeeModel;
        }
    }
}