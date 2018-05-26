using System;
using System.Linq;
using Xunit;
using template.domain.models;
using template.domain.services;

namespace template.test.domain.services
{
    public class WeatherServiceTests
    {
        [Fact]
        public void SampleDataController_WeatherForecasts_ShouldReturnFiveItems()
        {
            // Arrange
            var target = new WeatherService();
            
            // Act
            var result = target.GetForecast();

            // Assert
            Assert.Equal(5, result.Count());
        }

        [Fact]
        public void SampleDataController_WeatherForecasts_ForecastDataIsPopulated()
        {
            // Arrange
            var target = new WeatherService();
            
            // Act
            var result = target.GetForecast();

            // Assert
            Assert.NotNull(result.FirstOrDefault().DateFormatted);
            Assert.NotNull(result.FirstOrDefault().Summary);
            Assert.NotNull(result.FirstOrDefault().TemperatureC);
            Assert.NotNull(result.FirstOrDefault().TemperatureF);
        }
    }
}
