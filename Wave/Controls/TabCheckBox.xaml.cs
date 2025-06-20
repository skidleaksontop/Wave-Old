

// Wave, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// Wave.Controls.TabCheckBox
using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Shapes;
using Wave.Classes.Cosmetic;
using Wave.Controls;
using Wave.Classes;

namespace Wave.Controls
{
    public partial class TabCheckBox : UserControl
    {
        public static readonly DependencyProperty CanToggleProperty = DependencyProperty.Register("CanToggle", typeof(bool), typeof(TabCheckBox), new PropertyMetadata(true));

        public static readonly DependencyProperty EnabledProperty = DependencyProperty.Register("Enabled", typeof(bool), typeof(TabCheckBox), new PropertyMetadata(false));

        public static readonly DependencyProperty IconProperty = DependencyProperty.Register("Icon", typeof(string), typeof(TabCheckBox), new PropertyMetadata(""));

        public static readonly DependencyProperty IsIconUniformProperty = DependencyProperty.Register("IsIconUniform", typeof(bool), typeof(TabCheckBox), new PropertyMetadata(false));

        public static readonly DependencyProperty BackgroundSelectedProperty = DependencyProperty.Register("BackgroundSelected", typeof(Brush), typeof(TabCheckBox), new PropertyMetadata(new SolidColorBrush(Color.FromRgb(29, 29, 30))));

        public static readonly DependencyProperty IconSelectedProperty = DependencyProperty.Register("IconSelected", typeof(Brush), typeof(TabCheckBox), new PropertyMetadata(new SolidColorBrush(Colors.White)));

        public bool CanToggle
        {
            get
            {
                return (bool)GetValue(CanToggleProperty);
            }
            set
            {
                SetValue(CanToggleProperty, value);
            }
        }

        public bool Enabled
        {
            get
            {
                return (bool)GetValue(EnabledProperty);
            }
            set
            {
                if (CanToggle)
                {
                    SetValue(EnabledProperty, value);
                    if (value)
                    {
                        this.OnEnabled(this, new EventArgs());
                    }
                    else
                    {
                        this.OnDisabled(this, new EventArgs());
                    }
                }
            }
        }

        public string Icon
        {
            get
            {
                return (string)GetValue(IconProperty);
            }
            set
            {
                SetValue(IconProperty, value);
            }
        }

        public bool IsIconUniform
        {
            get
            {
                return (bool)GetValue(IsIconUniformProperty);
            }
            set
            {
                SetValue(IsIconUniformProperty, value);
                IconPath.Stretch = ((!value) ? Stretch.Fill : Stretch.Uniform);
            }
        }

        public Brush BackgroundSelected
        {
            get
            {
                return (Brush)GetValue(BackgroundSelectedProperty);
            }
            set
            {
                SetValue(BackgroundSelectedProperty, value);
            }
        }

        public Brush IconSelected
        {
            get
            {
                return (Brush)GetValue(IconSelectedProperty);
            }
            set
            {
                SetValue(IconSelectedProperty, value);
            }
        }

        public event EventHandler OnEnabled;

        public event EventHandler OnDisabled;

        public TabCheckBox()
        {
            InitializeComponent();
            OnEnabled += TabButton_OnEnabled;
            OnDisabled += TabButton_OnDisabled;
        }

        private void TabButton_OnEnabled(object sender, EventArgs e)
        {
            Animation.Animate(new AnimationPropertyBase(Highlight)
            {
                Property = UIElement.OpacityProperty,
                To = 1
            }, new AnimationPropertyBase(IconHighlight)
            {
                Property = UIElement.OpacityProperty,
                To = 1
            });
        }

        private void TabButton_OnDisabled(object sender, EventArgs e)
        {
            Animation.Animate(new AnimationPropertyBase(Highlight)
            {
                Property = UIElement.OpacityProperty,
                To = 0
            }, new AnimationPropertyBase(IconHighlight)
            {
                Property = UIElement.OpacityProperty,
                To = 0
            });
        }

        private void MainGrid_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Enabled = !Enabled;
        }
    }
}
