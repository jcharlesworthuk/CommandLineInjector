@ECHO OFF
IF NOT EXIST "./examples/CommandLineInjector.Autofac.Example/bin/Debug/netcoreapp2.1" dotnet build ./examples/CommandLineInjector.Autofac.Example

dotnet ./examples/CommandLineInjector.Autofac.Example/bin/Debug/netcoreapp2.1/CommandLineInjector.Autofac.Example.dll %*
