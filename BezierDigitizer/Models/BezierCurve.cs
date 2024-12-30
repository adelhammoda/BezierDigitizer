

namespace BezierDigitizer.Models
{
    public class BezierCurve
    {
        public string Id { get; }
        public List<PointB> Points { get; }
        public Color Color { get; }

        public BezierCurve(Color color)
        {
            Id = Guid.NewGuid().ToString();
            Points = new List<PointB>();
            Color = color;
        }
    }
}