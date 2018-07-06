@ECHO OFF
IF NOT EXIST "./examples/CommandLineInjector.Lamar.Example/bin/Debug/netcoreapp2.1" dotnet build ./examples/CommandLineInjector.Lamar.Example

cd ./examples/CommandLineInjector.Lamar.Example/bin/Debug/netcoreapp2.1
dotnet CommandLineInjector.Lamar.Example.dll %*
cd ../../../../../