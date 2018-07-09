# Command Line Injector

Parameter binding + Dependency Injection for .NET Core console apps.

## Overview

Let's say you have some code in a class like this that calls an injected dependency...

```csharp
public class SendEmailCommand
{
    private readonly IEmailService _service; 

    // inject a service class that can perform the actual sending
    // (eg. SendGrid/Mailchimp client)
    public SendEmailCommand(IEmailService service)
    {
        _service = service; 
    }

    // Invokes this command to send an email
    public async Task Invoke(string address, string content = "Empty")
    {
        if (address == null)
            throw new ArgumentNullException(nameof(address));

        // Build the email
        var email = new Email { Address = address, Content = content };

        // Send the email using the injected service
        await _service.Send(email);
    }
}
```

If we were using ASP.Net the above command class is pretty easy to wire up and call from a controller

```csharp
public class EmailsController : Controller
{
    private readonly SendEmailCommand _command;

    public EmailsController(SendEmailCommand command)
    {
        _command = command;
    }

    [HttpPost]
    public async Task<IActionResult> Send(string address, string content)
    {
        await _command.Invoke(address, content);
        return Accepted();
    }
}
```

In the above example the parameter binding and dependency injection are handled for you in ASP.Net.  Making a POST request to `~/emails?address=test@example.com?content=Hello` will bind the 'address' and 'content' parameters for you and let you call your command with the SendEmailCommand injected in...

### But what about Console apps?  

Maybe you want to execute your command manaully from a console app, build a full command line runner for your application, or maybe you just want to use DI in console apps to keep your code organised.

The Command Line Injector package will allow you to bind the command line parameters to class methods, and resolve the dependencies at runtime like this.

```csharp
namespace EmailSenderConsoleApp
{
    class Program
    {
        static void Main(string[] args)
        {
            // Register dependencies to a new collection
            var serviceCollection = new ServiceCollection()
                                        .AddTransient<IEmailService, SendGridEmailService>()
                                        .AddTransient<SendEmailCommand>();

            // Create the application
            var app = new CommandLineInjectingApplication("emailsender", new MicrosoftDependencyInjectionAdapter(serviceCollection, null));

            // Add our command class
            app.Command<SendEmailCommand>("send");

            // Execute the application with the command line argument array
            app.Execute(args);
        }
    }
}
```

The above console app can be run with

`> dotnet emailsender send test@example.com --content="Hello World"`

The package uses Microsoft's [Microsoft.Extensions.CommandLineUtils](https://msdn.microsoft.com/en-us/magazine/mt763239.aspx?f=255&MSPPError=-2147217396) package, extending the base `CommandLineApplication` by adding a generic overload of the `Command<T>()` method.  This means you can decorate your command class and get help text


```csharp
[Description("Sends an email with content to the given address")]
public class SendEmailCommand
{
    private readonly IEmailService _service; 

    public SendEmailCommand(IEmailService service)
    {
        _service = service;
    }

    public async Task Invoke(
            [Description("Email address to send to")] string address, 
            [Description("Optional email body content")] string content = "Empty")
    {
        if (address == null)
            throw new ArgumentNullException(nameof(address));

        await _service.Send(new Email { Address = address, Content = content });

        // ... more code if you like
    }
}
```

Passing the help option `--help` (or `-?`) will print the contents of the description attributes.

```
> dotnet emailsender send --help

  Usage: dotnet emailsender send [arguments] [options]

  Arguments:
    [address] Email address to send to

  Options:
    -c|--content <value>      Optional email body content
    -?|-h|--help              Show help information
```

## Multiple Commands

The true power of all of this comes when you add multiple command registrations to build up a complex and multi-functional command line application.  Let's add a second command to Retry and email.


```csharp
[Description("Retries the last email sent to an address")]
public class RetryEmailCommand
{
    private readonly IEmailService _service; 

    public SendEmailCommand(IEmailService service)
    {
        _service = service;
    }

    public async Task Invoke([Description("Email address to resend to")] string address)
    {
        if (address == null)
            throw new ArgumentNullException(nameof(address));

        await _service.Retry(address);

        // ... more code if you like
    }
}


namespace EmailSenderConsoleApp
{
    class Program
    {
        static void Main(string[] args)
        {
            // Register dependencies to a new collection
            var serviceCollection = new ServiceCollection()
                                        .AddTransient<IEmailService, SendGridEmailService>()
                                        .AddTransient<SendEmailCommand>()
                                        .AddTransient<RetryEmailCommand>();

            // Create the application
            var app = new CommandLineInjectingApplication("emailsender", new MicrosoftDependencyInjectionAdapter(serviceCollection, null));

            // Add our send command
            app.Command<SendEmailCommand>("send");

            // Add our retry command
            app.Command<RetryEmailCommand>("retry");

            // Execute the application with the command line argument array
            app.Execute(args);
        }
    }
}

```

Now if we check the help text you can see we have two commands available


