# Command Line Injector

Framework for adding dependency injection to Microsoft's Microsoft.Extensions.CommandLineUtils package to build supercharged command line utilities in .NET Core!

## Introduction

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