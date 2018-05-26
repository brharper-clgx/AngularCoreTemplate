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

```
+-- <app-name>
|   +-- <app-name>.web
|   +-- <app-name>.domain
|   +-- <app-name>.data
|   +-- <app-name>.test
```

## Web Layer
In the *\<app-name\>.web* directory, use the .Net CLI to initialize a web project. Also, go ahead and make your first commit after the setup is complete:
```shell
$ cd <app-name>.web
$ dotnet new angular                       # Initialize web layer
$ cd ..
$ git add -A                               # Stage changes
$ git commit -m "Initialize web layer."    # First commit
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
```
+-- <app-name>
|   +-- <app-name>.web
|   +-- <app-name>.domain
|   +-- <app-name>.data
|   +-- <app-name>.test
|   |   +-- WebTests
|   |   |   +-- ControllerTests
|   |   |   |   +-- SampleDataControllerTests.cs
```















[vscode]: https://code.visualstudio.com/?wt.mc_id=adw-brand&gclid=CjwKCAjwi6TYBRAYEiwAOeH7GR-Akqgcd31hHVa6sKOtBb_pn_DJA3Kclgj3KNjswpFYaoy-SOjTWhoC55MQAvD_BwE
[dotnetcore]: https://www.microsoft.com/net/download/windows
[node]: https://nodejs.org/en/
[npm]: https://www.npmjs.com/get-npm