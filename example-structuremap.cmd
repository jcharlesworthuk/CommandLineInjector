@ECHO OFF
IF NOT EXIST "./examples/CommandLineInjector.StructureMap.Example/bin/Debug/netcoreapp2.1" dotnet build ./examples/CommandLineInjector.StructureMap.Example

dotnet ./examples/CommandLineInjector.StructureMap.Example/bin/Debug/netcoreapp2.1/CommandLineInjector.StructureMap.Example.dll %*