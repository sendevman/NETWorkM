﻿using log4net;
using NETworkManager.Models;
using NETworkManager.Models.Network;
using NETworkManager.Models.PowerShell;
using System;
using System.Collections.ObjectModel;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace NETworkManager.Settings
{
    public static class SettingsManager
    {
        #region Variables
        private static readonly ILog _log = LogManager.GetLogger(typeof(SettingsManager));

        private static string SettingsFolderName => "Settings";
        private static string SettingsFileName => "Settings";
        private static string SettingsFileExtension => ".xml";

        public static SettingsInfo Current { get; set; }

        public static bool HotKeysChanged { get; set; }
        #endregion

        #region Methods        
        #region Settings locations (default, custom, portable)
        public static string GetDefaultSettingsLocation()
        {
            return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), AssemblyManager.Current.Name, SettingsFolderName);
        }

        public static string GetCustomSettingsLocation()
        {
            return LocalSettingsManager.Settings_CustomSettingsLocation;
        }

        public static string GetPortableSettingsLocation()
        {

            return Path.Combine(AssemblyManager.Current.Location ?? throw new InvalidOperationException(), SettingsFolderName);
        }

        public static string GetSettingsLocation()
        {
            return ConfigurationManager.Current.IsPortable ? GetPortableSettingsLocation() : GetSettingsLocationNotPortable();
        }

        public static string GetSettingsLocationNotPortable()
        {
            var settingsLocation = GetCustomSettingsLocation();

            if (!string.IsNullOrEmpty(settingsLocation) && Directory.Exists(settingsLocation))
                return settingsLocation;

            return GetDefaultSettingsLocation();
        }
        #endregion

        #region FileName, FilePath
        public static string GetSettingsFileName()
        {
            return $"{SettingsFileName}{SettingsFileExtension}";
        }

        public static string GetSettingsFilePath()
        {
            return Path.Combine(GetSettingsLocation(), GetSettingsFileName());
        }

        #endregion

        #region Load, Save
        public static void Load()
        {
            var filePath = GetSettingsFilePath();

            if (File.Exists(filePath))
            {
                Current = DeserializeFromFile(filePath);

                Current.SettingsChanged = false;
            }
            else
            {
                InitDefault();
            }
        }

        private static SettingsInfo DeserializeFromFile(string filePath)
        {
            SettingsInfo settingsInfo;

            var xmlSerializer = new XmlSerializer(typeof(SettingsInfo));

            using (var fileStream = new FileStream(filePath, FileMode.Open))
            {
                settingsInfo = (SettingsInfo)xmlSerializer.Deserialize(fileStream);
            }

            return settingsInfo;
        }

        public static void Save()
        {
            var location = GetSettingsLocation();

            // Create the directory if it does not exist
            if (!Directory.Exists(location))
                Directory.CreateDirectory(location);

            SerializeToFile(GetSettingsFilePath());

            // Set the setting changed to false after saving them as file...
            Current.SettingsChanged = false;
        }

        private static void SerializeToFile(string filePath)
        {
            var xmlSerializer = new XmlSerializer(typeof(SettingsInfo));

            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                xmlSerializer.Serialize(fileStream, Current);
            }
        }
        #endregion

        #region Move settings
        public static Task MoveSettingsAsync(string targedLocation)
        {
            return Task.Run(() => MoveSettings(targedLocation));
        }

        private static void MoveSettings(string targedLocation)
        {
            // Save the current settings
            Save();

            // Create the dircetory and copy the files to the new location
            if (!Directory.Exists(targedLocation))
                Directory.CreateDirectory(targedLocation);

            var settingsFilePath = GetSettingsFilePath();

            // Copy file
            File.Copy(settingsFilePath, Path.Combine(targedLocation, GetSettingsFileName()), true);

            // Delete file
            File.Delete(settingsFilePath);

            // Delete folder if it is empty
            var settingsLocation = GetSettingsLocation();

            if (Directory.GetFiles(settingsLocation).Length == 0 && Directory.GetDirectories(settingsLocation).Length == 0)
                Directory.Delete(settingsLocation);
        }

        #endregion

        #region Import, Export
        public static void Import(string filePath)
        {
            using var zipArchive = ZipFile.OpenRead(filePath);

            zipArchive.GetEntry(GetSettingsFileName()).ExtractToFile(GetSettingsFilePath(), true);
        }

        public static void Export(string filePath)
        {
            // Delete existing file
            File.Delete(filePath);

            // Save the current settings
            Save();

            // Create archiv
            using var zipArchive = ZipFile.Open(filePath, ZipArchiveMode.Create);

            // Copy file
            zipArchive.CreateEntryFromFile(GetSettingsFilePath(), GetSettingsFileName(), CompressionLevel.Optimal);
        }
        #endregion

        #region Init, Reset
        public static void InitDefault()
        {
            // Init new Settings with default data
            Current = new SettingsInfo
            {
                SettingsChanged = true
            };

            Save();
        }
        #endregion

        #region Upgrade 
        public static void Upgrade(Version fromVersion, Version toVersion)
        {
            _log.Info($"Start settings upgrade from {fromVersion} to {toVersion}...");

            // Update to 2022.12.22.0            
            if (fromVersion < new Version(2022, 12, 22, 0))
            {
                _log.Info("Apply update to 2022.12.22.0");

                // Add AWS Session Manager application
                _log.Info("Add new App AWS Session Manager...");
                Current.General_ApplicationList.Add(ApplicationManager.GetList().First(x => x.Name == ApplicationName.AWSSessionManager));

                var powerShellPath = "";
                foreach (var file in PowerShell.GetDefaultIntallationPaths)
                {
                    if (File.Exists(file))
                    {
                        powerShellPath = file;
                        break;
                    }
                }

                _log.Info($"Set AWS Session Manager application file path to \"{powerShellPath}\"...");
                Current.AWSSessionManager_ApplicationFilePath = powerShellPath;

                // Add Bit Calculator application
                _log.Info("Add new App Bit Calculator...");
                Current.General_ApplicationList.Add(ApplicationManager.GetList().First(x => x.Name == ApplicationName.BitCalculator));
            }

            // Update to latest
            if (fromVersion < toVersion)
            {
                _log.Info($"Apply upgrade to {toVersion}...");

                // Add NTP Lookup application
                _log.Info("Add new App SNTP Lookup...");
                Current.General_ApplicationList.Add(ApplicationManager.GetList().First(x => x.Name == ApplicationName.SNTPLookup));
                Current.SNTPLookup_SNTPServers = new ObservableCollection<ServerConnectionInfoProfile>(SNTPServer.GetDefaultList());

                // Update default settings values
                if (Current.IPScanner_Threads > 1024)
                {
                    _log.Info("Change IP Scanner threads to 1024");
                    Current.IPScanner_Threads = 1024;
                }

                if (Current.PortScanner_HostThreads > 256)
                {
                    _log.Info("Change Port Scanner host threads to 256");
                    Current.PortScanner_HostThreads = 256;
                }

                if (Current.PortScanner_PortThreads > 1024)
                {
                    _log.Info("Change Port Scanner port threads to 1024");
                    Current.PortScanner_PortThreads = 1024;
                }
            }

            // Add or update Port Scanner port profiles
            foreach (var portProfile in PortProfile.GetDefaultList())
            {
                var portProfileFound = Current.PortScanner_PortProfiles.FirstOrDefault(x => x.Name == portProfile.Name);

                if (portProfileFound == null)
                {
                    _log.Info($"Add Port Scanner port profile \"{portProfile.Name}\"...");
                    Current.PortScanner_PortProfiles.Add(portProfile);
                }
                else if (!portProfile.Ports.Equals(portProfileFound.Ports))
                {
                    _log.Info($"Update Port Scanner port profile \"{portProfile.Name}\"...");
                    Current.PortScanner_PortProfiles.Remove(portProfileFound);
                    Current.PortScanner_PortProfiles.Add(portProfile);
                }
            }

            // Set to latest version and save
            Current.Version = toVersion.ToString();
            Save();

            _log.Info("Settings upgrade finished!");
        }
        #endregion
        #endregion
    }
}