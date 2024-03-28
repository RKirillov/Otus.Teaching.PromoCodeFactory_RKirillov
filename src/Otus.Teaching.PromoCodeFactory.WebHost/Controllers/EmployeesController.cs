using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Otus.Teaching.PromoCodeFactory.Core.Abstractions.Repositories;
using Otus.Teaching.PromoCodeFactory.Core.Domain.Administration;
using Otus.Teaching.PromoCodeFactory.WebHost.ModelDTO;
using Otus.Teaching.PromoCodeFactory.WebHost.Models;

namespace Otus.Teaching.PromoCodeFactory.WebHost.Controllers
{
    /// <summary>
    /// Сотрудники
    /// </summary>
    [ApiController]
    [Route("api/v1/[controller]")]
    public class EmployeesController
        : ControllerBase
    {
        private readonly IRepository<Employee> _employeeRepository;
        private readonly IRepository<Role> _rolesRepository;
        private readonly IMapper _mapper;
        public EmployeesController(IRepository<Employee> employeeRepository, IRepository<Role> rolesRepository, IMapper mapper)
        {
            _employeeRepository = employeeRepository;
            _rolesRepository = rolesRepository;
            _mapper = mapper;
        }

        /// <summary>
        /// Получить данные всех сотрудников
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<List<EmployeeShortResponse>> GetEmployeesAsync()
        {
            var employees = await _employeeRepository.GetAllAsync();

            var employeesModelList = employees.Select(x =>
                new EmployeeShortResponse()
                {
                    Id = x.Id,
                    Email = x.Email,
                    FullName = x.FullName,
                }).ToList();

            return employeesModelList;
        }

        /// <summary>
        /// Получить данные сотрудника по Id
        /// </summary>
        /// <returns></returns>
        [HttpGet]//("{id:guid}")
        [Route("{id:guid}")]
        public async Task<ActionResult<EmployeeDto>> GetEmployeeByIdAsync(Guid id)
        {
            var employee = await _employeeRepository.GetByIdAsync(id);

            if (employee == null)
                return NotFound();

            var newEmploeeDto = _mapper.Map<EmployeeDto>(employee);
            return newEmploeeDto;
        }

        /// <summary>
        /// Добавить сотрудника
        /// </summary>
        /// <param name="entity">Сущность сотрудника</param>
        /// <param name="roles">Роли сотрудника</param>
        /// <returns></returns>
        [HttpPut]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<ActionResult<EmployeeDto>> AddEmployeeAsync([FromBody] EmployeeSaveDto entity, [FromQuery] List<string> roles)
        {
            var newEmploeeDto = _mapper.Map<EmployeeDto>(entity);
            newEmploeeDto.Id = Guid.NewGuid();
            var newEmploee = _mapper.Map<Employee>(newEmploeeDto);
            //var z = (await _rolesRepository.GetAllAsync()).Where(first => roles.Any(second => second == first.Name)).ToList();
            //newEmploee.Roles = (await _rolesRepository.GetAllAsync()).Join(roles, first => first.Name, second => second,(first,second) => first).ToList();
            newEmploee.Roles = (await _rolesRepository.GetAllAsync()).Where(first => roles.Contains(first.Name)).ToList();
            var created = await _employeeRepository.AddAsync(newEmploee);
            if (created == default)
            {
                return Conflict();
            }
            return Ok(created);
            //return CreatedAtAction(nameof(GetEmployeeByIdAsync),new { id = newEmploee.Id } , newEmploee);
        }

        /// <summary>
        /// Удалить сотрудника
        /// </summary>
        /// <param name="id">GUID сотрудника</param>
        /// <returns></returns>
        [HttpDelete]
        [Route("{id:guid}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> RemoveEmployeeByIdAsync(Guid id)
        {
            if (!await _employeeRepository.RemoveAsync(id))
            {
                return NoContent();
            }
            return NotFound();
        }

        /// <summary>
        /// Обновить сущность сотрудника
        /// </summary>
        /// <param name="id">GUID сотрудника</param>
        /// <param name="entity">Cущность сотрудника</param>
        /// <returns></returns>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Route("{id}")]
        public async Task<IActionResult> UpdateEmployeeByIdAsync(Guid id, [FromBody] EmployeeSaveDto entity)//Task<ActionResult<EmployeeDto>>
        {
            var employee = await _employeeRepository.GetByIdAsync(id);

            if (employee == null)
            {
                return NotFound();
            }
            var newEmploeeDto = _mapper.Map<EmployeeDto>(entity);
            newEmploeeDto.Id = id;
            await _employeeRepository.UpdateAsync(_mapper.Map<Employee>(newEmploeeDto));
            return Ok(newEmploeeDto);
        }
    }
}
            
