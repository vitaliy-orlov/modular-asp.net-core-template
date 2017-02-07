# Modular ASP.NET Core template

I present to you a template to create a modular web application based on ASP.NET Core. It uses the basic features of MVC framework for the organization of a modular architecture. Each module is a dll file, which includes views and static files. Each module has a standard structure MVC application:

```
ModuleName/
├── Controllers/
│   ├── HomeController.cs
│   └── ...
├── Models/
│   ├── Model.cs
│   └── ...    
├── Views/
│   ├── Home
│   │   ├── Index.cshtml
|   |   └── ...
|   └── ...
├── wwwroot/
│   ├── css
|   |   └── ...
│   ├── js
|   |   └── ...
│   ├── img
|   |   └── ...
|   └── ...
├── ModuleInfo.cs
└── project.json
```

## Installing the template

The following steps describe the actions in the *Windows* environment using *Visual Studio*
* Clone repository
* Open solution *ModularWebApp.sln*
* Compile project *Host*
* Compile project *Module.Account*
* Create a folder **Modules** inside 'output' folder of *Host* compilation (example, **Host\bin\Debug\netcoreapp1.1**)
* Copy the contents of the project compilation *Module.Account* to the folder **Modules**
* Run project *Host*

By default, the current folder of **Host.dll** file is taken and it is sought inside its **Modules** folder. But you can also specify a full path to find modules by using *ModulesPath* setting in **appsettings.json file**.

P.S. For \*nix systems actions algorithm similar, only with the use of util *dotnet* 

## The main points for development
### Project Host

Main feature of the project *Host* is the presence of a class *ModuleLoader*, which loads the assembly of each module of the specified folder, and keeps a list of the type *IModuleBase* interfaces. *Startup* class has the following changes:

* Method **ConfigureServices**:
```cs

...

public void ConfigureServices(IServiceCollection services)
{
    string path = Configuration.GetValue<string>(Extensions.ModuleLoader.CONFIG_MODULES_PATH);

    // Load assemblies
    Extensions.ModuleLoader.LoadAssemblies(path);

    // Add custom locations for searching Views in modules
    services.Configure<RazorViewEngineOptions>(options =>
    {
        options.AreaViewLocationFormats.Add("{2}/Views/{1}/{0}.cshtml");
        options.AreaViewLocationFormats.Add("{2}/Views/Shared/{0}.cshtml");
    });

    var mvc = services.AddMvc();

    // Load each module to MVC service
    foreach (var module in Extensions.ModuleLoader.ModulesList)
    {
        module.InitServices(services);

        mvc.AddApplicationPart(module.Assembly).AddRazorOptions(o => {
            o.AdditionalCompilationReferences.Add(MetadataReference.CreateFromFile(module.Assembly.Location));
            o.FileProviders.Add(new EmbeddedFileProvider(module.Assembly, module.Assembly.GetName().Name.Replace("." + module.AreaName, "")));
        });
    }
}

...

```

* Method **Configure**:

```cs

...

public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
{
    loggerFactory.AddConsole(Configuration.GetSection("Logging"));
    loggerFactory.AddDebug();

    if (env.IsDevelopment())
    {
        app.UseDeveloperExceptionPage();
        app.UseBrowserLink();
    }
    else
    {
        app.UseExceptionHandler("/Home/Error");
    }

    app.UseStaticFiles();

    // Add modules assemblies for static files serving
    foreach (var module in Extensions.ModuleLoader.ModulesList)
    {
        app.UseStaticFiles(new StaticFileOptions()
        {
            FileProvider = new EmbeddedFileProvider(module.Assembly, module.Assembly.GetName().Name + ".wwwroot"),
            RequestPath = new PathString("/" + module.AreaName)
        });
    }

    app.UseMvc(routes =>
    {
        // Special route for handling Area request
        routes.MapRoute(
            name: "areaRoute",
            template: "{area:exists}/{controller=Home}/{action=Index}");
        routes.MapRoute(
            name: "default",
            template: "{controller=Home}/{action=Index}/{id?}");
    });
}

...

```

And special attention should be paid to the file *\_Layout.cshtml*, which shows how you can generate a navigation bar based on modules.

```cs
...

@foreach (var module in Host.Extensions.ModuleLoader.ModulesList)
{
    <li role="presentation" class="dropdown">
        <a class="dropdown-toggle" data-toggle="dropdown" href="#" role="button" aria-haspopup="true" aria-expanded="false">
            @module.MainTabTitle <span class="caret"></span>
        </a>
        <ul class="dropdown-menu">
            @foreach (var item in module.Controllers)
            {
                <li><a href="@Url.Action("Index", item.controller, new { Area = item.area })">@item.title</a></li>
            }
        </ul>
    </li>
}

...
```

## Create a new module (the highlights and restrictions)

When you create a new module, it is necessary to observe the following rules:
* The module should be dependent on the project *Core*
* The module must implement the interface *IModuleBase*
* Each controller must have the attribute *Area*
* To specify the name of the controller to display the name on the navigation bar, use the attribute *MenuTitle*
* The attribute value *Area* and the last word in the name of the project should be the same. For example, if your module is called *Product*, then each controller must have *Area* with a value of *Product*. If your project is called *My.Modular.App.Module.Product*, the *Area* should be set to *Product*
* The module must have a constructor that takes a single parameter of type Assembly
* The file **project.json** must have the following entry: ```"buildOptions": { "embed": [ "Views/**", "wwwroot/**" ] }```

### StaticResourcePathConverterTagHelper 

This *TagHelper* may help you in specifying the url address to the static module resources. For example, if you just specify ```~/js/test.js``` in an attribute *src* of element *script*,  you will get the address of ```</ApplicationName>/js/test.js```. But if you use *TagHelper*, the address will be the following ```</ApplicationName>/ControllerAreaName/js/test.js```. Example:

```html
<link rel="stylesheet" cth-src="~/css/site.css" /> -> <link rel="stylesheet" href="/myapp/module/css/site.css" />
<script type="text/javascript" cth-src="~/js/test.js"></script> -> <script type="text/javascript" src="/myapp/module/js/test.js"></script>
```
