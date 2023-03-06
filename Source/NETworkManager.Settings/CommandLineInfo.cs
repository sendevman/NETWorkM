﻿using NETworkManager.Models;

namespace NETworkManager.Settings;

public class CommandLineInfo
{
    public bool Help { get; set; }
    public bool Autostart { get; set; }
    public bool ResetSettings { get; set; }
    public int RestartPid { get; set; } = -1;

    public ApplicationName Application { get; set; } = ApplicationName.None;
    public string WrongParameter { get; set; }

    public CommandLineInfo()
    {

    }
}
