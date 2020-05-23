using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Otus.Teaching.PromoCodeFactory.WebHost.Models;

namespace Otus.Teaching.PromoCodeFactory.WebHost.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class PromocodesController
        : ControllerBase
    {
        [HttpGet]
        public Task<ActionResult<List<PromoCodeShortResponse>>> GetPromocodesAsync()
        {
            //TODO: Получить все промокоды, включая их предпочтения 
            throw new NotImplementedException();
        }
        
        [HttpPost]
        public Task<IActionResult> GivePromocodesToCustomersWithPreferenceAsync()
        {
            //TODO: Создать промокод и выдать его клиентам с указанным предпочтением,
            throw new NotImplementedException();
        }
    }
}