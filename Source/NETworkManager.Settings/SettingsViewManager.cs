﻿using MahApps.Metro.IconPacks;
using NETworkManager.Models;
using System.Collections.Generic;

namespace NETworkManager.Settings;

public static class SettingsViewManager
{
    // List of all applications
    public static List<SettingsViewInfo> List => new()
    {
        // General
        new SettingsViewInfo(SettingsViewName.General, new PackIconModern{ Kind = PackIconModernKind.Layer }, SettingsViewGroup.General),
        new SettingsViewInfo(SettingsViewName.Window, new PackIconOcticons { Kind = PackIconOcticonsKind.Browser }, SettingsViewGroup.General),
        new SettingsViewInfo(SettingsViewName.Appearance, new PackIconMaterial { Kind = PackIconMaterialKind.Palette }, SettingsViewGroup.General),
        new SettingsViewInfo(SettingsViewName.Language, new PackIconMaterial { Kind = PackIconMaterialKind.Translate }, SettingsViewGroup.General),
        new SettingsViewInfo(SettingsViewName.Network, new PackIconModern { Kind = PackIconModernKind.Network }, SettingsViewGroup.General),
        new SettingsViewInfo(SettingsViewName.Status, new PackIconMaterial { Kind = PackIconMaterialKind.Pulse }, SettingsViewGroup.General),
        new SettingsViewInfo(SettingsViewName.HotKeys, new PackIconOcticons { Kind = PackIconOcticonsKind.Keyboard }, SettingsViewGroup.General),
        new SettingsViewInfo(SettingsViewName.Autostart, new PackIconMaterial { Kind = PackIconMaterialKind.Power }, SettingsViewGroup.General),
        new SettingsViewInfo(SettingsViewName.Update, new PackIconMaterial { Kind = PackIconMaterialKind.RocketLaunchOutline }, SettingsViewGroup.General),            
        new SettingsViewInfo(SettingsViewName.Profiles, new PackIconFontAwesome { Kind = PackIconFontAwesomeKind.ServerSolid }, SettingsViewGroup.General),
        new SettingsViewInfo(SettingsViewName.Settings, new PackIconMaterialLight { Kind = PackIconMaterialLightKind.Cog }, SettingsViewGroup.General),
       
        // Applications
        new SettingsViewInfo(SettingsViewName.Dashboard, ApplicationManager.GetIcon(ApplicationName.Dashboard), SettingsViewGroup.Applications),
        new SettingsViewInfo(SettingsViewName.IPScanner, ApplicationManager.GetIcon(ApplicationName.IPScanner), SettingsViewGroup.Applications),
        new SettingsViewInfo(SettingsViewName.PortScanner, ApplicationManager.GetIcon(ApplicationName.PortScanner), SettingsViewGroup.Applications),
        new SettingsViewInfo(SettingsViewName.PingMonitor, ApplicationManager.GetIcon(ApplicationName.PingMonitor), SettingsViewGroup.Applications),
        new SettingsViewInfo(SettingsViewName.Traceroute, ApplicationManager.GetIcon(ApplicationName.Traceroute), SettingsViewGroup.Applications),
        new SettingsViewInfo(SettingsViewName.DNSLookup, ApplicationManager.GetIcon(ApplicationName.DNSLookup), SettingsViewGroup.Applications ),
        new SettingsViewInfo(SettingsViewName.RemoteDesktop, ApplicationManager.GetIcon(ApplicationName.RemoteDesktop), SettingsViewGroup.Applications),
        new SettingsViewInfo(SettingsViewName.PowerShell, ApplicationManager.GetIcon(ApplicationName.PowerShell), SettingsViewGroup.Applications),
        new SettingsViewInfo(SettingsViewName.PuTTY, ApplicationManager.GetIcon(ApplicationName.PuTTY), SettingsViewGroup.Applications),
        new SettingsViewInfo(SettingsViewName.AWSSessionManager, ApplicationManager.GetIcon(ApplicationName.AWSSessionManager), SettingsViewGroup.Applications),
        new SettingsViewInfo(SettingsViewName.TigerVNC, ApplicationManager.GetIcon(ApplicationName.TigerVNC), SettingsViewGroup.Applications),
        new SettingsViewInfo(SettingsViewName.WebConsole, ApplicationManager.GetIcon(ApplicationName.WebConsole), SettingsViewGroup.Applications),
        new SettingsViewInfo(SettingsViewName.SNMP, ApplicationManager.GetIcon(ApplicationName.SNMP), SettingsViewGroup.Applications),
        new SettingsViewInfo(SettingsViewName.SNTPLookup, ApplicationManager.GetIcon(ApplicationName.SNTPLookup), SettingsViewGroup.Applications),
        new SettingsViewInfo(SettingsViewName.WakeOnLAN, ApplicationManager.GetIcon(ApplicationName.WakeOnLAN), SettingsViewGroup.Applications),
        new SettingsViewInfo(SettingsViewName.BitCalculator, ApplicationManager.GetIcon(ApplicationName.BitCalculator), SettingsViewGroup.Applications),           
    };
}

