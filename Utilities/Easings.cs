using System;

namespace ThePlayground.Utilities;

/// <summary>
///     Has multiple easing functions sourced from https://easings.net. The parameter to be passed in for all of these is a
///     fraction of the current progress
///     ranging from 0 to 1. The double returned is supposed to be used as the lerp coefficient.
/// </summary>
public static class Easings
{
    public static double EaseInOutSine(double x)
    {
        return -(Math.Cos(Math.PI * x) - 1) / 2;
    }

    public static double EaseOutQuart(double x)
    {
        return 1 - Math.Pow(1 - x, 4);
    }

    public static double EaseInCirc(double x)
    {
        return 1 - Math.Sqrt(1 - Math.Pow(x, 2));
    }

    public static double EaseInOutBack(double x)
    {
        const double c1 = 1.70158;
        const double c2 = c1 * 1.525;

        return x < 0.5
            ? Math.Pow(2 * x, 2) * ((c2 + 1) * 2 * x - c2) / 2
            : (Math.Pow(2 * x - 2, 2) * ((c2 + 1) * (x * 2 - 2) + c2) + 2) / 2;
    }
}