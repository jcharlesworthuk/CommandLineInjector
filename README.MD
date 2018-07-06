# Command Line Injector

 Supercharged command line utilities in .NET Core!
Parameter binding for .NET Core console apps!

Framework for adding dependency injection and parameter binding to console apps, based on Microsoft's [Microsoft.Extensions.CommandLineUtils](https://msdn.microsoft.com/en-us/magazine/mt763239.aspx?f=255&MSPPError=-2147217396) package.


```csharp
namespace MyConsoleApp
{
    class Program
    {
        static void Main(string[] args)
        {
            var serviceCollection = new ServiceCollection()
                                        .AddTransient<IEmailService, SendGridEmailService>()
                                        .AddTransient<SendEmailCommand>();

            var app = new CommandLineInjectingApplication("myapp", new MicrosoftDependencyInjectionAdapter(serviceCollection, null));

            app.Command<SendEmailCommand>("send");

            app.Execute(args);
        }
    }
}
```

## What The  Why And How?


Let's say you've architected your code to use dependency injection and you have your functionality isolated in nice, descriptive and highly testable classes like this

```csharp
public class SendEmailCommand
{
    private readonly IEmailService _service; 

    public SendEmailCommand(IEmailService service)
    {
        _service = service;
    }

    public async Task Invoke(string address, string content)
    {
        if (address == null)
            throw new ArgumentNullException(nameof(address));

        await _service.Send(new Email { Address = address, Content = content });

        // ... more code if you like
    }
}
```

Isolating your functionality like this is a great way to abstract code into more reusable chunks and there are many DI frameworks out there to help with the dependency injection side of things (including [Microsoft's own](https://docs.microsoft.com/en-us/aspnet/core/fundamentals/dependency-injection?view=aspnetcore-2.1)).  If you were using ASP.Net the above command can easily be injected into a controller and ASP.Net will take care of the parameter binding for you.

```csharp
public class EmailsController : Controller
{
    private readonly SendEmailCommand _sendEmail;

    public EmailsController(SendEmailCommand sendEmail)
    {
        _sendEmail = sendEmail;
    }

    [HttpPost]
    public async Task<IActionResult> SendNew(string address, [FromBody]string content)
    {
        await _sendEmail.Invoke(address, content);
        return Accepted();
    }
}
```

But what if you wanted to execute your command from a console app?

```csharp
class Program
{
    static void Main(string[] args)
    {
        var serviceProvider = new ServiceCollection()
            .AddTransient<IEmailService, SendGridEmailService>()
            .AddTransient<SendEmailCommand>()
            .BuildServiceProvider();

        var command = serviceProvider.GetService<SendEmailCommand>();

        var address = args[0];
        var content = args[1];

        command.Invoke(address, content).Wait();
    }
}
```
The above code will work, but it's messy



If you've used ASP.Net you'll know how simple parameter binding is in controllers, and since controllers can be wired up with dependency injection that makes writing code like this a breeze

Isolating your functionality like this is a great way to abstract functionality into more reusable chunks, and there are many DI frameworks out there to help with the dependency injection side of things (including Microsoft's own).  If you were using ASP.Net the above command can easily be injected into a controller and ASP.Net will take care of the parameter binding for you.  Making a POST request to `~/mycontroller?someValue=hello` will bind the 'someValue' parameter for you and let you call your command with the IExampleService injected in.


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




If you are building command line utilities in .NET Core you may already be aware of Microsoft's de-facto package for handling command line arguments [Microsoft.Extensions.CommandLineUtils](https://msdn.microsoft.com/en-us/magazine/mt763239.aspx?f=255&MSPPError=-2147217396).  Well _Command Line Injector_ takes this one step further and infers the commands and options from the method signature of your command classes

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