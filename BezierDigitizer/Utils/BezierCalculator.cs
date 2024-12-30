using System.Collections.Generic;
using BezierDigitizer.Models;

namespace BezierDigitizer.Utils
{
    public static class BezierCalculator
    {
        public static PointB DeCasteljau(List<PointB> points, float t)
        {
            if (points.Count == 1)
                return points[0];

            var newPoints = new List<PointB>();
            for (int i = 0; i < points.Count - 1; i++)
            {
                float x = (1 - t) * points[i].X + t * points[i + 1].X;
                float y = (1 - t) * points[i].Y + t * points[i + 1].Y;
                newPoints.Add(new PointB(x, y));
            }

            return DeCasteljau(newPoints, t);
        }
    }
}