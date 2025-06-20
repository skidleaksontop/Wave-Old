// Decompiled with JetBrains decompiler
// Type: Wave.Controls.Settings.SettingCheckBox
// Assembly: Wave, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 33553988-2CCE-4180-BC86-D1681DD7B18E
// Assembly location: C:\Users\zavie\Documents\Executors\V1Wave\WaveTrial\WaveTrial\Wave.exe

using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Shapes;
using Wave.Classes.Cosmetic;

namespace Wave.Controls.Settings
{

    public partial class SettingCheckBox : UserControl, IComponentConnector
    {
        public static readonly DependencyProperty IsCheckedProperty = DependencyProperty.Register(nameof(IsChecked), typeof(bool), typeof(SettingCheckBox), new PropertyMetadata((object)false));
        public static readonly DependencyProperty TitleProperty = DependencyProperty.Register(nameof(Title), typeof(string), typeof(SettingCheckBox), new PropertyMetadata((object)nameof(Title)));
        public static readonly DependencyProperty DescriptionProperty = DependencyProperty.Register(nameof(Description), typeof(string), typeof(SettingCheckBox), new PropertyMetadata((object)nameof(Description)));

        public bool IsChecked
        {
            get => (bool)this.GetValue(SettingCheckBox.IsCheckedProperty);
            set
            {
                this.SetValue(SettingCheckBox.IsCheckedProperty, (object)value);
                EventHandler<EventArgs> onCheckedChanged = this.OnCheckedChanged;
                if (onCheckedChanged != null)
                    onCheckedChanged((object)this, new EventArgs());
                Animation.Animate(new AnimationPropertyBase((object)this.IndicatorPath)
                {
                    Property = (object)UIElement.OpacityProperty,
                    To = (object)(value ? 1 : 0)
                });
            }
        }

        public string Title
        {
            get => (string)this.GetValue(SettingCheckBox.TitleProperty);
            set => this.SetValue(SettingCheckBox.TitleProperty, (object)value);
        }

        public string Description
        {
            get => (string)this.GetValue(SettingCheckBox.DescriptionProperty);
            set => this.SetValue(SettingCheckBox.DescriptionProperty, (object)value);
        }

        public event EventHandler<EventArgs> OnCheckedChanged;

        public SettingCheckBox() => this.InitializeComponent();

        private void MainBorder_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.IsChecked = !this.IsChecked;
        }
    }
}