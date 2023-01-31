using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace LifeGame.Charting
{
    /*
     *  Элемент для отображения графиков
     */
    public class ChartingElement : Canvas
    {
        public ChartingElement()
        {
            visuals = new VisualCollection(this);
        }

        private VisualCollection visuals;

        // Эти переменные нужны для рисования графика с сеткой
        private double xStep; // Ширина квадрата сетки
        private double yStep; // Высота квадрата сетки
        private double compressionYRatio; // Коэффициент сжатия по высоте, нужно для вмещения сетки
        private double margin = 0; // Отступ от краёв

        // Эти переменные нужны для вмещения графика в элемент
        double maxChartsValue = 0; // Максимальное значение из всех графиков
        int chartNumbersCount = 0; // Количество точек на графике

        protected override void OnInitialized(EventArgs e)
        {
            ChangeCanvasCoordinateSystem();
        }

        protected override int VisualChildrenCount => visuals.Count;

        protected override Visual GetVisualChild(int index)
        {
            if (index < 0 || index >= visuals.Count)
            {
                throw new ArgumentOutOfRangeException();
            }

            return visuals[index];
        }

        private void ChangeCanvasCoordinateSystem()
        {
            TransformGroup group = new TransformGroup();
            group.Children.Add(new TranslateTransform(0, -Height));
            group.Children.Add(new ScaleTransform(1, -1));
            RenderTransform = group;
        }

        public void ClearVisual()
        {
            visuals.Clear();
        }

        private void DrawChartNumbers(int coord, in Point coords)
        {
            FormattedText formattedText = new FormattedText(
                $"{coord}",
                CultureInfo.CurrentCulture,
                FlowDirection.LeftToRight,
                new Typeface("Century Gothic"), 12,
                Brushes.Black, 1);

            TransformGroup group = new TransformGroup();
            group.Children.Add(new TranslateTransform(0, -Height));
            group.Children.Add(new ScaleTransform(1, -1));

            DrawingVisual drawingVisual = new DrawingVisual();
            drawingVisual.Transform = group;

            using (DrawingContext drawingContext = drawingVisual.RenderOpen())
            {
                drawingContext.DrawText(formattedText, coords);
            }

            visuals.Add(drawingVisual);
        }

        private void DrawChartAxes()
        {
            DrawingVisual drawingVisual = new DrawingVisual();
            StreamGeometry streamGeometry = new StreamGeometry();
            Pen chartAxesPen = new Pen
            {
                Thickness = 0.2,
                Brush = Brushes.Gray,
                StartLineCap = PenLineCap.Round,
                EndLineCap = PenLineCap.Round
            };

            int xLinesCount = 7;
            int yLinesCount = 10;

            int lineStepX = chartNumbersCount / xLinesCount;
            int lineStepY = (int)(maxChartsValue / yLinesCount);

            if (lineStepX == 0) lineStepX = 1;
            if (lineStepY == 0) lineStepY = 1;

            using (DrawingContext drawingContext = drawingVisual.RenderOpen())
            {
                using (StreamGeometryContext ctx = streamGeometry.Open())
                {
                    for (int i = lineStepX; i < chartNumbersCount; i += lineStepX)
                    {
                        ctx.BeginFigure(new Point(i * xStep, 0), false, false);
                        ctx.LineTo(new Point(i * xStep, Height), true, true);

                        DrawChartNumbers(i, new Point(i * xStep, 0));
                    }

                    for (int i = 0; i < maxChartsValue; i += lineStepY)
                    {
                        ctx.BeginFigure(new Point(0, i * yStep), false, false);
                        ctx.LineTo(new Point(Width, i * yStep), true, true);

                        DrawChartNumbers(lineStepY * yLinesCount - i, new Point(0, i * yStep));
                    }

                    drawingContext.DrawGeometry(null, chartAxesPen, streamGeometry);
                }
            }

            visuals.Add(drawingVisual);
        }

        // Рисование графика без сетки
        public void DrawChart(in List<double> points, double thickness, in Brush chartColor, bool drawAxes = false)
        {
            if (points.Count == 0) return;

            ClearVisual();

            DrawingVisual drawingVisualChart = new DrawingVisual();
            StreamGeometry chartLineGeometry = new StreamGeometry();
            Pen chartLinePen = new Pen
            {
                Thickness = thickness,
                Brush = Brushes.Black,
                StartLineCap = PenLineCap.Round,
                EndLineCap = PenLineCap.Round
            };

            chartLinePen.Brush = chartColor;

            xStep = Width / (points.Count - 1);
            yStep = Height / points.Max();

            using (DrawingContext context = drawingVisualChart.RenderOpen())
            {
                ClearVisual();

                using (StreamGeometryContext ctx = chartLineGeometry.Open())
                {
                    ctx.BeginFigure(new Point(0, points[0] * yStep), true, false);

                    for (int i = 1; i < points.Count; i++)
                    {
                        ctx.LineTo(new Point(i * xStep, points[i] * yStep), true, true);
                    }
                }

                context.DrawGeometry(null, chartLinePen, chartLineGeometry);
            }

            if (drawAxes) DrawChartAxes();

            visuals.Add(drawingVisualChart);
        }


        // Рисование графика с сеткой
        public void DrawChart(in Dictionary<string, List<double>> charts, in Dictionary<string, double> thicknessses, in Dictionary<string, Brush> chartColors, bool drawAxes = false)
        {
            if (charts.Values.First().Count == 0) return;

            foreach (var list in charts)
            {
                if (list.Value.Max() > maxChartsValue) maxChartsValue = list.Value.Max();
                if (list.Value.Count > chartNumbersCount) chartNumbersCount = list.Value.Count();
            }

            xStep = Width / (chartNumbersCount - 1);
            yStep = Height / maxChartsValue;

            compressionYRatio = 1;

            ClearVisual();

            foreach (var chart in charts)
            {
                DrawingVisual drawingVisualChart = new DrawingVisual();
                StreamGeometry chartLineGeometry = new StreamGeometry();
                Pen chartLinePen = new Pen
                {
                    Thickness = thicknessses[chart.Key],
                    Brush = Brushes.Black,
                    StartLineCap = PenLineCap.Round,
                    EndLineCap = PenLineCap.Round
                };

                using (DrawingContext context = drawingVisualChart.RenderOpen())
                {
                    using (StreamGeometryContext ctx = chartLineGeometry.Open())
                    {
                        chartLinePen.Brush = chartColors[chart.Key];

                        ctx.BeginFigure(new Point(margin, (chart.Value[0] * yStep + margin) * compressionYRatio), true, false);

                        for (int i = 1; i < chart.Value.Count; i++)
                        {
                            ctx.LineTo(new Point(i * xStep + margin, (chart.Value[i] * yStep + margin) * compressionYRatio), true, true);
                        }

                        context.DrawGeometry(null, chartLinePen, chartLineGeometry);
                    }

                }

                visuals.Add(drawingVisualChart);
            }

            if (drawAxes) DrawChartAxes();
        }
    }
}