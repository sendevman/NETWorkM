<div align="center">
  <img alt="NETManager Preview" src="https://github.com/sendevman/NETManager/blob/main/Images/NETManager_128x128.png?raw=true" />
  <h1>NETManager</h1>
  <p>
    <b>A powerful open source tool for managing networks and troubleshooting network problems!</b>
  </p>
  <p>
    Connect and manage remote systems with Remote Desktop, PowerShell, PuTTY, TigerVNC or AWS (Systems Manager) Session Manager. Analyze and troubleshoot your network and systems with features such as the WiFi Analyzer, IP Scanner, Port Scanner, Ping Monitor, Traceroute, DNS lookup or LLDP/CDP capture (and many <a href="https://sendevman.net/NETManager/docs/features">more</a>) in a unfied interface. Hosts (or networks) can be saved in (encrypted) profiles and used across all features.  
  <p>
     <a href="https://sendevman.net/NETManager/download" target="_blank">
      <img alt="All releases" src="https://img.shields.io/badge/>>_download_now_<<-00abbd?style=for-the-badge" height="48" />
    </a>
  </p>
  <p>
    <a href="https://github.com/sendevman/NETManager/releases" target="_blank">
      <img alt="All releases" src="https://img.shields.io/github/downloads/sendevman/NETManager/total.svg?style=for-the-badge&logo=github" />
    </a>    
    <a href="https://github.com/sendevman/NETManager/releases/latest" target="_blank">
      <img alt="Latest release" src="https://img.shields.io/github/downloads/sendevman/NETManager/latest/total.svg?style=for-the-badge&logo=github" />
    </a>
    <a href="https://github.com/sendevman/NETManager/releases" target="_blank">
      <img alt="Latest pre-release" src="https://img.shields.io/github/downloads-pre/sendevman/NETManager/latest/total.svg?label=downloads%40pre-release&style=for-the-badge&logo=github" />
    </a>
  </p>
  <p>
    <a href="https://github.com/sendevman/NETManager/stargazers" target="_blank">
      <img alt="GitHub stars" src="https://img.shields.io/github/stars/sendevman/NETManager.svg?style=for-the-badge&logo=github" />
    </a>
    <a href="https://github.com/sendevman/NETManager/network" target="_blank">       
      <img alt="GitHub forks" src="https://img.shields.io/github/forks/sendevman/NETManager.svg?style=for-the-badge&logo=github" />
    </a>
  </p>
  <p> 
    <a href="https://ci.appveyor.com/project/sendevman/NETManager/branch/main">
      <img alt="AppVeyor" src="https://img.shields.io/appveyor/ci/sendevman/NETManager/main.svg?style=for-the-badge&logo=appveyor&&label=main" />
    </a>   
    <a href="https://github.com/sendevman/NETManager/blob/main/LICENSE">
      <img alt="AppVeyor" src="https://img.shields.io/github/license/sendevman/NETManager.svg?style=for-the-badge&logo=github" />
    </a>     
  </p> 
  <p> 
    <a href="https://transifex.com/sendevman/NETManager/">
      <img alt="Transifex" src="https://img.shields.io/badge/transifex-translate-green.svg?style=for-the-badge" />
    </a>   
    <a href="https://github.com/sendevman/NETManager/issues/new?labels=Feature-Request&template=Feature_request.md">
      <img alt="Feature request" src="https://img.shields.io/badge/github-feature_request-green.svg?style=for-the-badge&logo=github" />
    </a>       
    <a href="https://github.com/sendevman/NETManager/issues/new?labels=Issue&template=Bug_report.md">
      <img alt="Bug report" src="https://img.shields.io/badge/github-bug_report-red.svg?style=for-the-badge&logo=github" />
    </a>     
  </p>
  <p>
    <a href="#-download">Download</a> • <a href="#-changelog">Changelog</a> • <a href="#-documentation">Documentation</a> • <a href="#-contributing">Contributing</a> • <a href="#-build">Build</a> • <a href="#-license">License</a>
  </p>