```
> dotnet emailsender --help

  Usage: dotnet emailsender [arguments] [options]

  Options:
    -?|-h|--help  Show help information

  Commands:
    send     Sends an email with content to the given address
    retry    Retries the last email sent to an address

  Use "dotnet  [command] --help" for more information about a command.    
```

## Service Classes

You don't have to have a command class for every console command.  You can also specify a method from a "service" type class to use as a console app command.  For example we could place both our _Send_ and _Retry_ methods onto one EmailCommandService class

```csharp
public class EmailCommands
{
    private readonly IEmailService _service; 

    public EmailCommands(IEmailService service)
    {
        _service = service;
    }

    [Description("Retries the last email sent to an address")]
    public async Task Retry([Description("Email address to resend to")] string address)
    {
        if (address == null)
            throw new ArgumentNullException(nameof(address));

        await _service.Retry(address);
    }


    [Description("Sends an email with content to the given address")]
    public async Task Send(
            [Description("Email address to send to")] string address, 
            [Description("Optional email body content")] string content = "Empty")
    {
        if (address == null)
            throw new ArgumentNullException(nameof(address));

        await _service.Send(new Email { Address = address, Content = content });
    }
}


namespace EmailSenderConsoleApp
{
    class Program
    {
        static void Main(string[] args)
        {
            // Register dependencies to a new collection
            var serviceCollection = new ServiceCollection()
                                        .AddTransient<IEmailService, SendGridEmailService>()
                                        .AddTransient<EmailCommands>();

            // Create the application
            var app = new CommandLineInjectingApplication("emailsender", new MicrosoftDependencyInjectionAdapter(serviceCollection, null));

            // Add our sendand retry command methods on the EmailCommands class
            app.Commands<EmailCommands>(nameof(EmailCommands.Send), nameof(EmailCommands.Retry));

            // Execute the application with the command line argument array
            app.Execute(args);
        }
    }
}

```

The above example will produce the same result as having two command classes.


```
> dotnet emailsender --help

  Usage: dotnet emailsender [arguments] [options]

  Options:
    -?|-h|--help  Show help information

  Commands:
    send     Sends an email with content to the given address
    retry    Retries the last email sent to an address

  Use "dotnet  [command] --help" for more information about a command.    
```

## Global Option Flags

The console app also supports adding command-line options that are common to every command you register, and can be used to configure the DI container before it is resolved.  For example the following snippet adds the "provider" option to all commands registered

```csharp

namespace EmailSenderConsoleApp
{
    class Program
    {
        static void Main(string[] args)
        {
            // Create our global option flag
            var providerOption = new ContainerConfigurationOption
                    {
                        Name = "provider",
                        ShortcutName = "p",
                        HelpText = "Sets the provider in the DI container",
                        HasValue = true
                    };

            // Register dependencies to a new collection
            var serviceCollection = new ServiceCollection()
                                        .AddTransient<EmailCommands>();

            // Create the application
            var app = new CommandLineInjectingApplication("emailsender", new MicrosoftDependencyInjectionAdapter(serviceCollection, (scopeBuilder, commands) =>
            {
                
                // *** ALTER THE SERVICE COLLECTION HERE **
                scopeBuilder.AddTransient<IEmailService>(...);

                return scopeBuilder;
            });

            // Add the global option to all commands
            app.AddToSubsequentAllCommands(providerOption);

            // Add our send and retry command methods on the EmailCommands class
            app.Commands<EmailCommands>(nameof(EmailCommands.Send), nameof(EmailCommands.Retry));

            // Execute the application with the command line argument array
            app.Execute(args);
        }
    }
}

```

Now when we query the available options we will see our extra "--provider" option has been applied to both the commands:


```
> dotnet emailsender send --help

  Usage: dotnet emailsender send [arguments] [options]

  Arguments:
    [address] Email address to send to

  Options:
    -c|--content <value>      Optional email body content
    -p|--provider <value>     Sets the provider in the DI container
    -?|-h|--help              Show help information
```

If we now run any command with the new "--provider" option set the code will drop into the scope builder expression and allow us to modify the `IEmailService` dependency based on the value of the "--provider=XXX" option

`> dotnet emailsender send test@example.com --provider=Mailchimp`



# DI Frameworks

