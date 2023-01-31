using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Media;

namespace LifeGame.Charting
{
    /*
     *  Класс управления линейными графиками
     */
    public class Chart
    {
        // Словарь графиков
        private Dictionary<string, List<double>> charts = new Dictionary<string, List<double>>();
        // Словарь толщины линий графиков
        private Dictionary<string, double> chartThicknesses = new Dictionary<string, double>();
        // Словарь ограничений количества точек в графике
        private Dictionary<string, int> chartLimits = new Dictionary<string, int>();
        // Словарь цветов линий графиков
        private Dictionary<string, Brush> chartColors = new Dictionary<string, Brush>();

        // Элемент для отображения графиков
        private ChartingElement chartingElement;

        // Проверка на отсутствие точек на графике
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

        // Проверка существования графика
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

        // Рисование одного графика
        public void DrawChart(string chartName, bool drawAxes = false)
        {
            chartingElement.DrawChart(charts[chartName], chartThicknesses[chartName], chartColors[chartName], drawAxes);
        }

        // Рисование всех графиков
        public void DrawAllCharts(bool drawAxes = false)
        {
            chartingElement.DrawChart(charts, chartThicknesses, chartColors, drawAxes);
        }

        // Добавление элемента в указанный график
        public void AddChartElement(string chartName, double value)
        {
            CheckExistenceOfChart(chartName);

            if (charts[chartName].Count >= chartLimits[chartName])
            {
                charts[chartName].RemoveAt(0);
            }

            charts[chartName].Add(value);
        }

        // Очистка точек на всех графиках без их удаления, а также очистка холста
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
