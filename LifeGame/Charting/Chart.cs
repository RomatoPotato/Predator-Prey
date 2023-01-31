using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Media;

namespace LifeGame.Charting
{
    public class Chart
    {
        private Dictionary<string, List<double>> charts = new Dictionary<string, List<double>>();
        private Dictionary<string, double> chartThicknesses = new Dictionary<string, double>();
        private Dictionary<string, int> chartLimits = new Dictionary<string, int>();
        private Dictionary<string, Brush> chartColors = new Dictionary<string, Brush>();

        private ChartingElement chartingElement;

        public bool isChartsEmpty
        {
            get
            {
                foreach (var chart in charts)
                {
                    if (chart.Value.Count == 0) return true;
                }

                return false;
            }
        }

        public Chart(ChartingElement chartingElement)
        {
            this.chartingElement = chartingElement;
        }

        private void CheckExistenceOfChart(string chartName)
        {
            if (!charts.ContainsKey(chartName))
            {
                string errorDescription = $"Chart with name '{chartName}' doesn't exist";
                MessageBox.Show(errorDescription, "Error!", MessageBoxButton.OK, MessageBoxImage.Error);
                throw new Exception(errorDescription);
            }
        }

        public void AddChart(string chartName, int limit, double thickness, Brush color)
        {
            charts.Add(chartName, new List<double>());
            chartLimits.Add(chartName, limit);
            chartThicknesses.Add(chartName, thickness);
            chartColors.Add(chartName, color);
        }

        public void RemoveChart(string chartName)
        {
            CheckExistenceOfChart(chartName);

            charts.Remove(chartName);
            chartLimits.Remove(chartName);
            chartThicknesses.Remove(chartName);
            chartColors.Remove(chartName);
        }

        public double ChartLastElement(string chartName)
        {
            CheckExistenceOfChart(chartName);

            return charts[chartName].Last();
        }

        public void DrawChart(string chartName, bool drawAxes = false)
        {
            chartingElement.DrawChart(charts[chartName], chartThicknesses[chartName], chartColors[chartName], drawAxes);
        }

        public void DrawAllCharts(bool drawAxes = false)
        {
            chartingElement.DrawChart(charts, chartThicknesses, chartColors, drawAxes);
        }

        public void AddChartElement(string chartName, double value)
        {
            CheckExistenceOfChart(chartName);

            if (charts[chartName].Count >= chartLimits[chartName])
            {
                charts[chartName].RemoveAt(0);
            }

            charts[chartName].Add(value);
        }


        public void ClearAllCharts()
        {
            foreach (var chart in charts.Values)
            {
                chart.Clear();
            }

            chartingElement.ClearVisual();
        }
    }
}
