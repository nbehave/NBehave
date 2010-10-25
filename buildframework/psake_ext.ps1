
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

function zip
{
	$path = $args[0]
	$files = $input
  
	if (-not $path.EndsWith('.zip')) {$path += '.zip'} 

	if (-not (test-path $path)) { 
	  set-content $path ("PK" + [char]5 + [char]6 + ("$([char]0)" * 18)) 
	} 

	$ZipFile = (new-object -com shell.application).NameSpace($path) 
	$files | foreach {$zipfile.CopyHere($_.fullname)} 
}

function ilmerge($key, $directory, $name, $assemblies, $extension)
{	
	Move-Item "$directory\$name.$extension" "$directory\$name.$temp.$extension"

	if($framework -eq "4.0")
	{
		Exec { tools\ilmerge\ilmerge.exe /ndebug /keyfile:$key /out:"$directory\$name.$extension" "$directory\$name.$temp.$extension" $assemblies /targetplatform:"v4,$env:ProgramFiles\Reference Assemblies\Microsoft\Framework\.NETFramework\v4.0" }
	}
	else
	{
		Exec { tools\ilmerge\ilmerge.exe /ndebug /keyfile:$key /out:"$directory\$name.$extension" "$directory\$name.$temp.$extension" $assemblies }
	}
	
	Remove-Item "$directory\$name.$temp.$extension" -ErrorAction SilentlyContinue
}