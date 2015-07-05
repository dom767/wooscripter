; example2.nsi
;
; This script is based on example1.nsi, but it remember the directory, 
; has uninstall support and (optionally) installs start menu shortcuts.
;
; It will install example2.nsi into a directory that the user selects,

;--------------------------------

; The name of the installer
Name "WooScripter"

; The file to write
OutFile "WooScripter.exe"

; The default installation directory
InstallDir $PROGRAMFILES\WooScripter

; Registry key to check for directory (so if you install again, it will 
; overwrite the old one automatically)
InstallDirRegKey HKLM "Software\WooScripter" "Install_Dir"

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
Section "WooScripter (required)"

  SectionIn RO
  
  ; Set output path to the installation directory.
  SetOutPath $INSTDIR
  
  ; Put file there
  File /oname=coretracer.dll "..\bin\coretracer.dll"
  File /oname=wooscripter.exe "..\bin\Release\wooscripter.exe"

  SetOutPath $DOCUMENTS

  CreateDirectory $DOCUMENTS\WooScripter\Scripts\Background
  CreateDirectory $DOCUMENTS\WooScripter\Scripts\Lighting
  CreateDirectory $DOCUMENTS\WooScripter\Scripts\Scene
  CreateDirectory $DOCUMENTS\WooScripter\Scripts\Scene\Tutorial
  CreateDirectory $DOCUMENTS\WooScripter\Scripts\Scene\Nanoblocks
  CreateDirectory $DOCUMENTS\WooScripter\Scripts\Scene\FunObjects
  CreateDirectory $DOCUMENTS\WooScripter\Scripts\Scene\Fractal

  ; examples
  File /oname=WooScripter\Scripts\Background\scratch.woo "..\Scripts\Background\boxes.woo"
  File /oname=WooScripter\Scripts\Background\plain_white_circle.woo "..\Scripts\Background\plain_white_circle.woo"
  File /oname=WooScripter\Scripts\Background\boxes.woo "..\Scripts\Background\boxes.woo"

  File /oname=WooScripter\Scripts\Lighting\scratch.woo "..\Scripts\Lighting\daylight.woo"
  File /oname=WooScripter\Scripts\Lighting\examples.woo "..\Scripts\Lighting\examples.woo"
  File /oname=WooScripter\Scripts\Lighting\bigsphere.woo "..\Scripts\Lighting\bigsphere.woo"
  File /oname=WooScripter\Scripts\Lighting\daylight.woo "..\Scripts\Lighting\daylight.woo"
  File /oname=WooScripter\Scripts\Lighting\plaindirectional.woo "..\Scripts\Lighting\plaindirectional.woo"
  File /oname=WooScripter\Scripts\Lighting\ringoflight.woo "..\Scripts\Lighting\ringoflight.woo"
  File /oname=WooScripter\Scripts\Lighting\sunset.woo "..\Scripts\Lighting\sunset.woo"

  File /oname=WooScripter\Scripts\Scene\scratch.woo "..\Scripts\Scene\examples.woo"
  File /oname=WooScripter\Scripts\Scene\examples.woo "..\Scripts\Scene\examples.woo"
  File /oname=WooScripter\Scripts\Scene\simplebox.woo "..\Scripts\Scene\simplebox.woo"
  File /oname=WooScripter\Scripts\Scene\hairyball.woo "..\Scripts\Scene\hairyball.woo"
  File /oname=WooScripter\Scripts\Scene\spiral.woo "..\Scripts\Scene\spiral.woo"
  File /oname=WooScripter\Scripts\Scene\FunObjects\fragmentedsphere.woo "..\Scripts\Scene\FunObjects\fragmentedsphere.woo"
  File /oname=WooScripter\Scripts\Scene\Tutorial\tutorial_pos.woo "..\Scripts\Scene\Tutorial\tutorial_pos.woo"
  File /oname=WooScripter\Scripts\Scene\Tutorial\tutorial_rotate.woo "..\Scripts\Scene\Tutorial\tutorial_rotate.woo"
  File /oname=WooScripter\Scripts\Scene\Tutorial\tutorial_scale.woo "..\Scripts\Scene\Tutorial\tutorial_scale.woo"
  File /oname=WooScripter\Scripts\Scene\Tutorial\tutorial_smoothrotate.woo "..\Scripts\Scene\Tutorial\tutorial_smoothrotate.woo"
  File /oname=WooScripter\Scripts\Scene\Nanoblocks\nanoblock_babyduck.woo "..\Scripts\Scene\Nanoblocks\nanoblock_babyduck.woo"
  File /oname=WooScripter\Scripts\Scene\Nanoblocks\nanoblock_empire.woo "..\Scripts\Scene\Nanoblocks\nanoblock_empire.woo"
  File /oname=WooScripter\Scripts\Scene\Nanoblocks\nanoblock_stmichel.woo "..\Scripts\Scene\Nanoblocks\nanoblock_stmichel.woo"
  File /oname=WooScripter\Scripts\Scene\Fractal\kifs_hybrid.woo "..\Scripts\Scene\Fractal\kifs_hybrid.woo"
  File /oname=WooScripter\Scripts\Scene\Fractal\kifs_hybrid_optimised.woo "..\Scripts\Scene\Fractal\kifs_hybrid_optimised.woo"
  File /oname=WooScripter\Scripts\Scene\distance.woo "..\Scripts\Scene\distance.woo"
  File /oname=WooScripter\Scripts\Scene\menger.woo "..\Scripts\Scene\menger.woo"

  ; Write the installation path into the registry
  WriteRegStr HKLM SOFTWARE\WooScripter "Install_Dir" "$INSTDIR"
  
  ; Write the uninstall keys for Windows
  WriteRegStr HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\WooScripter" "DisplayName" "WooScripter"
  WriteRegStr HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\WooScripter" "UninstallString" '"$INSTDIR\uninstall.exe"'
  WriteRegDWORD HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\WooScripter" "NoModify" 1
  WriteRegDWORD HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\WooScripter" "NoRepair" 1
  WriteUninstaller "uninstall.exe"
  
SectionEnd

; Optional section (can be disabled by the user)
Section "Start Menu Shortcuts"

  CreateDirectory "$SMPROGRAMS\WooScripter"
  CreateShortCut "$SMPROGRAMS\WooScripter\Uninstall.lnk" "$INSTDIR\uninstall.exe" "" "$INSTDIR\uninstall.exe" 0
  CreateShortCut "$SMPROGRAMS\WooScripter\WooScripter.lnk" "$INSTDIR\WooScripter.exe" "" "$INSTDIR\WooScripter.exe" 0
  
SectionEnd

;--------------------------------

; Uninstaller

Section "Uninstall"
  
  ; Remove registry keys
  DeleteRegKey HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\WooScripter"
  DeleteRegKey HKLM SOFTWARE\WooScripter

  ; Remove files and uninstaller
  Delete $INSTDIR\coretracer.dll
  Delete $INSTDIR\uninstall.exe

  ; Remove Scripts
;  Delete $DOCUMENTS\WooScripter\Scripts

  ; Remove shortcuts, if any
  Delete "$SMPROGRAMS\WooScripter\*.*"

  ; Remove directories used
  RMDir "$SMPROGRAMS\WooScripter"
  RMDir "$INSTDIR"

SectionEnd
