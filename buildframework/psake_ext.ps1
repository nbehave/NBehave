function SwitchDotNetFrameworkVersion
{
param(
	[string]$version
)
	$framework = $version
	Configure-BuildEnvironment
}

function xmlPoke([string]$file, [string]$xpath, $value, [hashtable]$namespaces) { 
    [xml] $fileXml = Get-Content $file 
	$xmlNameTable = new-object System.Xml.NameTable
	$xmlNameSpace = new-object System.Xml.XmlNamespaceManager($xmlNameTable)

	foreach($key in $namespaces.keys)
	{
		$xmlNameSpace.AddNamespace($key, $namespaces.$key);
	}
	
    $node = $fileXml.SelectSingleNode($xpath, $xmlNameSpace) 
    if ($node) { 
        $node.InnerText = $value 

        $fileXml.Save($file)  
    } 
}