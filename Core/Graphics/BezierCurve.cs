using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

// ReSharper disable MemberCanBePrivate.Global

namespace ThePlayground.Core.Graphics;

// from SLR

/// <summary>
///     A bezier curve is a parametric curve. A set of discrete "control points" defines a smooth, continuous curve by
///     means of a formula.
/// </summary>
/// <param name="controls">Control points for the curve</param>
public class BezierCurve(params Vector2[] controls)
{
    public Vector2 this[int x]
    {
        get => controls[x];
        set => controls[x] = value;
    }

    /// <summary>
    ///     Return a Vector2 at value percentage along the bezier curve.
    /// </summary>
    /// <param name="percentage">How far along the bezier curve to return a point.</param>
    /// <returns></returns>
    public Vector2 Evaluate(float percentage)
    {
        Math.Clamp(percentage, 0, 1);

        return PrivateEvaluate(controls, percentage);
    }

    /// <summary>
    ///     Get a list of points along the bezier curve. Must be at least 2.
    /// </summary>
    /// <param name="amount">The amount of points to get.</param>
    /// <returns>A list of Vector2s representing the points.</returns>
    public List<Vector2> GetPoints(int amount)
    {
        if (amount < 2)
            amount = 2;

        float perStep = 1f / (amount - 1); // step size

        var points = new List<Vector2>();
        for (int i = 0; i < amount; i++) points.Add(Evaluate(perStep * i));

        return points;
    }

    private static Vector2 PrivateEvaluate(Vector2[] points, float percentage)
    {
        if (points.Length > 2)
        {
            var nextPoints = new Vector2[points.Length - 1];
            for (int i = 0; i < points.Length - 1; i++) nextPoints[i] = Vector2.Lerp(points[i], points[i + 1], percentage);

            return PrivateEvaluate(nextPoints, percentage);
        }

        return Vector2.Lerp(points[0], points[1], percentage);
    }
}