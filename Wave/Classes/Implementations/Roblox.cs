

// Wave, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// Wave.Classes.Implementations.Roblox
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using SynapseXtra;
using Wave.Classes.Implementations;

internal class Roblox
{
    public static List<RobloxInstance> RobloxInstances = new List<RobloxInstance>();

    private static readonly System.Timers.Timer autoAttachTimer = new System.Timers.Timer(2500.0);

    private static readonly System.Timers.Timer deadProcessTimer = new System.Timers.Timer(2500.0);

    public static event EventHandler<RobloxInstanceEventArgs> OnProcessFound;

    public static event EventHandler<RobloxInstanceEventArgs> OnProcessAdded;

    public static event EventHandler<RobloxInstanceEventArgs> OnProcessRemoved;

    public static event EventHandler<DetailedRobloxInstanceEventArgs> OnProcessInformationGained;

    public static void Start()
    {
        Process[] processesByName = Process.GetProcessesByName("RobloxPlayerBeta");
        foreach (Process robloxProcess in processesByName)
        {
            if (new RobloxInstance(robloxProcess).IsInjected())
            {
                AddProcess(robloxProcess, alreadyExisted: true);
            }
        }
        autoAttachTimer.Elapsed += async delegate
        {
            Process[] processesByName2 = Process.GetProcessesByName("RobloxPlayerBeta");
            foreach (Process process in processesByName2)
            {
                if (!IsProcessAdded(process))
                {
                    AddProcess(process);
                    await Task.Delay(5000);
                    Process.Start("Injector.exe", process.Id.ToString());
                    break;
                }
            }
        };
        autoAttachTimer.Start();
        deadProcessTimer.Elapsed += delegate
        {
            for (int j = 0; j < RobloxInstances.Count; j++)
            {
                RobloxInstance instance = RobloxInstances[j];
                if (instance.RobloxProcess.HasExited || (instance.IsRunning && !instance.IsInjected()))
                {
                    Application.Current.Dispatcher.Invoke(delegate
                    {
                        RemoveProcess(instance.RobloxProcess);
                    });
                }
            }
        };
        deadProcessTimer.Start();
    }

    public static async void AddProcess(Process robloxProcess, bool alreadyExisted = false)
    {
        RobloxInstance robloxInstance = new RobloxInstance(robloxProcess, awaitInjection: true);
        RobloxInstances.Add(robloxInstance);
        Roblox.OnProcessFound?.Invoke(robloxProcess, new RobloxInstanceEventArgs(robloxProcess.Id, alreadyExisted));
        while (!robloxInstance.IsRunning)
        {
            await Task.Delay(250);
        }
        Roblox.OnProcessAdded?.Invoke(robloxProcess, new RobloxInstanceEventArgs(robloxProcess.Id, alreadyExisted));
    }

    public static void RemoveProcess(Process robloxProcess)
    {
        for (int i = 0; i < RobloxInstances.Count; i++)
        {
            RobloxInstance robloxInstance = RobloxInstances[i];
            if (robloxInstance.RobloxProcess == robloxProcess)
            {
                RobloxInstances.Remove(robloxInstance);
                break;
            }
        }
        Roblox.OnProcessRemoved?.Invoke(null, new RobloxInstanceEventArgs(robloxProcess.Id, alreadyOpen: false));
    }

    public static bool IsProcessAdded(Process robloxProcess)
    {
        for (int i = 0; i < RobloxInstances.Count; i++)
        {
            if (RobloxInstances[i].RobloxProcess.Id == robloxProcess.Id)
            {
                return true;
            }
        }
        return false;
    }

    public static void ExecuteSpecific(int[] processIds, string script)
    {
        for (int i = 0; i < RobloxInstances.Count; i++)
        {
            RobloxInstance robloxInstance = RobloxInstances[i];
            if (processIds.Contains(robloxInstance.ProcessId) && robloxInstance.IsInjected())
            {
                robloxInstance.ExecuteScript(script);
            }
        }
    }

    public static void ExecuteAll(string script)
    {
        for (int i = 0; i < RobloxInstances.Count; i++)
        {
            RobloxInstance robloxInstance = RobloxInstances[i];
            if (robloxInstance.IsInjected())
            {
                robloxInstance.ExecuteScript(script);
            }
        }
    }

    public static void GainProcessInformation(ClientInformation clientInfo)
    {
        Roblox.OnProcessInformationGained?.Invoke(null, new DetailedRobloxInstanceEventArgs
        {
            Username = clientInfo.Username,
            UserId = clientInfo.UserId,
            ProcessId = clientInfo.ProcessId,
            JobId = clientInfo.JobId
        });
    }
}
