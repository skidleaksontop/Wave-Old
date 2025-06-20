

// Wave, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// Wave.App
using System;
using System.CodeDom.Compiler;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Windows;
using CefSharp;
using CefSharp.Wpf;
using Wave;
using Wave.Classes.Implementations;

namespace Wave;
public partial class App : Application
{
    private readonly string[] directories = new string[6] { "autoexec", "data", "data/tabs", "dependencies", "scripts", "workspace" };

    private Process lspProc;

    private void Application_Startup(object sender, StartupEventArgs e)
    {
        if (Process.GetProcessesByName("Wave").Length > 1)
        {
            MessageBox.Show("Wave is already open, please close it.");
            Environment.Exit(0);
            return;
        }
        if (Assembly.GetEntryAssembly().Location.StartsWith(Path.GetTempPath(), StringComparison.OrdinalIgnoreCase))
        {
            MessageBox.Show("Extract Wave before opening it.");
            Environment.Exit(0);
            return;
        }
        if (!File.Exists(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + "/Bloxstrap/Bloxstrap.exe"))
        {
            MessageBox.Show("You must install Bloxstrap to use Wave.");
            Environment.Exit(0);
            return;
        }
        if (Bloxstrap.Instance.Channel != "Live")
        {
            Process[] processesByName = Process.GetProcessesByName("RobloxPlayerBeta");
            if (processesByName.Length != 0)
            {
                if (MessageBox.Show("You are on an unsupported Roblox Channel. Close your games so we can correct this?", "!", MessageBoxButton.YesNo) == MessageBoxResult.No)
                {
                    Environment.Exit(0);
                    return;
                }
                Process[] array = processesByName;
                for (int i = 0; i < array.Length; i++)
                {
                    array[i].Kill();
                }
            }
            Bloxstrap.Instance.Channel = "Live";
            Bloxstrap.Instance.Save();
        }
        string[] array2 = directories;
        foreach (string path in array2)
        {
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
        }
        CefSettings cefSettings = new CefSettings();
        cefSettings.SetOffScreenRenderingBestPerformanceArgs();
        Cef.Initialize(cefSettings);
        lspProc = Process.Start(new ProcessStartInfo(Environment.CurrentDirectory + "/dist/node.exe")
        {
            WorkingDirectory = Environment.CurrentDirectory + "/dist",
            Arguments = "server",
            WindowStyle = ProcessWindowStyle.Hidden,
            CreateNoWindow = true
        });
        
    }

    private void Application_Exit(object sender, ExitEventArgs e)
    {
        if (!lspProc.HasExited)
        {
            lspProc.Kill();
        }
    }
}
