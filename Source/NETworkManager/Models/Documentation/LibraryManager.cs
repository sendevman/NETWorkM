﻿using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace NETworkManager.Models.Documentation
{
    public static class LibraryManager
    {
        private const string LicenseFolderName = "Licenses";

        public static string GetLicenseLocation()
        {
            return Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) ?? throw new DirectoryNotFoundException("Program execution directory not found, while trying to build path to license directory!"), LicenseFolderName);
        }

        public static List<LibraryInfo> List => new List<LibraryInfo>
        {
            new LibraryInfo("MahApps.Metro", "https://github.com/mahapps/mahapps.metro", "A toolkit for creating Metro / Modern UI styled WPF apps.", "MIT License", "https://github.com/MahApps/MahApps.Metro/blob/master/LICENSE"),
            new LibraryInfo("MahApps.Metro.IconPacks", "https://github.com/MahApps/MahApps.Metro.IconPacks", "Some awesome icons for WPF and UWP all together...", "MIT License", "https://github.com/MahApps/MahApps.Metro.IconPacks/blob/master/LICENSE"),
            new LibraryInfo("ControlzEx", "https://github.com/ControlzEx/ControlzEx", "Shared Controlz for WPF and ... more", "MIT License", "https://github.com/ButchersBoy/Dragablz/blob/master/LICENSE"),
            new LibraryInfo("Heijden.DNS", "https://www.codeproject.com/Articles/23673/DNS-NET-Resolver-C", "Reusable DNS resolver component.", "Code Project Open License", "https://www.codeproject.com/info/cpol10.aspx"),
            new LibraryInfo("Octokit", "https://github.com/octokit/octokit.net", "A GitHub API client library for .NET", "MIT License", "https://github.com/octokit/octokit.net/blob/master/LICENSE.txt"),
            new LibraryInfo("#SNMP Libary", "https://github.com/lextm/sharpsnmplib", "Sharp SNMP Library - Open Source SNMP for .NET and Mono", "MIT License", "https://github.com/lextm/sharpsnmplib/blob/master/LICENSE"),
            new LibraryInfo("Dragablz", "https://github.com/ButchersBoy/Dragablz", "Dragable and tearable tab control for WPF", "MIT License","https://github.com/ButchersBoy/Dragablz/blob/master/LICENSE"),
            new LibraryInfo("IPNetwork", "https://github.com/lduchosal/ipnetwork", "C# library take care of complex network, IP, IPv4, IPv6, netmask, CIDR, subnet, subnetting, supernet, and supernetting calculation for .NET developers.", "BSD-2-Clause", "https://github.com/lduchosal/ipnetwork/blob/master/LICENSE"),
            new LibraryInfo("AirspaceFixer" ,"https://github.com/chris84948/AirspaceFixer", "AirspacePanel fixes all Airspace issues with WPF-hosted Winforms.", "MIT License", "https://github.com/chris84948/AirspaceFixer/blob/master/LICENSE"),
            new LibraryInfo("Newtonsoft.Json", "https://github.com/JamesNK/Newtonsoft.Json", "Json.NET is a popular high-performance JSON framework for .NET", "MIT License","https://github.com/JamesNK/Newtonsoft.Json/blob/master/LICENSE.md"),
            new LibraryInfo("LiveCharts", "https://github.com/Live-Charts/Live-Charts", "Simple, flexible, interactive & powerful charts, maps and gauges for .Net", "MIT License","https://github.com/Live-Charts/Live-Charts/blob/master/LICENSE.TXT"),
            new LibraryInfo("LiveCharts.Wpf", "https://github.com/Live-Charts/Live-Charts", "Simple, flexible, interactive & powerful charts, maps and gauges for .Net", "MIT License","https://github.com/Live-Charts/Live-Charts/blob/master/LICENSE.TXT")
        };
    }
}
