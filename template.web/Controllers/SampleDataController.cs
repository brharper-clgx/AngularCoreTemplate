using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using template.domain.interfaces;
using template.domain.models;

namespace template.web.controllers
{
    [Route("api/[controller]")]
    public class SampleDataController : Controller
    {
        private IWeatherService _weatherService;
        public SampleDataController(IWeatherService weatherService)
        {
            _weatherService = weatherService;
        }

        [HttpGet("[action]")]
        public IEnumerable<WeatherForecast> WeatherForecasts()
        {
            return this._weatherService.GetForecast();
        }
    }
}
