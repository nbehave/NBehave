copy gherkin-2.4.5.dll gherkin-2.4.5.unsigned.orig.dll
ildasm gherkin-2.4.5.unsigned.orig.dll /out:gherkin-2.4.5.il
"C:\Windows\Microsoft.NET\framework\v2.0.50727\ilasm" gherkin-2.4.5.il /res:gherkin-2.4.5.res /dll /key:..\..\src\NBehave.snk /out:gherkin-2.4.5.signed.dll

del *.il
del *.res
del *.exports
del *.properties
del *.txt
del *.xml
del *.MF
del gherkin-2.4.5.unsigned.orig.dll
