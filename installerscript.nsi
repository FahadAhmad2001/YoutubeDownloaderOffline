!include "MUI.nsh"
!define MUI_ABORTWARNING
!insertmacro MUI_PAGE_LICENSE "LSYT_license.txt"
!insertmacro MUI_PAGE_LICENSE "ffmpeg_license.txt"
!insertmacro MUI_PAGE_COMPONENTS
!define MUI_TEXT_LICENSE_TITLE "License Agreement"
!insertmacro MUI_LANGUAGE "English"
Name "LightSpeed YouTube Downloader"
Caption ""
Icon "UpdatedUIApp\Resources\icon.ico"
OutFile "LightSpeed YouTube Downloader.exe"
SetDateSave on
SetDatablockOptimize on
CRCCheck on
SilentInstall normal
InstallDir "$PROGRAMFILES\LightSpeed\YouTube Downloader"
RequestExecutionLevel admin
ManifestSupportedOS all
Page directory
Page instfiles
UninstPage uninstConfirm
UninstPage instfiles
AutoCloseWindow false
ShowInstDetails show
Section ""
SetOutPath $INSTDIR
SetOverwrite On
File /nonfatal /a /r "UpdatedUIApp\bin\Debug\"
SectionEnd
Section "Install Backend Server"
SetOutPath "$INSTDIR\Server"
SetOverwrite On
File /nonfatal /a /r "YTDLBackendServer\bin\Debug\"
SectionEnd
Section "Install Source Code"
SetOutPath "$INSTDIR\Source"
SetOverwrite On
File /nonfatal /a /r "SourceCode\"
SectionEnd
Section "Create Shortcut on Desktop"
SetOutPath $INSTDIR
CreateShortCut "$DESKTOP\LightSpeed YouTube Downloader.lnk" "$INSTDIR\\UpdatedUIApp.exe" "" "$INSTDIR\\if_youtube_294703_lNF_icon.ico" 0 SW_SHOWNORMAL ALT|CONTROL|SHIFT|F4 "LightSpeed YouTube Downloader"
SectionEnd