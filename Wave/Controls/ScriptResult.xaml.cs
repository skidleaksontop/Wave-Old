// Decompiled with JetBrains decompiler
// Type: Wave.Controls.ScriptResult
// Assembly: Wave, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 33553988-2CCE-4180-BC86-D1681DD7B18E
// Assembly location: C:\Users\zavie\Documents\Executors\V1Wave\WaveTrial\WaveTrial\Wave.exe

using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Wave.Classes.Passive;

#nullable disable
namespace Wave.Controls;

public partial class ScriptResult : UserControl, IComponentConnector
{
    public ScriptObject scriptObject;

    public ScriptResult() => this.InitializeComponent();

    public ScriptResult(ScriptObject scriptObj)
    {
        ScriptResult scriptResult = this;
        this.scriptObject = scriptObj;
        scriptObj.Correct();
        this.InitializeComponent();
        ImageBrush background404 = (ImageBrush)this.Icon.Background;
        BitmapImage bitmapImage = new BitmapImage();
        bitmapImage.DownloadFailed += (EventHandler<ExceptionEventArgs>)((sender, e) => scriptResult.Icon.Background = (Brush)background404);
        bitmapImage.BeginInit();
        bitmapImage.UriSource = new Uri(scriptObj.game.imageUrl, UriKind.Absolute);
        bitmapImage.EndInit();
        this.Icon.Background = (Brush)new ImageBrush()
        {
            ImageSource = (ImageSource)bitmapImage
        };
        this.Title.Content = (object)scriptObj.title;
        this.Created.Content = (object)("Created: " + scriptObj.createdAt.ToShortDateString());
        this.Views.Content = (object)("Views: " + scriptObj.views.ToString("#,##0"));
        if (!scriptObj.isPatched)
            this.Patched.Visibility = Visibility.Collapsed;
        if (!scriptObj.isUniversal)
            this.Universal.Visibility = Visibility.Collapsed;
        if (scriptObj.key)
            return;
        this.Key.Visibility = Visibility.Collapsed;
    }
}