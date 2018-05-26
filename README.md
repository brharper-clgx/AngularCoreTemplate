# Prerequisites
## Download & Install the Following:
- [ ] [Visual Studio Code][vscode]
- [ ] [.Net Core SDK][dotnetcore]
- [ ] [Node][node]

Confirm that Node and its package manage NPM are install by running the following commands:
```shell
$ node -v   # This should output something like: 'v8.11.2'
$ npm -v    # This should output something like: '5.6.0'
```
If versions are listed, then the installations have worked.

# Stand the Application on its Feet
## Layout:
In whatever workspace directory you prefer,  create a directory that will contain each layer of your application. Give it a name, *\<app-name\>*, that concisely denotes your application. For example, if your application is 'Appointment Scheduler' you might call use 'scheduler'. 

Go ahead and layout the directories for each layer of the application. The file structure should be as follows:

>```
>+-- <app-name>
>|   +-- <app-name>.web
>|   +-- <app-name>.domain
>|   +-- <app-name>.data
>|   +-- <app-name>.test
>```

## Web Layer
In the *\<app-name\>.web* directory, use the .Net CLI to initialize a web project. Also, go ahead and make your first commit after the setup is complete:
```shell
$ cd <app-name>.web
$ dotnet new angular                       # Initialize web layer
$ cd ..
$ git add -A                               # Stage changes
$ git commit -m "Initialize web layer."    # First commit
```

Add a *.gitignore* file the the project root, and give it the following:
```
*bin/
*obj/
```

Open the app in **VS Code** (from the project root directory \<app-name\>):
```shell
$ code .
```

Test the app so far by pressing F5 (or using the debugger panel on the left). Your browser should open to *https://localhost:5000*. 

Spend some time exploring the application, both the UI displayed through the browser, and the source code through **VS Code**. This should give you a good guide as to how the web layer operates. 

The directory *ClientApp* contains all of the UI code. The directory *Controllers* contain your Web API endpoints (for GET, POST, PUT, etc.). The Web API is how applications (including your front-end) communicate with the back-end of your application.

The file *Startup.cs* is where your app is configured. We'll do more here later.


## Test Layer
Automated tests will save you from an immense number of headaches. Write and run them often, and you'll catch bugs the moment they emerge and confidently deploy refactored code.

In the *\<app-name\>.test* directory, use the .Net CLI to initialize a new test layer:
```shell
$ cd <app-name>.test
$ dotnet new xunit
```

Our test project needs to know about the layers it will be testing, add a reference to the web layer by running this command:
```shell
$ dotnet add reference ../<app-name>.web/<app-name>.web.csproj
```
Additionally, since there are now multiple project layers, we should create a *solution file* to link them all together. Do this in the project root directory, *\<app-name\>*. The add references to the layers we've made so far:
```shell
$ cd ..
$ dotnet new solution
$ dotnet sln add <app-name>.web/<app-name>.web.csproj <app-name>.test/<app-name>.test.csproj 
```

In your test directory, add a directory for testing your web layer, and in that directory create another directory for testing your controllers. We'll be adding a file for testing the *SampleDataController*:
>```
>+-- <app-name>
>|   +-- <app-name>.web
>|   +-- <app-name>.domain
>|   +-- <app-name>.data
>|   +-- <app-name>.test
>|   |   +-- WebTests
>|   |   |   +-- ControllerTests
>|   |   |   |   +-- SampleDataControllerTests.cs
>```

Here's are some simple example tests your could write:
```cs
using System;
using System.Linq;
using <app-name>.web.Controllers;
using Xunit;

namespace <app-name>.test.web.controllers
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
```

Now would be a good time to make another commit.


