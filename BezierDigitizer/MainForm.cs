using BezierDigitizer.Models;
using BezierDigitizer.Utils;

namespace BezierDigitizer
{
    public partial class MainForm : Form
    {
        private const int MAX_POINTS = 5;
        private readonly Color[] COLORS = new[] { 
            Color.Red, Color.Green, Color.Blue, Color.Magenta, Color.Cyan 
        };

        private List<BezierCurve> curves;
        private BezierCurve currentCurve;
        private bool isDrawingMode;

        public MainForm()
        {
            InitializeComponent();
            curves = new List<BezierCurve>();
            SetupCanvas();
        }

        private void InitializeComponent()
        {
            this.Text = "Bézier Curve Digitizer";
            this.Size = new Size(1000, 800);

            // Create buttons
            var buttonPanel = new Panel
            {
                Dock = DockStyle.Top,
                Height = 50
            };

            var drawButton = new Button
            {
                Text = "Draw Points",
                Location = new Point(10, 10),
                Size = new Size(100, 30)
            };
            drawButton.Click += DrawButton_Click;

            var clearButton = new Button
            {
                Text = "Clear",
                Location = new Point(120, 10),
                Size = new Size(100, 30)
            };
            clearButton.Click += ClearButton_Click;

            buttonPanel.Controls.AddRange(new Control[] { drawButton, clearButton });

            // Create canvas
            var canvas = new PictureBox
            {
                Dock = DockStyle.Fill,
                BackColor = Color.White
            };
            canvas.MouseClick += Canvas_MouseClick;
            canvas.Paint += Canvas_Paint;

            // Add controls to form
            this.Controls.AddRange(new Control[] { buttonPanel, canvas });
        }

        private void SetupCanvas()
        {
            isDrawingMode = false;
            currentCurve = null;
        }

        private void DrawButton_Click(object sender, EventArgs e)
        {
            if (!isDrawingMode)
            {
                isDrawingMode = true;
                currentCurve = new BezierCurve(COLORS[curves.Count % COLORS.Length]);
                (sender as Button).Text = "Finish Curve";
            }
            else
            {
                if (currentCurve.Points.Count >= 2)
                {
                    curves.Add(currentCurve);
                }
                isDrawingMode = false;
                currentCurve = null;
                (sender as Button).Text = "Draw Points";
            }
        }

        private void ClearButton_Click(object sender, EventArgs e)
        {
            curves.Clear();
            currentCurve = null;
            isDrawingMode = false;
            Refresh();
        }

        private void Canvas_MouseClick(object sender, MouseEventArgs e)
        {
            if (!isDrawingMode || currentCurve == null || 
                currentCurve.Points.Count >= MAX_POINTS)
                return;

            currentCurve.Points.Add(new PointB(e.X, e.Y));
            Refresh();
        }

        private void Canvas_Paint(object sender, PaintEventArgs e)
        {
            var g = e.Graphics;
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

            // Draw saved curves
            foreach (var curve in curves)
            {
                DrawCurve(g, curve);
                DrawPoints(g, curve);
            }

            // Draw current curve
            if (currentCurve != null)
            {
                DrawCurve(g, currentCurve);
                DrawPoints(g, currentCurve);
            }
        }

        private void DrawPoints(Graphics g, BezierCurve curve)
        {
            using (var brush = new SolidBrush(Color.Gold))
            using (var pen = new Pen(curve.Color))
            using (var font = new Font("Arial", 8))
            {
                foreach (var point in curve.Points)
                {
                    g.FillEllipse(brush, point.X - 5, point.Y - 5, 10, 10);
                    g.DrawString(
                        $"({Math.Round(point.X)},{Math.Round(point.Y)})",
                        font,
                        Brushes.Black,
                        point.X + 10,
                        point.Y - 10
                    );
                }
            }
        }

        private void DrawCurve(Graphics g, BezierCurve curve)
        {
            if (curve.Points.Count < 2)
                return;

            using (var pen = new Pen(curve.Color, 2))
            {
                const int steps = 100;
                var points = new List<PointF>();

                for (int i = 0; i <= steps; i++)
                {
                    float t = i / (float)steps;
                    var point = BezierCalculator.DeCasteljau(curve.Points, t);
                    points.Add(new PointF(point.X, point.Y));
                }

                g.DrawCurve(pen, points.ToArray());
            }
        }
    }
}