!ifndef VERSION
	!define VERSION "0.0.0"
!endif
!define FILES "..\..\Build\dist"
!define PLUGIN "..\..\Build\plugin"
!define EXAMPLEFILES "..\..\Build"
; The name of the installer
Name "NBehave"

; The files to write
Outfile "..\..\Build\NBehave_${VERSION}.exe"

; The default installation directory
InstallDir $PROGRAMFILES\NBehave\${VERSION}

; Registry key to check for directory (so if you install again, it will 
; overwrite the old one automatically)
InstallDirRegKey HKLM "Software\NBehave\${VERSION}" "Install_Dir"

; Request application privileges for Windows Vista
RequestExecutionLevel admin
;--------------------------------
; Pages

Page components
Page directory
Page instfiles

UninstPage uninstConfirm
UninstPage instfiles

;--------------------------------
; .Net 3.5 version
Section ".Net 3.5 files" ;No components page, name is not important
	
	;SectionIn RO
	
	; Set output path to the installation directory.
	SetOutPath $INSTDIR\v3.5
  
	; Put file there
	File "${FILES}\v3.5\NBehave-Console.exe"	
	File "${FILES}\v3.5\NBehave.NAnt.dll"
	File "${FILES}\v3.5\NBehave.MSBuild.dll"
	File "${FILES}\v3.5\NBehave.Narrator.Framework.dll"
	File "${FILES}\v3.5\NBehave.Narrator.Framework.dll.tdnet"
	File "${FILES}\v3.5\NBehave.Spec.Framework.dll"
	File "${FILES}\v3.5\NBehave.Spec.MbUnit.dll"
	File "${FILES}\v3.5\NBehave.Spec.MSTest9.dll"
	File "${FILES}\v3.5\NBehave.Spec.NUnit.dll"
	File "${FILES}\v3.5\NBehave.Spec.Xunit.dll"
	File "${FILES}\v3.5\NBehave.TestDriven.Plugin.dll"
	File "${FILES}\v3.5\Gallio.dll"
	File "${FILES}\v3.5\MbUnit.dll"
	File "${FILES}\v3.5\nunit.framework.dll"
	File "${FILES}\v3.5\Rhino.Mocks.dll"
	File "${FILES}\v3.5\xunit.dll"
	File "${FILES}\v3.5\languages.yml"

	; Write the installation path into the registry
	WriteRegStr HKLM SOFTWARE\NBehave\${VERSION} "Install_Dir" "$INSTDIR"
  
	; Write the uninstall keys for Windows
	WriteRegStr HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\NBehave ${VERSION}" "DisplayName" "NBehave ${VERSION}"
	WriteRegStr HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\NBehave ${VERSION}" "UninstallString" '"$INSTDIR\uninstall.exe"'
	WriteRegDWORD HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\NBehave ${VERSION}" "NoModify" 1
	WriteRegDWORD HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\NBehave ${VERSION}" "NoRepair" 1
	WriteUninstaller "uninstall.exe"

SectionEnd

;--------------------------------
; .Net 4.0 version
Section ".Net 4.0 files" ;No components page, name is not important
	
	; Set output path to the installation directory.
	SetOutPath $INSTDIR\v4.0
  
	; Put file there
	File "${FILES}\v4.0\NBehave-Console.exe"	
	File "${FILES}\v4.0\NBehave.NAnt.dll"
	File "${FILES}\v4.0\NBehave.MSBuild.dll"
	File "${FILES}\v4.0\NBehave.Narrator.Framework.dll"
	File "${FILES}\v4.0\NBehave.Narrator.Framework.dll.tdnet"
	File "${FILES}\v4.0\NBehave.Spec.Framework.dll"
	File "${FILES}\v4.0\NBehave.Spec.MbUnit.dll"
	File "${FILES}\v4.0\NBehave.Spec.MSTest10.dll"
	File "${FILES}\v4.0\NBehave.Spec.NUnit.dll"
	File "${FILES}\v4.0\NBehave.Spec.Xunit.dll"
	File "${FILES}\v4.0\NBehave.TestDriven.Plugin.dll"
	File "${FILES}\v4.0\Gallio.dll"
	File "${FILES}\v4.0\MbUnit.dll"
	File "${FILES}\v4.0\nunit.framework.dll"
	File "${FILES}\v4.0\Rhino.Mocks.dll"
	File "${FILES}\v4.0\xunit.dll"
	File "${FILES}\v4.0\languages.yml"

	; Write the installation path into the registry
	WriteRegStr HKLM SOFTWARE\NBehave\${VERSION} "Install_Dir" "$INSTDIR"
  
	; Write the uninstall keys for Windows
	WriteRegStr HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\NBehave ${VERSION}" "DisplayName" "NBehave ${VERSION}"
	WriteRegStr HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\NBehave ${VERSION}" "UninstallString" '"$INSTDIR\uninstall.exe"'
	WriteRegDWORD HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\NBehave ${VERSION}" "NoModify" 1
	WriteRegDWORD HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\NBehave ${VERSION}" "NoRepair" 1
	WriteUninstaller "uninstall.exe"

SectionEnd

Section "NBehave Example code"
	File "${EXAMPLEFILES}\NBehave.Examples.zip"
SectionEnd

Section "Visual Studio 2010 Plugin"

	SetOutPath "$PROGRAMFILES\Microsoft Visual Studio 10.0\Common7\IDE\Extensions\NBehave"
	
	File "${PLUGIN}\extension.vsixmanifest"
	File "${PLUGIN}\NBehave.VS2010.Plugin.dll"
	File "${PLUGIN}\NBehave.VS2010.Plugin.pkgdef"

SectionEnd

; Uninstaller
Section "Uninstall"
  
  ; Remove registry keys
  DeleteRegKey HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\NBehave ${VERSION}"
  DeleteRegKey HKLM SOFTWARE\NBehave\${VERSION}
  
  ; Remove v3.5 files and uninstaller
  Delete $INSTDIR\v3.5\*.dll
  Delete $INSTDIR\v3.5\*.tdnet
  Delete $INSTDIR\v3.5\*.zip
  Delete $INSTDIR\v3.5\*.yml
  Delete $INSTDIR\v3.5\NBehave-Console.exe
  
  ; Remove v3.5 files and uninstaller
  Delete $INSTDIR\v4.0\*.dll
  Delete $INSTDIR\v4.0\*.tdnet
  Delete $INSTDIR\v4.0\*.zip
  Delete $INSTDIR\v4.0\*.yml
  Delete $INSTDIR\v4.0\NBehave-Console.exe

  ; Remove VS2010 Plugin
  Delete "$PROGRAMFILES\Microsoft Visual Studio 10.0\Common7\IDE\Extensions\NBehave\extension.vsixmanifest"
  Delete "$PROGRAMFILES\Microsoft Visual Studio 10.0\Common7\IDE\Extensions\NBehave\NBehave.VS2010.Plugin.dll"
  Delete "$PROGRAMFILES\Microsoft Visual Studio 10.0\Common7\IDE\Extensions\NBehave\NBehave.VS2010.Plugin.pkgdef"
  
  ; Remove uninstaller
  Delete $INSTDIR\uninstall.exe
  
  
  ; Remove directories used
  RMDir "$INSTDIR"
  RMDir "$PROGRAMFILES\Microsoft Visual Studio 10.0\Common7\IDE\Extensions\NBehave"
  
SectionEnd
