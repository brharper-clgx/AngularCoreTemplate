using Moq;
using template.domain.interfaces;
using template.web.controllers;
using Xunit;

namespace template.test.web.controllers
{
    public class SampleDataControllerTests
    {
        [Fact]
        public void SampleDataController_WeatherForecasts_ShouldCallWeatherService()
        {
            // Arrange
            var mockWeatherService = new Mock<IWeatherService>();
            var target = new SampleDataController(mockWeatherService.Object);

            // Act
            target.WeatherForecasts();

            // Assert
            mockWeatherService.Verify(w => w.GetForecast(), Times.Once);
        }
    }
}
