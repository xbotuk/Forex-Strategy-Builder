#define MyAppName      "Forex Strategy Builder"
#define MyAppVersion   "3.1.0.0"
#define MyAppVerText   "v3.1"
#define MyOutputPath   SourcePath
#define MyCertPass     ReadIni(SourcePath + "\Install.ini", "Release", "CertificatePassoword")

[Setup]
AppName            = {#MyAppName}
AppVersion         = {#MyAppVersion}
VersionInfoVersion = {#MyAppVersion}
AppVerName         = {#MyAppName} {#MyAppVerText}

ArchitecturesInstallIn64BitMode = x64 ia64
AppPublisher       = Forex Software Ltd.
AppPublisherURL    = http://forexsb.com/
AppCopyright       = Copyright © 2006-2014 Miroslav Popov
AppComments        = Freeware forex strategy back tester, generator and optimizer.
DefaultDirName     = {pf}\{#MyAppName}
DefaultGroupName   = {#MyAppName}
SourceDir          = ..\Output\Release
LicenseFile        = License.rtf
OutputBaseFilename = ForexStrategyBuilder
OutputDir          = {#MyOutputPath}
SignTool           = signtool sign /f "{#MyOutputPath}\ForexSoftwareCertificate.pfx" /t "http://timestamp.digicert.com" /p "{#MyCertPass}" /d $q{#MyAppName}$q $f

[Components]
Name: "main";    Description: "Main files.";        Types: full compact custom; Flags: fixed;
Name: "data";    Description: "Data files. (Disable to prevent overwriting user data.)"; Types: full; Flags: disablenouninstallwarning;
Name: "custind"; Description: "Custom indicators."; Types: full; Flags: disablenouninstallwarning;
Name: "strat";   Description: "Demo strategies.";   Types: full; Flags: disablenouninstallwarning;
Name: "lang";    Description: "Language files.";    Types: full; Flags: disablenouninstallwarning;
Name: "color";   Description: "Color schemes.";     Types: full; Flags: disablenouninstallwarning;

[InstallDelete]
Type: files; Name: "{app}\User Files\System\lcconfig.xml";
Type: files; Name: "{app}\User Files\System\fsb-update.xml";

[Files]
Source: Forex Strategy Builder.exe;        DestDir: "{app}";                                Components: main;     Flags: replacesameversion;
Source: Forex Strategy Builder.CustomAnalytics.dll; DestDir: "{app}";                       Components: main;     Flags: replacesameversion;
Source: FSB_Launcher.exe;                  DestDir: "{app}";                                Components: main;     Flags: replacesameversion;
Source: FSB_Launcher.xml;                  DestDir: "{app}";                                Components: main;
Source: ReadMe.html;                       DestDir: "{app}";                                Components: main;
Source: License.rtf;                       DestDir: "{app}";                                Components: main;
Source: User Files\Indicators\*;           DestDir: "{app}\User Files\Indicators";          Components: custind;
Source: User Files\Data\*;                 DestDir: "{app}\User Files\Data";                Components: data;
Source: User Files\Strategies\*;           DestDir: "{app}\User Files\Strategies";          Components: strat;
Source: User Files\System\Colors\*;        DestDir: "{app}\User Files\System\Colors";       Components: color;
Source: User Files\System\Languages\*;     DestDir: "{app}\User Files\System\Languages";    Components: lang;
Source: User Files\System\StartingTips\*;  DestDir: "{app}\User Files\System\StartingTips"; Components: lang;

[Dirs]
Name: "{app}\User Files\Libraries";
Name: "{app}\User Files"; Permissions: users-modify;

[Icons]
Name: "{commondesktop}\Forex Strategy Builder"; Filename: "{app}\FSB_Launcher.exe"; WorkingDir: "{app}";
Name: "{group}\Forex Strategy Builder";         Filename: "{app}\FSB_Launcher.exe"; WorkingDir: "{app}";
Name: "{group}\User Files";                     Filename: "{app}\User Files";
Name: "{group}\Uninstall";                      Filename: "{uninstallexe}";

[Run]
Filename: "{app}\FSB_Launcher.exe"; Description: "Launch the application"; Flags: postinstall skipifsilent nowait;
Filename: "{app}\ReadMe.html";      Description: "View the ReadMe file";   Flags: postinstall skipifsilent shellexec unchecked;

[Code]
var
  OptionsPage: TInputOptionWizardPage;
  UsagePage:   TInputOptionWizardPage;
procedure InitializeWizard;
begin
  OptionsPage := CreateInputOptionPage(wpSelectProgramGroup,
    'Installation Options', 'User controlled installation options.',
    'Choose whether to preserve or overwrite program settings from a previous installation of Forex Strategy Builder.',
    False, False);
  OptionsPage.Add('Preserve program settings.');
  OptionsPage.Values[0] := True;
  OptionsPage.Add('Preserve instruments settings.');
  OptionsPage.Values[1] := True;

  UsagePage := CreateInputOptionPage(OptionsPage.ID,
    'Usage Statistics', 'Help us improve Forex Strategy Builder.',
    'We would greatly appreciate if you allow us to collect anonymous usage statistics to help us provide a better quality product. The information collected is not used to identify or contact you. You can interrupt collecting usage statistics at any time from Help - Send anonymous usage statistics menu option in the program.',
    True, False);
  UsagePage.Add('Allow anonymous usage statistics (recomended).');
  UsagePage.Add('Do not send anonymous usage statistics.');
  UsagePage.SelectedValueIndex := 0;
end;

function InitializeSetup(): Boolean;
var
  ErrorCode : Integer;
  Result1   : Boolean;
begin
	Result := RegKeyExists(HKLM,'SOFTWARE\Microsoft\.NETFramework\policy\v2.0');
	if Result = false then
	begin
    Result1 := MsgBox('This setup requires the .NET Framework v2.0. Please download and install the .NET Framework v.2 and run this setup again. Do you want to download the framework now?', mbConfirmation, MB_YESNO) = idYes;
	  if Result1 = true then
	  begin
		  ShellExec('open', 'http://download.microsoft.com/download/5/6/7/567758a3-759e-473e-bf8f-52154438565a/dotnetfx.exe', '','', SW_SHOWNORMAL, ewNoWait, ErrorCode);
		end;
  end;
end;

function NextButtonClick(CurPageID: Integer): Boolean;
begin
  if CurPageID = OptionsPage.ID then
  begin
    if OptionsPage.Values[0] = false then
    begin
      DeleteFile(ExpandConstant('{app}\User Files\System\config.xml'));
    end;
    if OptionsPage.Values[1] = false then
    begin
      DeleteFile(ExpandConstant('{app}\User Files\System\instruments.xml'));
    end;
  end;

  if CurPageID = UsagePage.ID then
  begin
    RegWriteStringValue(HKCU, 'Software\Forex Software\Forex Strategy Builder', 'UsageStats', IntToStr(UsagePage.SelectedValueIndex));
  end;
  Result := True;
end;

procedure CurStepChanged(CurStep: TSetupStep);
begin
  if CurStep = ssInstall then
  begin
    // Delete legacy content
    DeleteFile(ExpandConstant('{app}\FSB Starter.exe'));
    DeleteFile(ExpandConstant('{app}\SplashConfig.cfg'));
    DeleteFile(ExpandConstant('{app}\ConnectWait.ico'));
    DelTree(ExpandConstant('{app}\SplashScreen'), True, True, True);

    // Move files in new location
    CreateDir(ExpandConstant('{app}\User Files'));
    RenameFile(ExpandConstant('{app}\Custom Indicators'), ExpandConstant('{app}\User Files\Indicators'));
    RenameFile(ExpandConstant('{app}\Data'),              ExpandConstant('{app}\User Files\Data'));
    RenameFile(ExpandConstant('{app}\Strategies'),        ExpandConstant('{app}\User Files\Strategies'));
    RenameFile(ExpandConstant('{app}\System'),            ExpandConstant('{app}\User Files\System'));
    DelTree(ExpandConstant('{app}\Custom Indicators'), True, True, True);
    DelTree(ExpandConstant('{app}\Data'),              True, True, True);
    DelTree(ExpandConstant('{app}\Strategies'),        True, True, True);
    DelTree(ExpandConstant('{app}\System'),            True, True, True);
  end;
end;


