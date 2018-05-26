using System.Collections.Generic;
using template.domain.models;

namespace template.domain.interfaces
{
    public interface IWeatherService
    {   
        IEnumerable<WeatherForecast> GetForecast();
    }
}