## Domain Layer
The domain layer should be the heart of your app, containing all the core logic and models(currently our controllers contain some of this; we'll extract them to the domain shortly). The reason to put the core of your app in a single layer, is so that the other layers can easily be replaced plug-and-play style. This is done by creating *interfaces* that other layers will plug into. Then, if one day you decide you need, for example, a different data-storage means, you can simply replace the data layer without the need of a major refactor of the core of your app.

To initialize the domain layer, run the following:
```shell
$ cd <app-name>.domain
$ dotnet new classlib
$ cd ..
$ dotnet sln add <app-name>.domain/<app-name>.domain.csproj
```

Now, the other layers will have the domain layer as a dependency, but the domain layer should NOT be dependent on them. Thus, we add a reference in the web layer to the domain layer, but not vice versa:
```shell
$ cd <app-name>.web
$ dotnet add reference ../<app-name>.domain/<app-name>.domain.csproj
$ cd ..
$ cd <app-name>.test
$ dotnet add reference ../<app-name>.domain/<app-name>.domain.csproj
```

The SampleDataController file has a class named WeatherForecast. This kind of model belongs in the domain layer. Let's move it there:
>```
>+-- <app-name>
>|   +-- <app-name>.web
>|   +-- <app-name>.domain
>|   |   +-- models
>|   |   |   +-- WeatherForecast.cs
>|   +-- <app-name>.data
>|   +-- <app-name>.test
>```

```cs
namespace <app-name.domain.models
{
    public class WeatherForecast
    {
        public string DateFormatted { get; set; }
        public int TemperatureC { get; set; }
        public string Summary { get; set; }

        public int TemperatureF
        {
            get
            {
                return 32 + (int)(TemperatureC / 0.5556);
            }
        }
    }
}
```

The SampleDataController and its tests will now complain that they are missing references. Remedy this by adding this to the top of each file:
```cs
using <app-name.domain.models;
```

We've moved the WeatherForecast model into the domain layer, but there is still key logic in the controller that *should* be in the domain layer (we wouldn't want to loose this functionality if we replaced our web layer). The process for doing this is quite involved but worthwhile. To do this, we'll create an interface, *IWeatherService* and an implementation of that interface, *WeatherService*. Then, we'll give the controller access to the interface, and tell *Startup.cs* to inject the implementation.

### IWeatherService
>```
>+-- <app-name>
>|   +-- <app-name>.web
>|   +-- <app-name>.domain
>|   |   +-- interfaces
>|   |   |   +-- IWeatherService.cs
>|   |   +-- models
>|   |   +-- services
>|   |   |   +-- WeatherService.cs
>|   +-- <app-name>.data
>|   +-- <app-name>.test
>```

**IWeatherService.cs**
```cs
using System.Collections.Generic;
using <app-name>.domain.models;

namespace <app-name.domain.interfaces
{
    public interface IWeatherService
    {   
        IEnumerable<WeatherForecast> GetForecast();
    }
}
```

**WeatherService.cs**
```cs
using System;
using System.Collections.Generic;
using System.Linq;
using <app-name>.domain.interfaces;
using <app-name>.domain.models;

namespace <app-name>.domain.services
{
    public class WeatherService : IWeatherService
    {
        private static string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        public IEnumerable<WeatherForecast> GetForecast()
        {
            var rng = new Random();
            return Enumerable.Range(1, 5).Select(index => new WeatherForecast
            {
                DateFormatted = DateTime.Now.AddDays(index).ToString("d"),
                TemperatureC = rng.Next(-20, 55),
                Summary = Summaries[rng.Next(Summaries.Length)]
            });
        }
    }
}
```

Now, we strip the logic out of the controller:
```cs
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using <app-name>.domain.interfaces;
using <app-name>.domain.models;

namespace <app-name>.web.Controllers
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
```
(Aside: you may notice our tests are broken. We'll fix them soon).

Now, you may wonder how the WeatherService is getting injected into the controller's constructor. This is done by configuring *Startup.cs* to do so. Add the following to *Startup.cs*:
```cs
...
using <app-name>.domain.interfaces;
using <app-name>.domain.services;

namespace <app-name>.web
{
    public class Startup
    {
        ...
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddTransient<IWeatherService, WeatherService>(); // Dependency Injection
            services.AddMvc();
        }
        ...
    }
}
```
This tells your application to inject a new instance of WeatherService whenever IWeatherService is requested. 

*Finally*, we need to fix the broken unit tests. Currently, however, there really isn't any logic in the controller. So let's move these tests and make the WeatherService tests instead:
>```
>+-- <app-name>
>|   +-- <app-name>.web
>|   +-- <app-name>.domain
>|   +-- <app-name>.data
>|   +-- <app-name>.test
>|   |   +-- Controllers
>|   |   +-- Services
>|   |   |   +-- WeatherServiceTests.cs
>```
**WeatherServiceTets.cs**
```cs
using System;
using System.Linq;
using Xunit;
using <app-name>.domain.models;
using <app-name>.domain.services;

namespace <app-name>.test.domain.services
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

```

But what if we *did* want to unit test our controller? The controller now has a dependency, *IWorksheetService*. But in unit tests, we don't want to test the dependency. We only want to test the logic of the function in question. In order to test the controller then, we would have to create a *fake* version of the *IWorksheetService* for testing. To do this, see [Moq][moq]



[vscode]: https://code.visualstudio.com/?wt.mc_id=adw-brand&gclid=CjwKCAjwi6TYBRAYEiwAOeH7GR-Akqgcd31hHVa6sKOtBb_pn_DJA3Kclgj3KNjswpFYaoy-SOjTWhoC55MQAvD_BwE
[dotnetcore]: https://www.microsoft.com/net/download/windows
[node]: https://nodejs.org/en/
[npm]: https://www.npmjs.com/get-npm
[moq]: https://github.com/Moq/moq4/wiki/Quickstart