#!/bin/bash

# ********
# To pass parameters
# ./buildframework/FAKE/tools/FAKE.exe build.fsx Compile
# ********
clear
#IF EXIST ./buildframework/FAKE GOTO RunBuild
if [ ! -d ./buildframework/FAKE ]; then
	mono ./src/.nuget/NuGet.exe install FAKE -OutputDirectory ./buildframework/ -ExcludeVersion -Pre
fi

mono ./buildframework/FAKE/tools/FAKE.exe build.fsx
