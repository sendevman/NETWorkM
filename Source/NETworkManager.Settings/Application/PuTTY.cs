﻿using NETworkManager.Models.PuTTY;
using static NETworkManager.Models.PuTTY.PuTTY;

namespace NETworkManager.Settings.Application
{
    public static class PuTTY
    {
        public static int GetPortOrBaudByConnectionMode(ConnectionMode mode)
        {
            var portOrBaud = 0;

            switch (mode)
            {
                case ConnectionMode.SSH:
                    portOrBaud = SettingsManager.Current.PuTTY_SSHPort;
                    break;
                case ConnectionMode.Telnet:
                    portOrBaud = SettingsManager.Current.PuTTY_TelnetPort;
                    break;
                case ConnectionMode.Rlogin:
                    portOrBaud = SettingsManager.Current.PuTTY_RloginPort;
                    break;
                case ConnectionMode.RAW:
                    portOrBaud = SettingsManager.Current.PuTTY_DefaultRaw;
                    break;
                case ConnectionMode.Serial:
                    portOrBaud = SettingsManager.Current.PuTTY_BaudRate;
                    break;
            }

            return portOrBaud;
        }
    }
}
