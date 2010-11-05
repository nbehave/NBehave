
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

function xmlPeek([string]$file, [string]$xpath, [hashtable]$namespaces) { 
    [xml] $fileXml = Get-Content $file 
	$xmlNameTable = new-object System.Xml.NameTable
	$xmlNameSpace = new-object System.Xml.XmlNamespaceManager($xmlNameTable)

	foreach($key in $namespaces.keys)
	{
		$xmlNameSpace.AddNamespace($key, $namespaces.$key);
	}
	
    $node = $fileXml.SelectSingleNode($xpath, $xmlNameSpace) 
	return $node.InnerText
}

function xmlList([string]$file, [string]$xpath, [hashtable]$namespaces) { 
    [xml] $fileXml = Get-Content $file 
	$xmlNameTable = new-object System.Xml.NameTable
	$xmlNameSpace = new-object System.Xml.XmlNamespaceManager($xmlNameTable)

	foreach($key in $namespaces.keys)
	{
		$xmlNameSpace.AddNamespace($key, $namespaces.$key);
	}
	$nodes = @()
    $node = $fileXml.SelectNodes($xpath, $xmlNameSpace) 
	$node | ForEach-Object { $nodes += @($_.Value)}
	
	return $nodes
}

function zip($path, $files)
{
	if (-not $path.EndsWith('.zip')) {$path += '.zip'} 

	if (-not (test-path $path)) { 
	  set-content $path ("PK" + [char]5 + [char]6 + ("$([char]0)" * 18)) 
	} 

	$ZipFile = (new-object -com shell.application).NameSpace($path) 
	$ZipFile.CopyHere($files)
}

function ilmerge($key, $directory, $name, $assemblies, $extension)
{	
	new-item -path $directory -name "temp_merge" -type directory -ErrorAction SilentlyContinue
	
	if($framework -eq "4.0")
	{
		Exec { ..\tools\ilmerge\ilmerge.exe /keyfile:$key /out:"$directory\temp_merge\$name.$extension" "$directory\$name.$extension" $assemblies /targetplatform:"v4,$env:ProgramFiles\Reference Assemblies\Microsoft\Framework\.NETFramework\v4.0" }
	}
	else
	{
		Exec { ..\tools\ilmerge\ilmerge.exe /keyfile:$key /out:"$directory\temp_merge\$name.$extension" "$directory\$name.$extension" $assemblies }
	}
	
	Get-ChildItem "$directory\temp_merge\**" -Include *.dll, *.pdb | Copy-Item -Destination $directory
	
	Remove-Item "$directory\temp_merge" -Recurse -ErrorAction SilentlyContinue
}