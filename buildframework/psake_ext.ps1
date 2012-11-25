function nunitVersion() {
  return (get-item src/packages/nunit* -Exclude nunit.runners* | Sort-Object Name -descending | Select-Object -first 1).Name -replace "Nunit."
}

function mbUnitVersion() {
  return (get-item src/packages/mbunit* | Sort-Object Name -descending | Select-Object -first 1).Name -replace "mbunit."
}

function xunitVersion() {
  return (get-item src/packages/xunit* | Sort-Object Name -descending | Select-Object -first 1).Name -replace "xunit."
}

function ExtractBuildNumber {
  $foo = $buildNumber -match "\d+$"
  $num = $matches[0]
  return $num
}

function BuildNumber {
  $buildNum = ExtractBuildNumber
  if ($preReleaseTag -match "^(r|R)elease$")  { $buildNum = "$version.$buildNum" }
  else { $buildNum = "$version-$preReleaseTag$buildNum" }
  return $buildNum
}

function AssemblyInformationalVersion {
  $asmInfoVer = "-"
  $buildNum = ExtractBuildNumber
  if ($preReleaseTag -match "^(r|R)elease$")  { $asmInfoVer = $version }
  else { $asmInfoVer = "$version-$preReleaseTag$buildNum" }
  return $asmInfoVer
}

function AssemblyVersion {
  $buildNum = ExtractBuildNumber
  $asmVersion = "$version.$buildNum"
  return $asmVersion
}

function clearDir($dirToRemove) {
  Get-ChildItem $dirToRemove\* -Recurse | Select -ExpandProperty FullName | Sort {$_.Length} -Descending | ForEach-Object { Remove-Item $_ }
  Write-Host "Files removed."
  Get-ChildItem $dirToRemove -Recurse -Include * | Select -ExpandProperty FullName | Sort {$_.Length} -Descending | ForEach-Object { Remove-Item $_ -Recurse }
  Write-Host "Folders removed."
}

function xmlPoke([string]$file, [string]$xpath, $value, [hashtable]$namespaces) {
  [xml] $fileXml = Get-Content $file
  $xmlNameTable = new-object System.Xml.NameTable
  $xmlNameSpace = new-object System.Xml.XmlNamespaceManager($xmlNameTable)

  foreach($key in $namespaces.keys) {
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

  foreach($key in $namespaces.keys) {
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

function Run-ILMerge($directory, $outAssembly, $assemblies) {
  new-item -path $directory -name "temp_merge" -type directory -ErrorAction SilentlyContinue

  $outFile = "$directory\temp_merge\$outAssembly"
  Write-Host "$outFile"

  if($framework -eq "4.0") {
    Exec { .\tools\ilmerge\ilmerge.exe /out:$outFile $assemblies /targetplatform:"v4,$env:ProgramFiles\Reference Assemblies\Microsoft\Framework\.NETFramework\v4.0" }
  }
  else {
    Exec { .\tools\ilmerge\ilmerge.exe /out:$outFile $assemblies /targetplatform:"v2,$env:ProgramFiles\Reference Assemblies\Microsoft\Framework\.NETFramework\v3.5" }
  }

  Get-ChildItem "$directory\temp_merge\**" -Include *.dll, *.pdb | Copy-Item -Destination $directory
  Remove-Item "$directory\temp_merge" -Recurse -ErrorAction SilentlyContinue
}