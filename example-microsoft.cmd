@ECHO OFF
IF NOT EXIST "./examples/CommandLineInjector.Microsoft.DependencyInjection.Example/bin/Debug/netcoreapp2.1" dotnet build ./examples/CommandLineInjector.Microsoft.DependencyInjection.Example

dotnet ./examples/CommandLineInjector.Microsoft.DependencyInjection.Example/bin/Debug/netcoreapp2.1/CommandLineInjector.Microsoft.DependencyInjection.Example.dll %*
