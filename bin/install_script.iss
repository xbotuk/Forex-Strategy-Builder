[Setup]
AppName            = Forex Strategy Builder
AppVersion         = 2.70.0.0
VersionInfoVersion = 2.70.0.0
AppVerName         = Forex Strategy Builder v2.70

ArchitecturesInstallIn64BitMode = x64 ia64
AppPublisher       = Forex Software Ltd.
AppPublisherURL    = http://forexsb.com/
AppCopyright       = Copyright © 2006-2012 Miroslav Popov
AppComments        = Freeware forex strategy back tester, generator and optimizer.
DefaultDirName     = {pf}\Forex Strategy Builder
DefaultGroupName   = Forex Strategy Builder
SourceDir          = Forex Strategy Builder
LicenseFile        = License.rtf
OutputBaseFilename = ForexStrategyBuilder
OutputDir          = ..\

[Components]
Name: "main";    Description: "Main files.";        Types: full compact custom; Flags: fixed;
Name: "data";    Description: "Data files. (Disable to prevent overwriting user data.)"; Types: full; Flags: disablenouninstallwarning;
Name: "custind"; Description: "Custom indicators."; Types: full; Flags: disablenouninstallwarning;
Name: "strat";   Description: "Demo strategies.";   Types: full; Flags: disablenouninstallwarning;
Name: "lang";    Description: "Language files.";    Types: full; Flags: disablenouninstallwarning;
Name: "color";   Description: "Color schemes.";     Types: full; Flags: disablenouninstallwarning;

[InstallDelete]
Type: files; Name: "{app}\System\lcconfig.xml";
Type: files; Name: "{app}\System\fsb-update.xml";

[Files]
Source: Forex Strategy Builder.exe; DestDir: "{app}";                      Components: main;    Flags: replacesameversion;
Source: FSB Starter.exe;            DestDir: "{app}";                      Components: main;
Source: SplashConfig.cfg;           DestDir: "{app}";                      Components: main;
Source: ConnectWait.ico;            DestDir: "{app}";                      Components: main;
Source: ReadMe.html;                DestDir: "{app}";                      Components: main;
Source: License.rtf;                DestDir: "{app}";                      Components: main;
Source: Custom Indicators\*;        DestDir: "{app}\Custom Indicators";    Components: custind;
Source: Data\*;                     DestDir: "{app}\Data";                 Components: data;
Source: Strategies\*;               DestDir: "{app}\Strategies";           Components: strat;
Source: System\Colors\*;            DestDir: "{app}\System\Colors";        Components: color;
Source: System\Languages\*;         DestDir: "{app}\System\Languages";     Components: lang;
Source: System\SplashScreen\*;      DestDir: "{app}\System\SplashScreen";  Components: main;
Source: System\StartingTips\*;      DestDir: "{app}\System\StartingTips";  Components: lang;

[Dirs]
Name: "{app}\Custom Indicators";    Permissions: users-modify;
Name: "{app}\Data";                 Permissions: users-modify;
Name: "{app}\Strategies";           Permissions: users-modify;
Name: "{app}\System";               Permissions: users-modify;
Name: "{app}\System\SplashScreen";  Permissions: users-modify;
Name: "{app}\System\Languages";     Permissions: users-modify;

[Icons]
Name: "{commondesktop}\Forex Strategy Builder"; Filename: "{app}\FSB Starter.exe"; WorkingDir: "{app}";
Name: "{group}\Forex Strategy Builder";         Filename: "{app}\FSB Starter.exe"; WorkingDir: "{app}";
Name: "{group}\Custom Indicators";              Filename: "{app}\Custom Indicators";
Name: "{group}\Data";                           Filename: "{app}\Data";
Name: "{group}\Uninstall";                      Filename: "{uninstallexe}";

[Run]
Filename: "{app}\FSB Starter.exe"; Description: "Launch the application"; Flags: postinstall skipifsilent nowait;
Filename: "{app}\ReadMe.html";     Description: "View the ReadMe file";   Flags: postinstall skipifsilent shellexec unchecked;

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
      DeleteFile(ExpandConstant('{app}\System\config.xml'));
    end;
    if OptionsPage.Values[1] = false then
    begin
      DeleteFile(ExpandConstant('{app}\System\instruments.xml'));
    end;
  end;

  if CurPageID = UsagePage.ID then
  begin
    RegWriteStringValue(HKCU, 'Software\Forex Software\Forex Strategy Builder', 'UsageStats', IntToStr(UsagePage.SelectedValueIndex));
  end;
  Result := True;
end;
