﻿using System;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace NETworkManager.Models.Settings
{
    public static class SettingsManager
    {
        private const string SettingsFolderName = "Settings";
        private const string SettingsFileExtension = ".settings";
        private const string IsPortableFileName = "IsPortable" + SettingsFileExtension;

        public static SettingsInfo Current { get; set; }

        public static bool ForceRestart { get; set; }
        public static bool HotKeysChanged { get; set; }

        private static string GetApplicationName()
        {
            return Assembly.GetEntryAssembly().GetName().Name;
        }

        private static string GetApplicationLocation()
        {
            return Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        }

        public static string GetSettingsFileName()
        {
            return string.Format("{0}{1}", GetApplicationName(), SettingsFileExtension);
        }

        #region Settings locations (default, custom, portable)
        public static string GetDefaultSettingsLocation()
        {
            return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), GetApplicationName(), SettingsFolderName);
        }

        public static string GetCustomSettingsLocation()
        {
            return Properties.Settings.Default.Settings_CustomSettingsLocation;
        }

        public static string GetPortableSettingsLocation()
        {
            return Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), SettingsFolderName);
        }

        #endregion

        #region File paths
        private static string GetIsPortableFilePath()
        {
            return Path.Combine(GetApplicationLocation(), IsPortableFileName);
        }

        public static string GetSettingsFilePath()
        {
            return Path.Combine(GetSettingsLocation(), GetSettingsFileName());
        }

        #endregion

        #region IsPortable, SettingsLocation, SettingsLocationNotPortable
        public static bool GetIsPortable()
        {
            return File.Exists(GetIsPortableFilePath());
        }

        public static string GetSettingsLocation()
        {
            if (GetIsPortable())
                return GetPortableSettingsLocation();

            string settingsLocation = GetCustomSettingsLocation();

            if (!string.IsNullOrEmpty(settingsLocation) && Directory.Exists(settingsLocation))
                return settingsLocation;

            return GetDefaultSettingsLocation();
        }

        public static string GetSettingsLocationNotPortable()
        {
            string settingsLocation = GetCustomSettingsLocation();

            if (!string.IsNullOrEmpty(settingsLocation) && Directory.Exists(settingsLocation))
                return settingsLocation;

            return GetDefaultSettingsLocation();
        }
        #endregion

        public static void Load()
        {
            if (File.Exists(GetSettingsFilePath()) && !CommandLineManager.Current.ResetSettings)
            {
                SettingsInfo settingsInfo = new SettingsInfo();

                XmlSerializer xmlSerializer = new XmlSerializer(typeof(SettingsInfo));

                using (FileStream fileStream = new FileStream(GetSettingsFilePath(), FileMode.Open))
                {
                    settingsInfo = (SettingsInfo)(xmlSerializer.Deserialize(fileStream));
                }

                Current = settingsInfo;

                // Set the setting changed to false after loading them from a file...
                Current.SettingsChanged = false;
            }
            else
            {
                Current = new SettingsInfo();
            }
        }

        public static void Save()
        {
            // Create the directory if it does not exist
            Directory.CreateDirectory(GetSettingsLocation());

            XmlSerializer xmlSerializer = new XmlSerializer(typeof(SettingsInfo));

            using (FileStream fileStream = new FileStream(Path.Combine(GetSettingsFilePath()), FileMode.Create))
            {
                xmlSerializer.Serialize(fileStream, Current);
            }

            // Set the setting changed to false after saving them as file...
            Current.SettingsChanged = false;
        }

        public static Task MoveSettingsAsync(string sourceLocation, string targedLocation)
        {
            return Task.Run(() => MoveSettings(sourceLocation, targedLocation));
        }

        private static void MoveSettings(string sourceLocation, string targedLocation)
        {
            string[] sourceFiles = Directory.GetFiles(sourceLocation);

            // Create the dircetory and copy the files to the new location
            Directory.CreateDirectory(targedLocation);

            foreach (string file in sourceFiles)
                File.Copy(file, Path.Combine(targedLocation, Path.GetFileName(file)), true);

            // Delete the old files
            foreach (string file in sourceFiles)
                File.Delete(file);

            // Delete the folder, if it is not the default settings locations and does not contain any files or directories
            if (sourceLocation != GetDefaultSettingsLocation() && Directory.GetFiles(sourceLocation).Length == 0 && Directory.GetDirectories(sourceLocation).Length == 0)
                Directory.Delete(sourceLocation);
        }

        public static Task MakePortableAsync(bool isPortable)
        {
            return Task.Run(() => MakePortable(isPortable));
        }

        public static void MakePortable(bool isPortable)
        {
            if (isPortable)
            {
                MoveSettings(GetSettingsLocationNotPortable(), GetPortableSettingsLocation());

                // After moving the files, set the indicator that the settings are now portable
                File.Create(GetIsPortableFilePath());
            }
            else
            {
                MoveSettings(GetPortableSettingsLocation(), GetSettingsLocationNotPortable());

                // Remove the indicator after moving the files...
                File.Delete(GetIsPortableFilePath());
            }
        }

        public static void Reset()
        {
            // Init new Settings with default data
            Current = new SettingsInfo()
            {
                SettingsChanged = true
            };

            ForceRestart = true;
        }
    }
}