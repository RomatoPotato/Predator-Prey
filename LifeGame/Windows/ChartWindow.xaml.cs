using LifeGame.Charting;
using System;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Media;

namespace LifeGame.Windows
{
    /*
     *  Окно для отображения графика
     */
    public partial class ChartWindow : Window
    {
        Chart chart;

        public ChartWindow()
        {
            InitializeComponent();

            var entitiesInfo = CoordsManager.ReadInfoFromFile();
            chart = new Chart(ChartingElement);

            chart.AddChart("Predator", entitiesInfo.numberAndPredator.Values.Count, 2, Brushes.Black);
            chart.AddChart("Prey", entitiesInfo.numberAndPrey.Values.Count, 2, Brushes.Green);

            for (int i = 1; i <= entitiesInfo.numberAndPredator.Keys.Count; i++)
            {
                chart.AddChartElement("Predator", entitiesInfo.numberAndPredator[i]);
                chart.AddChartElement("Prey", entitiesInfo.numberAndPrey[i]);
            }

            chart.DrawAllCharts(true);

        }
    }
}
