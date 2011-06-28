To compile this project you have to manually put the referenced R# files in the lib\ReSharper folder

To debug things set devenv.exe as "Start external external program"
For parameters add /ReSharper.Plugin <absolute path>\NBehave.ReSharper.Plugin.dll
you may also add another parameter pointing to a sln or csproj file so you can load a testproject automatically