</div>

<img alt="NETManager Preview" src="https://github.com/sendevman/NETManager/blob/main/Website/static/img/preview.gif?raw=true" />

## 📦 Download

Pre-built and [signed](https://sendevman.net/NETManager/download#code-signing-policy) binaries (setup, portable and archive) are available on the [download page](https://sendevman.net/NETManager/Download) with install instructions (e.g. silent install). The files are provided via [GitHub releases](https://github.com/sendevman/NETManager/releases/latest).

In addition, NETManager is also available through the following [package managers](https://sendevman.net/NETManager/download#package-manager)

- [Chocolatey](https://chocolatey.org/packages/NETManager)

  ```PowerShell
  # Install via Chocolatey
  choco install NETManager
  ```

- [WinGet](https://github.com/microsoft/winget-pkgs/tree/master/manifests/b/sendevman/NETManager/)

  ```PowerShell
  # Install via WinGet
  winget install sendevman.NETManager
  ```

- [Evergreen](https://stealthpuppy.com/evergreen/apps/)

  ```PowerShell
  # Get release via Evergreen
  Get-EvergreenApp -Name NETManager

  # Get release via Evergreen and save the setup file to disk
  Get-EvergreenApp -Name NETManager | Save-EvergreenApp -Path C:\Users\$env:Username\Downloads\
  ```

  Evergreen PowerShell module: <https://github.com/aaronparker/evergreen>

## 📃 Changelog

You can find the changelog for each version of NETManager [here](https://sendevman.net/NETManager/docs/category/changelog).

## 📖 Documentation

The documentation is provided with Docusaurus via GitHub pages and can be found [here](https://sendevman.net/NETManager/docs/introduction).

## ✨ Contributing

Want to contribute to NETManager? Here are a few information on how to get started:

- [Request a feature, report a bug or ask a question](CONTRIBUTING.md#contributing)
- [Add a feature or fix a bug](CONTRIBUTING.md#code)
- [Add or improve a translation](CONTRIBUTING.md#translation)
- [Improve the documentation](CONTRIBUTING.md#documentation)
- [Report a security vulnerability](https://github.com/sendevman/NETManager/blob/main/SECURITY.md)

A list of all contributors can be found [here](https://github.com/sendevman/NETManager/blob/main/Contributors.md).

This project has adopted the [code of conduct](https://github.com/sendevman/NETManager/blob/main/CODE_OF_CONDUCT.md) defined by the [Contributor Covenant](https://contributor-covenant.org/).

## 🔧 Build

You can build the application like any other .NET / WPF application on Windows.

1. Make sure that the following requirements are installed:

   - [.NET 8.x - SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
   - Visual Studio 2022 with `.NET desktop development` and `Universal Windows Platform development`

2. Clone the repository with all submodules:

   ```PowerShell
   # Clone the repository
   git clone https://github.com/sendevman/NETManager

   # Navigate to the repository
   cd NETManager

   # Clone the submodules
   git submodule update --init
   ```

3. Open the project file `.\Source\NETManager.sln` with Visual Studio or JetBrains Rider to build (or debug)
   the solution.

   > **ALTERNATIVE**
   >
   > With the following commands you can directly build the binaries from the command line:
   >
   > ```PowerShell
   > dotnet restore .\Source\NETManager.sln
   >
   > dotnet build .\Source\NETManager.sln --configuration Release --no-restore
   > ```

## 🙏 Thanks

Thanks to everyone helping to improve NETManager by contributing code, translations, bug reports, feature requests, documentation, and more.

We would like to thank the following people and companies for their support of this open source project:

|                                              |                                                                                             |
| -------------------------------------------- | ------------------------------------------------------------------------------------------- |
| [AppVeyor](https://www.appveyor.com/)        | CI/CD service for Windows, Linux and macOS                                                  |
| [GitHub Pages](https://pages.github.com/)    | Websites for you and your projects                                                          |
| [ip-api.com](https://ip-api.com/)            | IP Geolocation API (free for non-commercial use)                                            |
| [JetBrains](https://www.jetbrains.com/)      | Providing a license for [JetBrains Rider](https://www.jetbrains.com/rider/) and other tools |
| [SignPath Foundation](https://signpath.org/) | Free code signing certificates for open source projects                                     |
| [SignPath.io](https://signpath.io/)          | Free code signing service for open source projects                                          |
| [Transifex](https://www.transifex.com/)      | Localization platform                                                                       |

NETManager uses the following projects and libraries. Please consider supporting them as well (e.g., by starring their repositories):

|                                                                               |                                                                        |
| ----------------------------------------------------------------------------- | ---------------------------------------------------------------------- |
| [#SNMP Library](https://github.com/lextudio/sharpsnmplib)                     | SNMP library for .NET                                                  |
| [AirspaceFixer](https://github.com/chris84948/AirspaceFixer)                  | AirspacePanel fixes all Airspace issues with WPF-hosted Winforms.      |
| [ControlzEx](https://github.com/ControlzEx/ControlzEx)                        | Shared Controlz for WPF and more                                       |
| [DnsClient.NET](https://github.com/MichaCo/DnsClient.NET)                     | Powerful, high-performance open-source library for DNS lookups         |
| [Docusaurus](https://docusaurus.io/)                                          | Easy to maintain open source documentation websites.                   |
| [Dragablz](https://dragablz.net/)                                             | Tearable TabControl for WPF                                            |
| [GongSolutions.Wpf.DragDrop](https://github.com/punker76/gong-wpf-dragdrop)   | An easy to use drag'n'drop framework for WPF                           |
| [IPNetwork](https://github.com/lduchosal/ipnetwork)                           | .NET library for complex network, IP, and subnet calculations          |
| [LoadingIndicators.WPF](https://github.com/zeluisping/LoadingIndicators.WPF)  | A collection of loading indicators for WPF                             |
| [MahApps.Metro.IconPacks](https://github.com/MahApps/MahApps.Metro.IconPacks) | Awesome icon packs for WPF and UWP in one library                      |
| [MahApps.Metro](https://mahapps.com/)                                         | UI toolkit for WPF applications                                        |
| [NetBeauty2](https://github.com/nulastudio/NetBeauty2)                        | Move .NET app runtime components and dependencies into a sub-directory |
| [PSDiscoveryProtocol](https://github.com/lahell/PSDiscoveryProtocol)          | PowerShell module for LLDP/CDP discovery                               |

## Code Signing Policy

NETManager uses free code signing provided by [SignPath.io](https://signpath.io/) and a free code signing certificate
from [SignPath Foundation](https://signpath.org/).

The binaries and installer are built on [AppVeyor](https://ci.appveyor.com/project/sendevman/NETManager) directly from the [GitHub repository](https://github.com/sendevman/NETManager/blob/main/appveyor.yml).
Build artifacts are automatically sent to [SignPath.io](https://signpath.io/) via webhook, where they are signed after manual approval by the maintainer.
The signed binaries are then uploaded to the [GitHub releases](https://github.com/sendevman/NETManager/releases) page.

## Privacy Policy

This program will not transfer any information to other networked systems unless specifically requested by the user or the person installing or operating it.

NETManager has integrated the following services for additional functions, which can be enabled or disabled at the first start (in the welcome dialog) or at any time in the settings:

- [api.github.com](https://docs.github.com/en/site-policy/privacy-policies/github-general-privacy-statement) (Check for program updates)
- [ipify.org](https://www.ipify.org/) (Retrieve the public IP address used by the client)
- [ip-api.com](https://ip-api.com/docs/legal) (Retrieve network information such as geo location, ISP, DNS resolver used, etc. used by the client)

## 📝 License

NETManager is published under the [GNU General Public License v3](https://github.com/sendevman/NETManager/blob/main/LICENSE).

The licenses of the libraries used can be found [here](https://github.com/sendevman/NETManager/tree/main/Source/NETManager.Documentation/Licenses).
