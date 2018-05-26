# Prerequisites
## Download & Install the Following:
- [ ] [Visual Studio Code][vscode]
- [ ] [.Net Core SDK][dotnetcore]
- [ ] [Node][node]

Confirm that Node and its package manager NPM are install by running the following commands:
```shell
$ dotnet --version 
$ node -v   
$ npm -v   
```
If a version listed, then the item in question is installed.

# Stand the Application on its Feet
## Layout:
In whatever workspace directory you prefer,  create a directory that will contain each layer of your application. Give it a name that concisely denotes your application. For example, if your application is 'Appointment Scheduler' you might use 'scheduler'. For our purposes, we'll use '*myapp*'.

Go ahead and lay out the directories for each layer of the application. The file structure should be as follows:

>```
>+-- myapp
>|   +-- myapp.web
>|   +-- myapp.domain
>|   +-- myapp.data
>|   +-- myapp.test
>```

## Web Layer
In the *myapp.web* directory, use the .Net CLI to initialize a web project. Also, go ahead and make your first commit after the setup is complete:
```shell
$ cd myapp.web
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

Open **VS Code** from the project root directory *myapp* with:
```shell
$ code .
```

In **VS Code**, test the app so far by pressing F5 (or using the debugger panel on the left). Your web browser should open to *https://localhost:5000*. 

Spend some time exploring the application, both the UI displayed through the browser, and the source code through **VS Code**. This should give you a good guide as to how the web layer operates. 

The directory *ClientApp* contains all of the UI code (angular framework). The directory *Controllers* contains your Web API endpoints (for HTTP GET, POST, PUT, etc.). The Web API is how applications (including your front-end) communicate with the back-end of your application.

The file *Startup.cs* is where your app is configured. We'll do more here later.


## Test Layer
Automated tests will save you from an immense number of headaches. Write and run them often, and you'll catch bugs the moment they emerge and confidently deploy refactored code.

In the *myapp.test* directory, use the .Net CLI to initialize a new test layer:
```shell
$ cd myapp.test
$ dotnet new xunit
```

Our test project needs to know about the layers it will be testing, add a reference to the web layer by running this command:
```shell
$ dotnet add reference ../myapp.web/myapp.web.csproj
```
Additionally, since there are now multiple project layers, we should create a *solution file* to link them all together. Do this in the project root directory, *myapp*. Then add references to the layers we've made so far:
```shell
$ cd ..
$ dotnet new solution
$ dotnet sln add myapp.web/myapp.web.csproj 
$ dotnet sln add myapp.test/myapp.test.csproj 
```

In your test directory, add another directory for testing the web layer. In this new directory create yet another directory for testing your controllers. We'll be adding a file for testing the *SampleDataController*:
>```
>+-- myapp
>|   +-- myapp.web
>|   +-- myapp.domain
>|   +-- myapp.data
>|   +-- myapp.test
>|   |   +-- Web
>|   |   |   +-- Controller
>|   |   |   |   +-- SampleDataControllerTests.cs
>```

Here's are some simple example tests your could write:
```cs
using System;
using System.Linq;
using myapp.web.Controllers;
using Xunit;

namespace myapp.test.web.controllers
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
```shell
$ git add -A
$ git commit -m "Test SampleData controller"
```

## Domain Layer
The domain layer should be the heart of your app, containing all the core logic and models. Currently, our controllers contain all of our logic. We'll extract this logic to the domain shortly.

The reason to put the core of your app in a single layer, is so that the other layers can easily be replaced plug-and-play style. This is done by creating *interfaces* that other layers will plug into. Then, for example, if one day you decide you need a different means of data-storage, you can simply replace the data layer with a new one without the need to do a major refactor of the core functionality of your app.

To initialize the domain layer, run the following:
```shell
$ cd myapp.domain
$ dotnet new classlib
$ cd ..
$ dotnet sln add myapp.domain/myapp.domain.csproj
```

Now, the other layers will have the domain layer as a dependency, but the domain layer should NOT be dependent on them. Thus, we add a reference in the web layer to the domain layer, but not vice versa:
```shell
$ cd myapp.web
$ dotnet add reference ../myapp.domain/myapp.domain.csproj
$ cd ..
$ cd myapp.test
$ dotnet add reference ../myapp.domain/myapp.domain.csproj
```

The SampleDataController file has a class named WeatherForecast. This kind of model belongs in the domain layer. Let's move it there:
>```
>+-- myapp
>|   +-- myapp.web
>|   +-- myapp.domain
>|   |   +-- models
>|   |   |   +-- WeatherForecast.cs
>|   +-- myapp.data
>|   +-- myapp.test
>```

```cs
namespace myapp.domain.models
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
using myapp.domain.models;
```

We've moved the WeatherForecast model into the domain layer, but there is still key logic in the controller that *should* be in the domain layer (we wouldn't want to loose this functionality if we replaced our web layer). The process for doing this is quite involved but worthwhile. To do this, we'll create an interface, *IWeatherService* and an implementation of that interface, *WeatherService*. Then, we'll give the controller access to the interface, and tell *Startup.cs* to inject the implementation.

### IWeatherService
>```
>+-- myapp
>|   +-- myapp.web
>|   +-- myapp.domain
>|   |   +-- interfaces
>|   |   |   +-- IWeatherService.cs
>|   |   +-- models
>|   |   +-- services
>|   |   |   +-- WeatherService.cs
>|   +-- myapp.data
>|   +-- myapp.test
>```

**IWeatherService.cs**
```cs
using System.Collections.Generic;
using myapp.domain.models;

namespace myapp.domain.interfaces
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
using myapp.domain.interfaces;
using myapp.domain.models;

namespace myapp.domain.services
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
...
using myapp.domain.interfaces;
using myapp.domain.models;

namespace myapp.web.Controllers
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
using myapp.domain.interfaces;
using myapp.domain.services;

namespace myapp.web
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
>+-- myapp
>|   +-- myapp.web
>|   +-- myapp.domain
>|   +-- myapp.data
>|   +-- myapp.test
>|   |   +-- Controllers
>|   |   +-- Services
>|   |   |   +-- WeatherServiceTests.cs
>```
**WeatherServiceTests.cs**
```cs
using System;
using System.Linq;
using Xunit;
using myapp.domain.models;
using myapp.domain.services;

namespace myapp.test.domain.services
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

## Data Layer
Todo

[vscode]: https://code.visualstudio.com/?wt.mc_id=adw-brand&gclid=CjwKCAjwi6TYBRAYEiwAOeH7GR-Akqgcd31hHVa6sKOtBb_pn_DJA3Kclgj3KNjswpFYaoy-SOjTWhoC55MQAvD_BwE
[dotnetcore]: https://www.microsoft.com/net/download/windows
[node]: https://nodejs.org/en/
[npm]: https://www.npmjs.com/get-npm
[moq]: https://github.com/Moq/moq4/wiki/Quickstart