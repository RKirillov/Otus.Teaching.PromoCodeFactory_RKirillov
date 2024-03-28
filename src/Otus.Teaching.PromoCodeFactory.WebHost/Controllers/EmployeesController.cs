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
        public async Task<List<EmployeeShortDto>> GetEmployeesAsync()
        {
            var employees = await _employeeRepository.GetAllAsync();

            var employeesModelList = _mapper.Map<List<EmployeeShortDto>>(employees);

/*            var employeesModelList = employees.Select(x =>
                new EmployeeShortResponse()
                {
                    Id = x.Id,
                    Email = x.Email,
                    FullName = x.FullName,
                }).ToList();
*/
            return employeesModelList;
        }

        /// <summary>
        /// Получить данные сотрудника по Id
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("{id:guid}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ActionName("GetEmployeeByIdAsync")]
        public async Task<ActionResult<EmployeeDto>> GetEmployeeByIdAsync(Guid id)
        {
            var employee = await _employeeRepository.GetByIdAsync(id);

            if (employee == null)
                return NotFound();

            return Ok(_mapper.Map<EmployeeDto>(employee));
        }

        /// <summary>
        /// Добавить сотрудника
        /// </summary>
        /// <param name="entity">Сущность сотрудника</param>
        /// <param name="roles">Роли сотрудника</param>
        /// <returns></returns>
        [HttpPut]
        //[ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<ActionResult> AddEmployeeAsync( EmployeeSaveDto entity, [FromQuery] List<string> roles)
        {
            var newEmploeeDto = _mapper.Map<EmployeeDto>(entity);
            newEmploeeDto.Id = Guid.NewGuid();
            newEmploeeDto.Roles = (await _rolesRepository.GetAllAsync()).Where(first => roles.Contains(first.Name)).ToList();
            var created = await _employeeRepository.AddAsync(_mapper.Map<Employee>(newEmploeeDto));
            if (created == default)
            {
                return Conflict();
            }
            //return Ok(created);
            return CreatedAtAction(nameof(GetEmployeeByIdAsync),new { id = newEmploeeDto.Id } , newEmploeeDto);
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
        /// <param name="roles">Роли сотрудников</param>
        /// <returns></returns>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Route("{id:guid}")]
        public async Task<IActionResult> UpdateEmployeeByIdAsync(Guid id, [FromQuery] List<string> roles, EmployeeSaveDto entity)//Task<ActionResult<EmployeeDto>>
        {
            var employee = await _employeeRepository.GetByIdAsync(id);

            if (employee == null)
            {
                return NotFound();
            }
            var newEmploeeDto = _mapper.Map<EmployeeDto>(entity);
            newEmploeeDto.Id = employee.Id;
            newEmploeeDto.Roles= (await _rolesRepository.GetAllAsync()).Where(first => roles.Contains(first.Name)).ToList();
            await _employeeRepository.UpdateAsync(_mapper.Map<Employee>(newEmploeeDto));
            return Ok(newEmploeeDto);
        }
    }
}
            
