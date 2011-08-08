$arguments = @(
	#'/internalize',
	'/closed',
	'/copyattrs',
	'/allowMultiple'
	'/allowDup',
	#'/lib:.\ikvm',
	'/keyfile:..\..\src\nbehave.snk',
	'/out:gherkin.dll',
	'gherkin-2.4.5.signed.dll',
	#'ikvm.openjdk.core.dll',
	#'ikvm.runtime.dll',
	'net.iharder-base64.dll',
	'com.googlecode.json-simple-json-simple.dll'
)

..\..\tools\ilmerge\ilmerge.exe $arguments