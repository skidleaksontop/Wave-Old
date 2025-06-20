

// Wave, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// SynapseXtra.ClientInformationBehavior
using System;
using System.Windows;
using Newtonsoft.Json;
using Wave.Classes.Implementations;
using WebSocketSharp;
using WebSocketSharp.Server;
using Wave;

namespace SynapseXtra;
public class ClientInformationBehavior : WebSocketBehavior
{

    // Fix: Ensure the correct namespace is used for ClientInformation.  
    // The error indicates that the compiler is treating 'ClientInformation' in the current file as 'Wave.SynapseXtra.ClientInformation'.  
    // To resolve this, explicitly specify the namespace for 'ClientInformation' in the deserialization call.  

    protected override void OnMessage(MessageEventArgs e)
    {
        try
        {
            // Explicitly specify the correct namespace for ClientInformation.  
            Roblox.GainProcessInformation(JsonConvert.DeserializeObject<ClientInformation>(e.Data));
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.InnerException?.Message ?? ex.Message);
        }
    }
}
