@ECHO OFF
IF NOT EXIST "./examples/CommandLineInjector.StructureMap.Example/bin/Debug/netcoreapp2.1" dotnet build ./examples/CommandLineInjector.StructureMap.Example

cd ./examples/CommandLineInjector.StructureMap.Example/bin/Debug/netcoreapp2.1
dotnet CommandLineInjector.StructureMap.Example.dll %*
cd ../../../../../