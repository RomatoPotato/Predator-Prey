using LifeGame.Charting;
using LifeGame.Entities;
using System;
using System.IO;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Media;
using System.Windows.Threading;

namespace LifeGame.Windows
{
    public partial class MainWindow : Window
    {
        Simulation s;
        Chart chart;
        DispatcherTimer timer;

        public MainWindow()
        {
            InitializeComponent();

            s = new Simulation(SimulationField);
            SetSimulationData();
            s.PlaceEntities();

            chart = new Chart(ChartingElement);
            chart.AddChart("Predator", 200, 2, Brushes.Black);
            chart.AddChart("Prey", 200, 2, Brushes.Green);

            timer = new DispatcherTimer(DispatcherPriority.Send);
            timer.Tick += DoStep;
            timer.Interval = TimeSpan.FromMilliseconds(100);
        }

        private void SetSimulationData()
        {
            s.CellSize = s.SimulationFieldSize / (int)fieldSizeSlider.Value;
            s.PreyCount = preyCountTextBox.Text == string.Empty ? 0 : int.Parse(preyCountTextBox.Text);
            s.PredatorCount = predatorCountTextBox.Text == string.Empty ? 0 : int.Parse(predatorCountTextBox.Text);

            Predator predatorSettings = new Predator
            {
                BreedWith2Parents = BreedWith2ParentsCheckBox_Predator.IsChecked.GetValueOrDefault(),
                CriticalAmountOfNeighbors = DeathByOverpopulatingCheckBox_Predator.IsChecked ?? false ? int.Parse(CriticalAmountOfNeighborsTextBox_Predator.Text) : 9,
                MovingIterations = int.Parse(MovingIterationsTextBox_Predator.Text),
                BreedingIterations = int.Parse(BreedingIterationsTextBox_Predator.Text),
                LifeTime = int.Parse(LifeTimeTextBox_Predator.Text),
                AmountOfEnergy = int.Parse(AmountOfEnergyTextBox_Predator.Text),
            };

            Prey preySettings = new Prey
            {
                BreedWith2Parents = BreedWith2ParentsCheckBox_Prey.IsChecked.GetValueOrDefault(),
                CriticalAmountOfNeighbors = DeathByOverpopulatingCheckBox_Prey.IsChecked ?? false ? int.Parse(CriticalAmountOfNeighborsTextBox_Prey.Text) : 9,
                MovingIterations = int.Parse(MovingIterationsTextBox_Prey.Text),
                BreedingIterations = int.Parse(BreedingIterationsTextBox_Prey.Text),
                LifeTime = int.Parse(LifeTimeTextBox_Prey.Text),
            };

            s.PredatorSettings = predatorSettings;
            s.PreySettings = preySettings;
        }

        private void FillFieldButton_Click(object sender, RoutedEventArgs e)
        {
            if (timer.IsEnabled) timer.Stop();

            SetSimulationData();

            if (Math.Pow(fieldSizeSlider.Value, 2) < s.PreyCount + s.PredatorCount)
            {
                if (timer.IsEnabled) timer.Stop();

                MessageBox.Show("Общее количество хищников и жертв превышает размер поля!", null, MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            chart.ClearAllCharts();
            CoordsManager.CreateInfoFile();

            s.PlaceEntities();
        }

        private void NextStep()
        {
            s.Step(ref chart);

            EntityCountTextBlock.Text = $"Хищников: {chart.ChartLastElement("Predator")}, Жертв: {chart.ChartLastElement("Prey")}";
            IterationCountTextBlock.Text = $"Итераций: {s.Iterations}";

            CoordsManager.WriteInfoIntoFile(s.Iterations, (int)chart.ChartLastElement("Predator"), (int)chart.ChartLastElement("Prey"));

            chart.DrawAllCharts();
        }

        private void DoStep(object? sender, EventArgs e)
        {
            NextStep();
        }

        private void MoveButton_Click(object sender, RoutedEventArgs e)
        {
            NextStep();
        }

        private void NumbersTextBox_PreviewTextInput(object sender, System.Windows.Input.TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }

        private void PlayButton_Click(object sender, RoutedEventArgs e)
        {
            timer.Start();
        }

        private void StopButton_Click(object sender, RoutedEventArgs e)
        {
            timer.Stop();
        }

        private void CommandBinding_Executed(object sender, System.Windows.Input.ExecutedRoutedEventArgs e)
        {
            ChartWindow chartWindow = new ChartWindow();
            chartWindow.Show();
        }
    }
}
