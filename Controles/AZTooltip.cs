using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Xamarin;
using Xamarin.Forms;
public enum TooltipPosition { Top, Left, Right, Bottom }
public static class AZTooltipEffect
{
    public static readonly BindableProperty HasTooltipProperty = BindableProperty.CreateAttached("HasTooltip", typeof(bool), typeof(AZTooltipEffect), true, propertyChanged: OnHasTooltipChanged);
    public static readonly BindableProperty TextColorProperty = BindableProperty.CreateAttached("TextColor", typeof(Color), typeof(AZTooltipEffect), Color.White);
    public static readonly BindableProperty BackgroundColorProperty = BindableProperty.CreateAttached("BackgroundColor", typeof(Color), typeof(AZTooltipEffect), Color.Black);
    public static readonly BindableProperty TextProperty = BindableProperty.CreateAttached("Text", typeof(string), typeof(AZTooltipEffect), "Tooltip");
    public static readonly BindableProperty PositionProperty = BindableProperty.CreateAttached("Position", typeof(TooltipPosition), typeof(AZTooltipEffect), TooltipPosition.Bottom);

    public static bool GetHasTooltip(BindableObject view)
    {
        return (bool)view.GetValue(HasTooltipProperty);
    }
    public static void SetHasTooltip(BindableObject view, bool value)
    {
        view.SetValue(HasTooltipProperty, value);
    }

    public static Color GetTextColor(BindableObject view)
    {
        return (Color)view.GetValue(TextColorProperty);
    }
    public static void SetTextColor(BindableObject view, Color value)
    {
        view.SetValue(TextColorProperty, value);
    }

    public static Color GetBackgroundColor(BindableObject view)
    {
        return (Color)view.GetValue(BackgroundColorProperty);
    }
    public static void SetBackgroundColor(BindableObject view, Color value)
    {
        view.SetValue(BackgroundColorProperty, value);
    }

    public static string GetText(BindableObject view)
    {
        return (string)view.GetValue(TextProperty);
    }
    public static void SetText(BindableObject view, string value)
    {
        view.SetValue(TextProperty, value);
    }

    public static TooltipPosition GetPosition(BindableObject view)
    {
        return (TooltipPosition)view.GetValue(PositionProperty);
    }
    public static void SetPosition(BindableObject view, TooltipPosition value)
    {
        view.SetValue(PositionProperty, value);
    }

    static void OnHasTooltipChanged(BindableObject bindable, object oldValue, object newValue)
    {
        var view = bindable as View;
        if (view == null)
        {
            return;
        }
        bool hasTooltip = (bool)newValue;
        if (hasTooltip)
        {
            view.Effects.Add(new ControlTooltipEffect());
        }
        else
        {
            var toRemove = view.Effects.FirstOrDefault(e => e is ControlTooltipEffect);
            if (toRemove != null)
            {
                view.Effects.Remove(toRemove);
            }
        }
    }
}

class ControlTooltipEffect : RoutingEffect
{
    public ControlTooltipEffect() : base("CrossGeeks.TooltipEffect")
    {

    }
}