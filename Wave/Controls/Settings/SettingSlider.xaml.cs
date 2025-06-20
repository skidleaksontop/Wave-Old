// Decompiled with JetBrains decompiler
// Type: Wave.Controls.Settings.SettingSlider
// Assembly: Wave, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 33553988-2CCE-4180-BC86-D1681DD7B18E
// Assembly location: C:\Users\zavie\Documents\Executors\V1Wave\WaveTrial\WaveTrial\Wave.exe

using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Markup;
using Wave.Classes.Cosmetic;

#nullable disable
namespace Wave.Controls.Settings;

public partial class SettingSlider : UserControl, IComponentConnector
{
    public static readonly DependencyProperty ValueProperty = DependencyProperty.Register(nameof(Value), typeof(double), typeof(SettingSlider), new PropertyMetadata((object)0.0));
    public static readonly DependencyProperty MinimumValueProperty = DependencyProperty.Register(nameof(MinimumValue), typeof(int), typeof(SettingSlider), new PropertyMetadata((object)0));
    public static readonly DependencyProperty MaximumValueProperty = DependencyProperty.Register(nameof(MaximumValue), typeof(int), typeof(SettingSlider), new PropertyMetadata((object)100));
    public static readonly DependencyProperty RoundingFactorProperty = DependencyProperty.Register(nameof(RoundingFactor), typeof(double), typeof(SettingSlider), new PropertyMetadata((object)1.0));
    public static readonly DependencyProperty TitleProperty = DependencyProperty.Register(nameof(Title), typeof(string), typeof(SettingSlider), new PropertyMetadata((object)nameof(Title)));
    public static readonly DependencyProperty DescriptionProperty = DependencyProperty.Register(nameof(Description), typeof(string), typeof(SettingSlider), new PropertyMetadata((object)nameof(Description)));

    public double Value
    {
        get => (double)this.GetValue(SettingSlider.ValueProperty);
        set
        {
            this.SetValue(SettingSlider.ValueProperty, (object)value);
            EventHandler<EventArgs> onValueChanged = this.OnValueChanged;
            if (onValueChanged == null)
                return;
            onValueChanged((object)this, new EventArgs());
        }
    }

    public int MinimumValue
    {
        get => (int)this.GetValue(SettingSlider.MinimumValueProperty);
        set => this.SetValue(SettingSlider.MinimumValueProperty, (object)value);
    }

    public int MaximumValue
    {
        get => (int)this.GetValue(SettingSlider.MaximumValueProperty);
        set => this.SetValue(SettingSlider.MaximumValueProperty, (object)value);
    }

    public double RoundingFactor
    {
        get => (double)this.GetValue(SettingSlider.RoundingFactorProperty);
        set => this.SetValue(SettingSlider.RoundingFactorProperty, (object)value);
    }

    public string Title
    {
        get => (string)this.GetValue(SettingSlider.TitleProperty);
        set => this.SetValue(SettingSlider.TitleProperty, (object)value);
    }

    public string Description
    {
        get => (string)this.GetValue(SettingSlider.DescriptionProperty);
        set => this.SetValue(SettingSlider.DescriptionProperty, (object)value);
    }

    public event EventHandler<EventArgs> OnValueChanged;

    public SettingSlider()
    {
        this.InitializeComponent();
        this.OnValueChanged += (EventHandler<EventArgs>)((sender, e) =>
        {
            this.IndicatorLabel.Content = (object)this.Value.ToString();
            Animation.Animate(new AnimationPropertyBase((object)this.IndicatorHighlight)
            {
                Property = (object)FrameworkElement.WidthProperty,
                To = (object)Math.Round((this.Value - (double)this.MinimumValue) / (double)(this.MaximumValue - this.MinimumValue) * this.IndicatorGrid.ActualWidth)
            });
        });
    }

    private void SettingSliderControl_Loaded(object sender, RoutedEventArgs e)
    {
        this.IndicatorLabel.Content = (object)this.Value.ToString();
        Animation.Animate(new AnimationPropertyBase((object)this.IndicatorHighlight)
        {
            Property = (object)FrameworkElement.WidthProperty,
            To = (object)Math.Round((this.Value - (double)this.MinimumValue) / (double)(this.MaximumValue - this.MinimumValue) * this.IndicatorGrid.ActualWidth)
        });
    }

    private async void IndicatorBorder_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
        while (Mouse.LeftButton == MouseButtonState.Pressed)
        {
            this.Value = Math.Floor(((double)this.MinimumValue + (double)(this.MaximumValue - this.MinimumValue) * (Math.Min(Math.Max(Mouse.GetPosition((IInputElement)this.IndicatorGrid).X, 0.0), this.IndicatorGrid.ActualWidth) / this.IndicatorGrid.ActualWidth)) * (1.0 / this.RoundingFactor)) / (1.0 / this.RoundingFactor);
            await Task.Delay(33);
        }
    }
}