!define VERSION "0.4"
!define FILES "..\..\Build\dist"
; The name of the installer
Name "NBehave"


; The files to write
Outfile "..\..\Build\NBehave ${VERSION}.exe"

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

; The stuff to install
Section "Framework Files (required)" ;No components page, name is not important
	
	SectionIn RO
	
	; Set output path to the installation directory.
	SetOutPath $INSTDIR
  
  ; Put file there
	File "${FILES}\NBehave-Console.exe"	
	File "${FILES}\NBehave.NAnt.dll"
	File "${FILES}\NBehave.Narrator.Framework.dll"
	File "${FILES}\NBehave.Spec.Framework.dll"
	File "${FILES}\NBehave.Spec.MbUnit.dll"
	File "${FILES}\NBehave.Spec.MSTest.dll"
	File "${FILES}\NBehave.Spec.NUnit.dll"
	File "${FILES}\NBehave.Spec.Xunit.dll"
	File "${FILES}\NBehave.TestDriven.Plugin.dll"

	; Write the installation path into the registry
	WriteRegStr HKLM SOFTWARE\NBehave\${VERSION} "Install_Dir" "$INSTDIR"
  
	; Write the uninstall keys for Windows
	WriteRegStr HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\NBehave ${VERSION}" "DisplayName" "NBehave ${VERSION}"
	WriteRegStr HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\NBehave ${VERSION}" "UninstallString" '"$INSTDIR\uninstall.exe"'
	WriteRegDWORD HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\NBehave ${VERSION}" "NoModify" 1
	WriteRegDWORD HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\NBehave ${VERSION}" "NoRepair" 1
	WriteUninstaller "uninstall.exe"

SectionEnd ; end the section

Section "Testdriven.NET plugin"
	File "${FILES}\NBehave.TestDriven.Plugin.dll"
	;File "${FILES}\TestDriven.Framework.dll"
	
	WriteRegStr HKLM "SOFTWARE\MutantDesign\TestDriven.NET\TestRunners\NBehave ${VERSION}" "AssemblyPath" "$INSTDIR\NBehave.TestDriven.Plugin.dll"
	WriteRegStr HKLM "SOFTWARE\MutantDesign\TestDriven.NET\TestRunners\NBehave ${VERSION}" "Application" "$INSTDIR\NBehave.TestDriven.Plugin.dll"
	WriteRegStr HKLM "SOFTWARE\MutantDesign\TestDriven.NET\TestRunners\NBehave ${VERSION}" "TypeName" "NBehave.TestDriven.Plugin.NBehaveStoryRunner"
SectionEnd
; Uninstaller

Section "Uninstall"
  
  ; Remove registry keys
  DeleteRegKey HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\NBehave ${VERSION}"
  DeleteRegKey HKLM SOFTWARE\NBehave\${VERSION}
  DeleteRegKey HKLM "SOFTWARE\MutantDesign\TestDriven.NET\TestRunners\NBehave ${VERSION}"
  ; Remove files and uninstaller
  Delete $INSTDIR\*.DLL
  Delete $INSTDIR\NBehave-Console.exe
  Delete $INSTDIR\uninstall.exe

  ; Remove directories used
  RMDir "$INSTDIR"

SectionEnd
