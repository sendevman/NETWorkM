﻿using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace NETworkManager.Documentation;

/// <summary>
/// This class provides information's about libraries used within the program.
/// </summary>
public static class LibraryManager
{
    /// <summary>
    /// Name of the license folder.
    /// </summary>
    private const string LicenseFolderName = "Licenses";

    /// <summary>
    /// Method to get the license folder location.
    /// </summary>
    /// <returns>Location of the license folder.</returns>
    public static string GetLicenseLocation()
    {            
        return Path.Combine(Path.GetDirectoryName(Assembly.GetEntryAssembly()?.Location) ?? throw new DirectoryNotFoundException("Program execution directory not found, while trying to build path to license directory!"), LicenseFolderName);
    }

    /// <summary>
    /// Static list with all libraries that are used.
    /// </summary>
    public static List<LibraryInfo> List => new()
    {
        new LibraryInfo("MahApps.Metro", "https://github.com/mahapps/mahapps.metro", Localization.Resources.Strings.Library_MahAppsMetro_Description, Localization.Resources.Strings.License_MITLicense, "https://github.com/MahApps/MahApps.Metro/blob/master/LICENSE"),
        new LibraryInfo("MahApps.Metro.IconPacks", "https://github.com/MahApps/MahApps.Metro.IconPacks", Localization.Resources.Strings.Library_MahAppsMetroIconPacks_Description, Localization.Resources.Strings.License_MITLicense, "https://github.com/MahApps/MahApps.Metro.IconPacks/blob/master/LICENSE"),
        new LibraryInfo("ControlzEx", "https://github.com/ControlzEx/ControlzEx", Localization.Resources.Strings.Library_ControlzEx_Description, Localization.Resources.Strings.License_MITLicense, "https://github.com/ButchersBoy/Dragablz/blob/master/LICENSE"),
        new LibraryInfo("Octokit", "https://github.com/octokit/octokit.net", Localization.Resources.Strings.Library_Octokit_Description, Localization.Resources.Strings.License_MITLicense, "https://github.com/octokit/octokit.net/blob/master/LICENSE.txt"),
        new LibraryInfo("#SNMP Library", "https://github.com/lextudio/sharpsnmplib", Localization.Resources.Strings.Library_SharpSNMP_Description, Localization.Resources.Strings.License_MITLicense, "https://github.com/lextudio/sharpsnmplib/blob/master/LICENSE"),
        new LibraryInfo("Dragablz", "https://github.com/ButchersBoy/Dragablz", Localization.Resources.Strings.Library_Dragablz_Description, Localization.Resources.Strings.License_MITLicense,"https://github.com/ButchersBoy/Dragablz/blob/master/LICENSE"),
        new LibraryInfo("IPNetwork", "https://github.com/lduchosal/ipnetwork", Localization.Resources.Strings.Library_IPNetwork_Description, Localization.Resources.Strings.License_BDS2Clause, "https://github.com/lduchosal/ipnetwork/blob/master/LICENSE"),
        new LibraryInfo("AirspaceFixer" ,"https://github.com/chris84948/AirspaceFixer", Localization.Resources.Strings.Library_AirspaceFixer_Description, Localization.Resources.Strings.License_MITLicense, "https://github.com/chris84948/AirspaceFixer/blob/master/LICENSE"),
        new LibraryInfo("Newtonsoft.Json", "https://github.com/JamesNK/Newtonsoft.Json", Localization.Resources.Strings.Library_NewtonsoftJson_Description, Localization.Resources.Strings.License_MITLicense,"https://github.com/JamesNK/Newtonsoft.Json/blob/master/LICENSE.md"),
        new LibraryInfo("LiveCharts", "https://github.com/Live-Charts/Live-Charts", Localization.Resources.Strings.Library_LiveCharts_Description, Localization.Resources.Strings.License_MITLicense,"https://github.com/Live-Charts/Live-Charts/blob/master/LICENSE.TXT"),
        new LibraryInfo("LoadingIndicators.WPF", "https://github.com/zeluisping/LoadingIndicators.WPF", Localization.Resources.Strings.Library_LoadingIndicatorsWPF_Description, Localization.Resources.Strings.License_Unlicense, "https://github.com/zeluisping/LoadingIndicators.WPF/blob/master/LICENSE"),
        new LibraryInfo("DnsClient.NET", "https://github.com/MichaCo/DnsClient.NET",Localization.Resources.Strings.Library_DnsClientNET_Description, Localization.Resources.Strings.License_ApacheLicense2dot0, "https://github.com/MichaCo/DnsClient.NET/blob/dev/LICENSE"),
        new LibraryInfo("PSDiscoveryProtocol", "https://github.com/lahell/PSDiscoveryProtocol",Localization.Resources.Strings.Library_PSDicoveryProtocol_Description, Localization.Resources.Strings.License_MITLicense, "https://github.com/lahell/PSDiscoveryProtocol/blob/master/LICENSE"),
        new LibraryInfo("Microsoft.PowerShell.SDK", "https://github.com/PowerShell/PowerShell", Localization.Resources.Strings.Library_PowerShellSDK_Description, Localization.Resources.Strings.License_MITLicense, "https://github.com/PowerShell/PowerShell/blob/master/LICENSE.txt"),
        new LibraryInfo("Microsoft.Web.WebView2", "https://docs.microsoft.com/en-us/microsoft-edge/webview2/", Localization.Resources.Strings.Library_WebView2_Description, Localization.Resources.Strings.License_MicrosoftWebView2License, "https://www.nuget.org/packages/Microsoft.Web.WebView2/1.0.824-prerelease/License"),
        new LibraryInfo("Microsoft.Windows.CsWinRT", "https://github.com/microsoft/cswinrt/tree/master/", Localization.Resources.Strings.Library_CsWinRT_Description, Localization.Resources.Strings.License_MITLicense, "https://github.com/microsoft/CsWinRT/blob/master/LICENSE"),
        new LibraryInfo("Microsoft.Xaml.Behaviors.Wpf", "https://github.com/microsoft/XamlBehaviorsWpf", Localization.Resources.Strings.Library_XamlBehaviorsWpf_Description, Localization.Resources.Strings.License_MITLicense, "https://github.com/microsoft/XamlBehaviorsWpf/blob/master/LICENSE"),
        new LibraryInfo("log4net", "https://logging.apache.org/log4net/", Localization.Resources.Strings.Library_log4net_Description, Localization.Resources.Strings.License_ApacheLicense2dot0, "https://github.com/apache/logging-log4net/blob/master/LICENSE"),
        new LibraryInfo("AWSSDK.EC2", "https://github.com/aws/aws-sdk-net/", Localization.Resources.Strings.Library_AWSSDKdotEC2_Description, Localization.Resources.Strings.License_ApacheLicense2dot0, "https://aws.amazon.com/apache-2-0/"),
        new LibraryInfo("nulastudio.NetBeauty", "https://github.com/nulastudio/NetBeauty2", Localization.Resources.Strings.Library_nulastudioNetBeauty_Description, Localization.Resources.Strings.License_MITLicense, "https://github.com/nulastudio/NetBeauty2/blob/master/LICENSE"),
    };
}
