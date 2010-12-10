# C:\Users\father\Desktop\Projects\nbehave\lib\Gherkin\gherkin.dll C:\Users\father\Desktop\Projects\nbehave\lib\ikvm\IKVM.OpenJDK.Core.dll C:\Users\father\Desktop\Projects\nbehave\lib\Gherkin\json-simple.dll" exited with code 1.

$arguments = @(
	#'/internalize',
	'/Closed',
	'/copyattrs',
	'/allowMultiple'
	'/allowDup',
	'/lib:..\ikvm',
	'/keyfile:..\..\src\nbehave.snk',
	'/out:gherkin-2.2.7.merged.dll',
	'gherkin.dll',
	'json-simple.dll',
	'..\ikvm\ikvm.openjdk.core.dll',
	'..\ikvm\ikvm.runtime.dll'
)

..\..\tools\ilmerge\ilmerge.exe $arguments