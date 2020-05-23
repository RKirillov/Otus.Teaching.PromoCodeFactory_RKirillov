using Microsoft.AspNetCore.Mvc;

namespace Otus.Teaching.PromoCodeFactory.WebHost.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class PreferencesController
        : ControllerBase
    {
        [HttpGet]
        public IActionResult GetPreferences()
        {
            //TODO: Получение списка предпочтений
            return Ok();
        }
    }
}