using Microsoft.AspNetCore.Mvc;

namespace Otus.Teaching.PromoCodeFactory.WebHost.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class PromocodesController
        : ControllerBase
    {
        [HttpGet]
        public IActionResult GetPromocodes()
        {
            //TODO: Получить все промокоды, включая их предпочтения 
            return Ok();
        }
        
        [HttpPost]
        public IActionResult GivePromocodesToCustomersWithPreference()
        {
            //TODO: Создать промокод и выдать его клиентам с указанным предпочтением,
            return Ok();
        }
    }
}