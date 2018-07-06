@ECHO OFF
IF NOT EXIST "./examples/CommandLineInjector.Autofac.Example/bin/Debug/netcoreapp2.1" dotnet build ./examples/CommandLineInjector.Autofac.Example

cd ./examples/CommandLineInjector.Autofac.Example/bin/Debug/netcoreapp2.1
dotnet CommandLineInjector.Autofac.Example.dll %*
cd ../../../../../