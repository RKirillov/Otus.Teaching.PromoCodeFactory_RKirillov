using System;
using Microsoft.AspNetCore.Mvc;

namespace Otus.Teaching.PromoCodeFactory.WebHost.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class CustomersController
        : ControllerBase
    {
        [HttpGet]
        public IActionResult GetCustomers()
        {
            //TODO: Добавить получение списка клиентов
            return Ok();
        }
        
        [HttpGet("{id}")]
        public IActionResult GetCustomers(Guid id)
        {
            //TODO: Добавить получение клиента
            return Ok();
        }
        
        [HttpPost]
        public IActionResult CreateCustomer()
        {
            //TODO: Добавить создание нового клиента
            return Ok();
        }
        
        [HttpPut("{id}")]
        public IActionResult EditCustomers(Guid id)
        {
            //TODO: Обновить данные клиента
            return Ok();
        }
        
        [HttpDelete]
        public IActionResult DeleteCustomer(Guid id)
        {
            //TODO: Удаление клиента вместе с выданными ему промокодами
            return Ok();
        }
    }
}