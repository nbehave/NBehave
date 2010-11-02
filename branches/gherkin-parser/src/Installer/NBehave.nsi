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
	File "${FILES}\v3.5\**"

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
	File "${FILES}\v4.0\**"

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

SectionEnd

Section "Visual Studio 2010 Plugin"

	SetOutPath "$PROGRAMFILES\Microsoft Visual Studio 10.0\Common7\IDE\Extensions\NBehave"
	
	File "${PLUGIN}\**"

SectionEnd

; Uninstaller
Section "Uninstall"
  
  ; Remove registry keys
  DeleteRegKey HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\NBehave ${VERSION}"
  DeleteRegKey HKLM SOFTWARE\NBehave\${VERSION}
  
  ; Remove v3.5 files and uninstaller
  Delete $INSTDIR\v3.5\**
  
  ; Remove v3.5 files and uninstaller
  Delete $INSTDIR\v4.0\**

  ; Remove VS2010 Plugin
  Delete "$PROGRAMFILES\Microsoft Visual Studio 10.0\Common7\IDE\Extensions\NBehave\**"
  
  ; Remove uninstaller
  Delete $INSTDIR\uninstall.exe
  
  
  ; Remove directories used
  RMDir "$INSTDIR"
  RMDir "$PROGRAMFILES\Microsoft Visual Studio 10.0\Common7\IDE\Extensions\NBehave"
  
SectionEnd
