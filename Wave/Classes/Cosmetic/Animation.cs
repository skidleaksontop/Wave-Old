﻿

// Wave, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// Wave.Classes.Cosmetic.Animation
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Animation;
using Wave.Classes.Cosmetic;

namespace Wave.Classes.Cosmetic;
internal class Animation
{
    internal class Presets
    {
    }

    public static Storyboard Animate(params AnimationPropertyBase[] propertyBases)
    {
        Storyboard storyboard = new Storyboard();
        foreach (AnimationPropertyBase animationPropertyBase in propertyBases)
        {
            Timeline timeline = animationPropertyBase.CreateTimeline();
            Storyboard.SetTarget(timeline, (animationPropertyBase.OverrideElement == null) ? animationPropertyBase.Element : animationPropertyBase.OverrideElement);
            Storyboard.SetTargetProperty(timeline, new PropertyPath(animationPropertyBase.Property));
            storyboard.Children.Add(timeline);
        }
        Task.Run(delegate
        {
            Application.Current.Dispatcher.Invoke(delegate
            {
                storyboard.Begin();
            });
        });
        return storyboard;
    }
}
