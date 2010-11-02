function PrintHelp()
{
	"
	This script is used to strong name an assembly. You need to configure POSH with VsVars32 first.
		
	Usage: .\sign.ps1 <assemblyName> <outputName>
		
	Example: .\sign.ps1 gherkin-2.2.4.unsigned.orig gherkin-2.2.4
	"
}

$arguments = $args

if($arguments.Length -ne 2)
	{PrintHelp}

$assemblyName = $arguments[0]
$outputName = $arguments[1]

$ildasmArguments = @(
	[string]::Concat($assemblyName,".dll"),
	[string]::Concat('/out:',$assemblyName,".il"))
	
$ilasmArguments = @(
	[string]::Concat($assemblyName,".il"), 
	[string]::Concat('/res:',$assemblyName,".res"),
	'/dll', 
	"/key:..\..\src\NBehave.snk", 
	[string]::Concat('/out:',$outputName,".dll"))


ildasm $ildasmArguments
C:\Windows\Microsoft.NET\framework\v2.0.50727\ilasm $ilasmArguments

rm *.il
rm *.res
rm *.exports
rm *.properties
rm *.txt
rm *.xml
rm *.MF
