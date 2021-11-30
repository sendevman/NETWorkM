﻿; Script generated by the Inno Setup Script Wizard.
; SEE THE DOCUMENTATION FOR DETAILS ON CREATING INNO SETUP SCRIPT FILES!

#define MyAppName "NETworkManager"
#define MyAppVersion "2021.11.30.0"
#define MyAppPublisher "BornToBeRoot"
#define MyAppURL "https://github.com/BornToBeRoot/NETworkManager/"
#define MyAppExeName "NETworkManager.exe"
#define MyAppCopyright "Copyright (C) 2016-2021 BornToBeRoot"

[Setup]
; NOTE: The value of AppId uniquely identifies this application. Do not use the same AppId value in installers for other applications.
; (To generate a new GUID, click Tools | Generate GUID inside the IDE.)
AppId={{8028080F-B785-4A74-A243-3D63467880A6}
AppName={#MyAppName}
AppVersion={#MyAppVersion}
VersionInfoVersion={#MyAppVersion}
AppCopyright={#MyAppCopyright}
;AppVerName={#MyAppName} {#MyAppVersion}
AppPublisher={#MyAppPublisher}
AppPublisherURL={#MyAppURL}
AppSupportURL={#MyAppURL}
AppUpdatesURL={#MyAppURL}
UninstallDisplayName={#MyAppName}
UninstallDisplayIcon={app}\{#MyAppExeName}
DefaultDirName={autopf}\{#MyAppName}
DisableProgramGroupPage=yes
LicenseFile=Build\NETworkManager\Licenses\NETworkManager.txt
; Uncomment the following line to run in non administrative install mode (install for current user only.)
;PrivilegesRequired=lowest
OutputDir=Build
OutputBaseFilename=NETworkManager_{#MyAppVersion}_Setup
Compression=lzma
SolidCompression=yes
WizardStyle=modern
ArchitecturesAllowed=x64
ArchitecturesInstallIn64BitMode=x64
WizardSmallImageFile=Images\NETworkManager_InnoSetup.bmp

[Languages]
Name: "english";            MessagesFile: "compiler:Default.isl"
Name: "brasilian";          MessagesFile: "compiler:Languages\BrazilianPortuguese.isl"
Name: "chinesesimplified";  MessagesFile: "compiler:Languages\ChineseSimplified.isl"
Name: "chinesetraditional"; MessagesFile: "compiler:Languages\ChineseTraditional.isl"
Name: "czech";              MessagesFile: "compiler:Languages\Czech.isl"
Name: "dutch";              MessagesFile: "compiler:Languages\Dutch.isl"
Name: "french";             MessagesFile: "compiler:Languages\French.isl"
Name: "german";             MessagesFile: "compiler:Languages\German.isl"
Name: "italian";            MessagesFile: "compiler:Languages\Italian.isl"
Name: "japanese";           MessagesFile: "compiler:Languages\Japanese.isl"
Name: "polish";             MessagesFile: "compiler:Languages\Polish.isl"
Name: "russian";            MessagesFile: "compiler:Languages\Russian.isl"
Name: "spanish";            MessagesFile: "compiler:Languages\Spanish.isl"
Name: "slovenian";          MessagesFile: "compiler:Languages\Slovenian.isl"
Name: "turkish";            MessagesFile: "compiler:Languages\Turkish.isl"

[Tasks]
Name: "desktopicon"; Description: "{cm:CreateDesktopIcon}"; GroupDescription: "{cm:AdditionalIcons}"

[Files]
Source: "Build\NETworkManager\NETworkManager.exe"; DestDir: "{app}"; Flags: ignoreversion
Source: "Build\NETworkManager\*"; DestDir: "{app}"; Flags: ignoreversion recursesubdirs createallsubdirs
; NOTE: Don't use "Flags: ignoreversion" on any shared system files

[Icons]
Name: "{autoprograms}\{#MyAppName}"; Filename: "{app}\{#MyAppExeName}"
Name: "{autodesktop}\{#MyAppName}"; Filename: "{app}\{#MyAppExeName}"; Tasks: desktopicon

[Run]
Filename: "{app}\{#MyAppExeName}"; Description: "{cm:LaunchProgram,{#StringChange(MyAppName, '&', '&&')}}"; Flags: nowait postinstall skipifsilent

