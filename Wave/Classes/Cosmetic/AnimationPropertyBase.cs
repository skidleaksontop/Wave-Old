

// Wave, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// Wave.Classes.Cosmetic.AnimationPropertyBase
using System;
using System.Windows;
using System.Windows.Media.Animation;

namespace Wave.Classes.Cosmetic;
internal class AnimationPropertyBase
{
    internal dynamic Element;

    internal dynamic OverrideElement;

    public dynamic Property;

    public dynamic From;

    public dynamic To;

    public bool DisableEasing;

    public IEasingFunction EasingFunction = new QuarticEase();

    public Duration Duration = TimeSpan.FromSeconds(0.4);

    public AnimationPropertyBase(dynamic element)
    {
        Element = element;
    }

    public AnimationPropertyBase(dynamic element, dynamic overrideElement)
    {
        Element = element;
        OverrideElement = overrideElement;
    }

    public Timeline CreateTimeline()
    {
        Timeline timeline = null;
        string text = ((Property is string) ? Property : Property.PropertyType.Name) as string;
        switch (text)
        {
            case "Double":
                timeline = new DoubleAnimation();
                break;
            case "Thickness":
                timeline = new ThicknessAnimation();
                break;
            default:
                if (text.Contains("Color"))
                {
                    timeline = new ColorAnimation();
                }
                break;
            case null:
                break;
        }
        timeline.GetType().GetProperty("From").SetValue(timeline, ((object)From == null) ? Element.GetType().GetProperty((Property is string) ? Property : Property.Name).GetValue(Element) : From);
        timeline.GetType().GetProperty("To").SetValue(timeline, (To is int) ? Convert.ToDouble(To) : To);
        if (!DisableEasing)
        {
            timeline.GetType().GetProperty("EasingFunction").SetValue(timeline, EasingFunction);
        }
        timeline.Duration = Duration;
        return timeline;
    }
}