The base _CommandLineInjector_ package contains the console app class (inherited from [Microsoft.Extensions.CommandLineUtils.CommandLineApplication](https://docs.microsoft.com/en-us/dotnet/api/microsoft.extensions.commandlineutils.commandlineapplication?view=aspnetcore-1.1)) and the interfaces required for dependency injection, but you have to also include the adapter for your DI Framework.  Adapters are available for the following Dependency Injection frameworks

## CommandInjector.Microsoft.DependencyInjection

This adapter uses the [Microsoft.Extensions.DependencyInjection](https://github.com/aspnet/DependencyInjection) package which is commonly used in ASP.Net Core and EF Core scenarios.

```csharp
namespace CommandInjector.Microsoft.DependencyInjection.Example
{
    class Program
    {
        static void Main(string[] args)
        {
            var serviceCollection = new ServiceCollection()
                .AddTransient<IExampleService, ExampleService>()
                .AddTransient<TestSimpleCommandClass>();

            var containerAdapter = new MicrosoftDependencyInjectionAdapter(serviceCollection, (scopeBuilder, commands) =>
            {
                scopeBuilder.AddSingleton<IExampleConfiguration>(new ExampleConfiguration(commands));
                return scopeBuilder;
            });

            var app = new CommandLineInjectingApplication("commandlineinjector-example", containerAdapter);

            app.RequiresCommand();
            app.AddToSubsequentAllCommands(ExampleConfiguration.ConfigValueOption);

            app.Command<TestSimpleCommandClass>("simple");
            app.Execute(args);

            Console.ReadLine();
        }
    }
}
```


## CommandLineInjector.Autofac

Adapter for the popular [Autofac](https://autofac.org/) IoC Container

```csharp
namespace CommandLineInjector.Autofac.Example
{
    class Program
    {
        static void Main(string[] args)
        {
            var builder = new ContainerBuilder();
            builder.RegisterType<ExampleService>().As<IExampleService>();
            builder.RegisterType<TestSimpleCommandClass>().AsSelf();

            var autofacContainer = builder.Build();

            var containerAdapter = new AutofacContainerAdapter(autofacContainer, (scopeBuilder, commands) =>
            {
                scopeBuilder.RegisterInstance(new ExampleConfiguration(commands)).As<IExampleConfiguration>();
                return scopeBuilder;
            });

            var app = new CommandLineInjectingApplication("commandlineinjector-example", containerAdapter);

            app.RequiresCommand();
            app.AddToSubsequentAllCommands(ExampleConfiguration.ConfigValueOption);

            app.Command<TestSimpleCommandClass>("simple");
            app.Execute(args);
        }
    }
}
```

## CommandLineInjector.StructureMap

Adapter for [StructureMap](http://structuremap.github.io/)

```csharp
namespace CommandLineInjector.StructureMap.Example
{
    class Program
    {
        static void Main(string[] args)
        {
            var structureMapContainer = new Container(config =>
            {
                config.For<IExampleService>().Use<ExampleService>();
            });

            var containerAdapter = new StructureMapContainerAdapter(structureMapContainer, (container, commands) =>
            {
                container.Configure(config =>
                {
                    config.For<IExampleConfiguration>().Use(new ExampleConfiguration(commands));
                });
                return container;
            });
            
            var app = new CommandLineInjectingApplication("commandlineinjector-example", containerAdapter);

            app.RequiresCommand();
            app.AddToSubsequentAllCommands(ExampleConfiguration.ConfigValueOption);

            app.Command<TestSimpleCommandClass>("simple");
            app.Execute(args);
        }
    }
}
```

## CommandLineInjector.Lamar

[Lamar](https://jasperfx.github.io/lamar/) is the long-term replacement for StructureMap, the adapter for Lamar looks very similar to the StructureMap adapter.

```csharp
namespace CommandLineInjector.Lamar.Example
{
    class Program
    {
        static void Main(string[] args)
        {
            var structureMapContainer = new Container(config =>
            {
                config.For<IExampleService>().Use<ExampleService>();
                config.For<IExampleConfiguration>().Use<ExampleConfiguration>();
                config.Injectable<ExampleConfiguration>();
            });

            var containerAdapter = new LamarContainerAdapter(structureMapContainer, (container, commands) =>
            {
                container.Inject(new ExampleConfiguration(commands));
                return container;
            });

            var app = new CommandLineInjectingApplication("commandlineinjector-example", containerAdapter);

            app.RequiresCommand();
            app.AddToSubsequentAllCommands(ExampleConfiguration.ConfigValueOption);

            app.Command<TestSimpleCommandClass>("simple");
            app.Execute(args);
        }
    }
}
```


## Why And How?

If you've used ASP.Net you'll know how simple parameter binding is in controllers, and since controllers can be wired up with dependency injection that makes writing code like this a breeze

```csharp
public class MyController : Controller
{
    private readonly SimpleCommandClass _command;

    public MyController(SimpleCommandClass command)
    {
        _command = command;
    }

    [HttpPost]
    public async Task<IActionResult> ExecuteCommand(string someValue)
    {
        await _command.Invoke(someValue);
        return Accepted();
    }
}
```

Isolating your functionality like this is a great way to abstract functionality into more reusable chunks, and there are many DI frameworks out there to help with the dependency injection side of things (including Microsoft's own).  



```
var argument = app.Argument("arg name", "arg description");

app.OnExecute(() =>
{
    if (!string.IsNullOrWhiteSpace(argument.Value))
    {
        Console.WriteLine($"Argument value: {argument.Value}");
    }
    return 0;
});
```