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
        EntitiesPreset entitiesPreset;

        public MainWindow()
        {
            InitializeComponent();

            entitiesPreset = new EntitiesPreset()
            {
                AreaWidth = 64,
                PredatorsCount = 50,
                PreysCount = 4000,
                CriticalAmountOfNeighborsPredator = 8,
                CriticalAmountOfNeighborsPrey = 8,
                DeathByOverpopulationPredator = false,
                DeathByOverpopulationPrey = false,
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
                AmountOfEnergyPredatorMax = 10,
                HasNoErrors = true
            };

            MainPanel.DataContext = entitiesPreset;

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
            s.CellSize = s.SimulationFieldSize / (double)entitiesPreset.AreaWidth;
            s.PreysCount = entitiesPreset.PreysCount;
            s.PredatorsCount = entitiesPreset.PredatorsCount;

            EntityTemplate predatorSettings = new EntityTemplate
            {
                BreedWith2Parents = entitiesPreset.BreedingWith2ParentsPredator,
                CriticalAmountOfNeighbors = entitiesPreset.CriticalAmountOfNeighborsPredator,
                MovingIterations = entitiesPreset.MovingIterationsPredator,
                BreedingIterations = (entitiesPreset.BreedingIterationsPredatorMin, entitiesPreset.BreedingIterationsPredatorMax),
                LifeTime = (entitiesPreset.LifeTimePredatorMin, entitiesPreset.LifeTimePredatorMax),
                AmountOfEnergy = (entitiesPreset.AmountOfEnergyPredatorMin, entitiesPreset.AmountOfEnergyPredatorMax)
            };

            EntityTemplate preySettings = new EntityTemplate
            {
                BreedWith2Parents = entitiesPreset.BreedingWith2ParentsPrey,
                CriticalAmountOfNeighbors = entitiesPreset.CriticalAmountOfNeighborsPrey,
                MovingIterations = entitiesPreset.MovingIterationsPrey,
                BreedingIterations = (entitiesPreset.BreedingIterationsPreyMin, entitiesPreset.BreedingIterationsPreyMax),
                LifeTime = (entitiesPreset.LifeTimePreyMin, entitiesPreset.LifeTimePreyMax)
            };

            if (!DeathByOverpopulatingCheckBox_Predator.IsChecked ?? default) predatorSettings.CriticalAmountOfNeighbors = 8;
            if (!DeathByOverpopulatingCheckBox_Prey.IsChecked ?? default) preySettings.CriticalAmountOfNeighbors = 8;

            s.PredatorSettings = predatorSettings;
            s.PreySettings = preySettings;
        }

        private void FillFieldButton_Click(object sender, RoutedEventArgs e)
        {
            if (timer.IsEnabled) timer.Stop();

            SetSimulationData();

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

            EntityConfigPanel.IsEnabled = false;
        }

        private void StopButton_Click(object sender, RoutedEventArgs e)
        {
            timer.Stop();

            EntityConfigPanel.IsEnabled = true;
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
