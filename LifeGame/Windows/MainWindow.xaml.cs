using LifeGame.AppData;
using LifeGame.Charting;
using LifeGame.Entities;
using System;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;

namespace LifeGame.Windows
{
    public partial class MainWindow : Window
    {
        Simulation s;
        Chart chart;
        DispatcherTimer timer;
        Data defaultDataConfig;

        public MainWindow()
        {
            InitializeComponent();

            defaultDataConfig = new Data()
            {
                AreaWidth = 64,
                PredatorsCount = 50,
                PreyCount = 4000,
                DeathByOverpopulatingPredator = false,
                DeathByOverpopulatingPrey = false,
                BreedingWith2ParentsPredator = false,
                BreedingWith2ParentsPrey = false,
                MovingIterationsPredator = 1,
                MovingIterationsPrey = 5,
                BreedingIterationsPredatorMin = 10,
                BreedingIterationsPredatorMax = 12,
                BreedingIterationsPreyMin = 5,
                BreedingIterationsPreyMax = 8,
                LifeTimePredatorMin = 30,
                LifeTimePredatorMax = 40,
                LifeTimePreyMin = 15,
                LifeTimePreyMax = 20,
                AmountOfEnergyPredatorMin = 8,
                AmountOfEnergyPredatorMax = 10
            };

            EntityConfig.DataContext = defaultDataConfig;

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

            EntitySettings predatorSettings = new EntitySettings
            {
                BreedWith2Parents = BreedWith2ParentsCheckBox_Predator.IsChecked.GetValueOrDefault(),
                CriticalAmountOfNeighbors = DeathByOverpopulatingCheckBox_Predator.IsChecked ?? false ? CriticalAmountOfNeighborsPredator_NumericBox.Value : 9,
                MovingIterations = MovingIterationsPredator_NumericBox.Value,
                BreedingIterations = (BreedingIterationsPredatorMin_NumericBox.Value, BreedingIterationsPredatorMax_NumericBox.Value),
                LifeTime = (LifeTimePredatorMin_NumericBox.Value, LifeTimePredatorMax_NumericBox.Value),
                AmountOfEnergy = (AmountOfEnergyPredatorMin_NumericBox.Value, AmountOfEnergyPredatorMax_NumericBox.Value)
            };

            EntitySettings preySettings = new EntitySettings
            {
                BreedWith2Parents = BreedWith2ParentsCheckBox_Prey.IsChecked.GetValueOrDefault(),
                CriticalAmountOfNeighbors = DeathByOverpopulatingCheckBox_Prey.IsChecked ?? false ? CriticalAmountOfNeighborsPrey_NumericBox.Value : 9,
                MovingIterations = MovingIterationsPrey_NumericBox.Value,
                BreedingIterations = (BreedingIterationsPreyMin_NumericBox.Value, BreedingIterationsPreyMax_NumericBox.Value),
                LifeTime = (LifeTimePreyMin_NumericBox.Value, LifeTimePreyMax_NumericBox.Value)
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

        private void NumbersTextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
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

        private void preyCountTextBox_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {

        }
    }
}
