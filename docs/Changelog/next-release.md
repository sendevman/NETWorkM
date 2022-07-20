---
layout: default
title: Next release
parent: Changelog
nav_order: 1
description: "Changelog for next release"
permalink: /Changelog/next-release
---

# Next release

**System requirements**
- Windows 10 / Server x64 (1809 or later)
- [.NET Desktop Runtime 6.x (LTS)](https://dotnet.microsoft.com/download/dotnet/6.0){:target="_blank"}
- [Microsoft Edge - WebView2 Runtime (Evergreen)](https://developer.microsoft.com/en-us/microsoft-edge/webview2/){:target="_blank"}

## What's new?
- Settings
  - Add support for [MahApps.Metro custom themes](https://mahapps.com/docs/themes/thememanager#creating-custom-themes). Themes can be placed in the folder `Themes` in the application root [#1439](https://github.com/BornToBeRoot/NETworkManager/pull/1439){:target="_blank"}
  
## Improvements
- Profiles
  - Migration dialog improved [#1393](https://github.com/BornToBeRoot/NETworkManager/pull/1393){:target="_blank"}

## Bugfixes
- Dashboard / Status Window
  - Detect local ipv6 address fixed [#1423](https://github.com/BornToBeRoot/NETworkManager/pull/1423){:target="_blank"}
- Profiles
  - Fixed some rare cases where the profile file was overwritten [#1495](https://github.com/BornToBeRoot/NETworkManager/pull/1425){:target="_blank"}
  - Header in add group dialog fixed [#1426](https://github.com/BornToBeRoot/NETworkManager/pull/1426){:target="_blank"}
- IP Scanner / OUI Lookup
  - Use ieee.org instead of linuxnet.ca to generate the oui.txt [#1460](https://github.com/BornToBeRoot/NETworkManager/pull/1460){:target="_blank"}

## Other
- Language files updated [#transifex](https://github.com/BornToBeRoot/NETworkManager/pulls?q=author%3Aapp%2Ftransifex-integration){:target="_blank"}
- Dependencies updated [#dependencies](https://github.com/BornToBeRoot/NETworkManager/pulls?q=author%3Aapp%2Fdependabot){:target="_blank"}
