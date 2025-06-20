

// Wave, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// Wave.Controls.AI.AIUserMessage
using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using Wave.Controls.AIMessages;

namespace Wave.Controls.AIMessages
{
    public partial class AIUserMessage : UserControl, IComponentConnector
    {
        public static readonly DependencyProperty UsernameProperty = DependencyProperty.Register("Username", typeof(string), typeof(AIUserMessage), new PropertyMetadata("Kieran"));

        public static readonly DependencyProperty MessageProperty = DependencyProperty.Register("Message", typeof(string), typeof(AIUserMessage), new PropertyMetadata("~~ No Message Supplied ~~"));

        public string Username
        {
            get
            {
                return (string)GetValue(UsernameProperty);
            }
            set
            {
                SetValue(UsernameProperty, value);
            }
        }

        public string Message
        {
            get
            {
                return (string)GetValue(MessageProperty);
            }
            set
            {
                SetValue(MessageProperty, value);
            }
        }

        public AIUserMessage()
        {
            InitializeComponent();
        }
    }
}