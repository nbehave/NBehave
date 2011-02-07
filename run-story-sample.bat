@ECHO OFF

@PUSHD .

@CD build\Debug\UnitTests\

@NBehave-Console.exe TestAssembly.dll /storyOutput=output.txt

@POPD