using System;
using System.Linq;
using template.web.Controllers;
using Xunit;

namespace template.test.web.controllers
{
    public class SampleDataControllerTests
    {
        [Fact]
        public void SampleDataController_WeatherForecasts_ShouldReturnFiveItems()
        {
            // Arrange
            var target = new SampleDataController();
            
            // Act
            var result = target.WeatherForecasts();

            // Assert
            Assert.Equal(5, result.Count());
        }

        [Fact]
        public void SampleDataController_WeatherForecasts_ForecastDataIsPopulated()
        {
            // Arrange
            var target = new SampleDataController();
            
            // Act
            var result = target.WeatherForecasts();

            // Assert
            Assert.NotNull(result.FirstOrDefault().DateFormatted);
            Assert.NotNull(result.FirstOrDefault().Summary);
            Assert.NotNull(result.FirstOrDefault().TemperatureC);
            Assert.NotNull(result.FirstOrDefault().TemperatureF);
        }
    }
}
