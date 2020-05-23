using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Otus.Teaching.PromoCodeFactory.WebHost.Models;

namespace Otus.Teaching.PromoCodeFactory.WebHost.Controllers
{
    /// <summary>
    /// Предпочтения клиентов
    /// </summary>
    [ApiController]
    [Route("api/v1/[controller]")]
    public class PreferencesController
        : ControllerBase
    {
        [HttpGet]
        public Task<ActionResult<List<PreferenceResponse>>> GetPreferencesAsync()
        {
            //TODO: Получение списка предпочтений
            throw new NotImplementedException();
        }
    }
}