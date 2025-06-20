// Decompiled with JetBrains decompiler
// Type: Wave.Controls.AI.AIBotMessage
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

#nullable disable
namespace Wave.Controls.AIMessages
{
    public partial class AIBotMessage : UserControl
    {
        public static readonly DependencyProperty MessageProperty = DependencyProperty.Register(nameof(Message), typeof(string), typeof(AIBotMessage), new PropertyMetadata((object)"~~ No Message Supplied ~~"));

        public string Message
        {
            get => (string)this.GetValue(AIBotMessage.MessageProperty);
            set => this.SetValue(AIBotMessage.MessageProperty, (object)value);
        }

        public AIBotMessage() => this.InitializeComponent();
    }
}