@ECHO OFF
IF NOT EXIST "./examples/CommandLineInjector.Lamar.Example/bin/Debug/netcoreapp2.1" dotnet build ./examples/CommandLineInjector.Lamar.Example

dotnet ./examples/CommandLineInjector.Lamar.Example/bin/Debug/netcoreapp2.1/CommandLineInjector.Lamar.Example.dll %*