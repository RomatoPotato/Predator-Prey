using LifeGame.PresetSettings;
using LifeGame.Charting;
using LifeGame.Entities;
using Microsoft.Win32;
using System;
using System.IO;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;
using System.Xml.Serialization;
using Excel = Microsoft.Office.Interop.Excel;

namespace LifeGame.Windows
{
    /* 
     *  Главное окно
     */
    public partial class MainWindow : System.Windows.Window
    {
        private Simulation sim;
        private Chart chart;
        private DispatcherTimer timer;
        private EntitiesPreset entitiesPreset;

        private bool isPaused = false;
        private bool isRunning = false;
        private Entity[][] entities;

        public MainWindow()
        {
            InitializeComponent();

            entitiesPreset = new EntitiesPreset()
            {
                AreaWidth = 100,
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
                AmountOfConsumingEnergy = 2,
                HasNoErrors = true
            };

            MainPanel.DataContext = entitiesPreset;

            timer = new DispatcherTimer(DispatcherPriority.Send);
            timer.Tick += DoStep;
            timer.Interval = TimeSpan.FromMilliseconds(100);

            sim = new Simulation(SimulationField);
            SetSimulationData();

            chart = new Chart(ChartingElement);
            chart.AddChart("Predator", 200, 2, Brushes.Black);
            chart.AddChart("Prey", 200, 2, Brushes.Green);
        }

        // Установка свойств сущностей и их расстановка на поле
        private void SetSimulationData(bool isReset = false)
        {
            sim.CellSize = sim.SimulationFieldSize / (double)entitiesPreset.AreaWidth;
            sim.PreysCount = entitiesPreset.PreysCount;
            sim.PredatorsCount = entitiesPreset.PredatorsCount;

            EntityTemplate predatorSettings = new EntityTemplate
            {
                BreedWith2Parents = entitiesPreset.BreedingWith2ParentsPredator,
                CriticalAmountOfNeighbors = entitiesPreset.CriticalAmountOfNeighborsPredator,
                MovingIterations = entitiesPreset.MovingIterationsPredator,
                BreedingIterations = (entitiesPreset.BreedingIterationsPredatorMin, entitiesPreset.BreedingIterationsPredatorMax),
                LifeTime = (entitiesPreset.LifeTimePredatorMin, entitiesPreset.LifeTimePredatorMax),
                AmountOfEnergy = (entitiesPreset.AmountOfEnergyPredatorMin, entitiesPreset.AmountOfEnergyPredatorMax),
                AmountOfConsumingEnergy = entitiesPreset.AmountOfConsumingEnergy
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

            sim.PredatorSettings = predatorSettings;
            sim.PreySettings = preySettings;
            
            if (isReset)
            {
                sim.PlaceEntities(entities);
            }
            else
            {
                sim.PlaceEntities();

                entities = new Entity[sim.Entities.Length][];
                for (int i = 0; i < sim.Entities.Length; i++)
                {
                    entities[i] = new Entity[sim.Entities.Length];

                    for (int j = 0; j < sim.Entities[i].Length; j++)
                    {
                        entities[i][j] = sim.Entities[i][j];
                    }
                }
            }

            timer.Interval = TimeSpan.FromMilliseconds(SimulationSpeedSlider.Value);
        }

        private void NextStep()
        {
            sim.Step(ref chart);

            EntityCountTextBlock.Text = $"Хищников: {chart.ChartLastElement("Predator")}, Жертв: {chart.ChartLastElement("Prey")}";
            IterationCountTextBlock.Text = $"Итераций: {sim.Iterations}";

            CoordsManager.WriteInfoIntoFile(sim.Iterations, (int)chart.ChartLastElement("Predator"), (int)chart.ChartLastElement("Prey"));

            chart.DrawAllCharts();
        }

        private void DoStep(object? sender, EventArgs e)
        {
            NextStep();
        }

        // Запрет на ввод ненужных символов в числовые поля
        private void NumbersTextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }

        private void ResetButton_Click(object sender, RoutedEventArgs e)
        {
            SetSimulationData();
            chart.ClearAllCharts();
            CoordsManager.CreateInfoFile();
        }

        private void Play()
        {
            PlayButton.Content = FindResource("PauseButton");
            PlayButton.ToolTip = "Приостановка симуляции";
            EntityConfigPanel.IsEnabled = false;
            RandomButton.IsEnabled = false;
            StopButton.Visibility = Visibility.Visible;
            ResetButton.Visibility = Visibility.Collapsed;
            timer.Start();
            isPaused = false;
        }

        private void Pause()
        {
            PlayButton.Content = FindResource("PlayButton");
            PlayButton.ToolTip = "Запуск симуляции";
            timer.Stop();
            isPaused = true;
        }

        private void PlayButton_Click(object sender, RoutedEventArgs e)
        {
            if (!isRunning)
            {
                SetSimulationData(true);
                chart.ClearAllCharts();
                CoordsManager.CreateInfoFile();

                isRunning = true;
                isPaused = false;

                Play();
            }
            else
            {
                if (!isPaused)
                {
                    Pause();
                }
                else
                {
                    Play();
                }
            }
            
        }

        private void StopButton_Click(object sender, RoutedEventArgs e)
        {
            timer.Stop();
            PlayButton.Content = FindResource("PlayButton");
            PlayButton.ToolTip = "Запуск симуляции";
            isPaused = false;
            isRunning= false;
            EntityConfigPanel.IsEnabled = true;
            RandomButton.IsEnabled = true;
            StopButton.Visibility = Visibility.Collapsed;
            ResetButton.Visibility = Visibility.Visible;

            SetSimulationData(true);
            chart.ClearAllCharts();
            CoordsManager.CreateInfoFile();
        }

        private void SimulationSpeedSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (timer is not null)
            {
                timer.Interval = TimeSpan.FromMilliseconds(SimulationSpeedSlider.Value);
            }
        }

        private void RandomButton_Click(object sender, RoutedEventArgs e)
        {
            Random random = new Random();

            entitiesPreset.AreaWidth = random.Next(6, 151);
            entitiesPreset.PreysCount = random.Next(1, (int)Math.Pow(entitiesPreset.AreaWidth, 2));
            entitiesPreset.PredatorsCount = random.Next(1, (int)Math.Pow(entitiesPreset.AreaWidth, 2) - entitiesPreset.PreysCount);
            entitiesPreset.DeathByOverpopulationPrey = Convert.ToBoolean(random.Next(0, 2));
            entitiesPreset.DeathByOverpopulationPredator = Convert.ToBoolean(random.Next(0, 2));
            entitiesPreset.BreedingWith2ParentsPrey = Convert.ToBoolean(random.Next(0, 2));
            entitiesPreset.BreedingWith2ParentsPredator = Convert.ToBoolean(random.Next(0, 2));
            entitiesPreset.MovingIterationsPrey = random.Next(1, 100);
            entitiesPreset.MovingIterationsPredator = random.Next(1, 100);
            entitiesPreset.BreedingIterationsPreyMax = random.Next(1, 100);
            entitiesPreset.BreedingIterationsPreyMin = random.Next(1, entitiesPreset.BreedingIterationsPreyMax);
            entitiesPreset.BreedingIterationsPredatorMax = random.Next(1, 100);
            entitiesPreset.BreedingIterationsPredatorMin = random.Next(1, entitiesPreset.BreedingIterationsPredatorMax);
            entitiesPreset.LifeTimePreyMax = random.Next(1, 100);
            entitiesPreset.LifeTimePreyMin = random.Next(1, entitiesPreset.LifeTimePreyMax);
            entitiesPreset.LifeTimePredatorMax = random.Next(1, 100);
            entitiesPreset.LifeTimePredatorMin = random.Next(1, entitiesPreset.LifeTimePredatorMax);
            entitiesPreset.AmountOfEnergyPredatorMax = random.Next(1, 100);
            entitiesPreset.AmountOfEnergyPredatorMin = random.Next(1, entitiesPreset.AmountOfEnergyPredatorMax);

            if (entitiesPreset.DeathByOverpopulationPredator) 
            {
                entitiesPreset.CriticalAmountOfNeighborsPredator = random.Next(1, 9);
            }
            else
            {
                entitiesPreset.CriticalAmountOfNeighborsPredator = 8;
            }

            if (entitiesPreset.DeathByOverpopulationPrey)
            {
                entitiesPreset.CriticalAmountOfNeighborsPrey = random.Next(1, 9);
            }
            else
            {
                entitiesPreset.CriticalAmountOfNeighborsPrey = 8;
            }

            SetSimulationData();
            chart.ClearAllCharts();
            CoordsManager.CreateInfoFile();
        }

        private void CommandBindingOpenChart_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            ChartWindow chartWindow = new ChartWindow();
            chartWindow.Show();
        }

        private void CommandBindingOpenFile_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Файл предустановки симуляции (*.sim)|*.sim| XML-файл (*.xml)|*.xml";
            openFileDialog.InitialDirectory = Environment.CurrentDirectory;
            
            var result = openFileDialog.ShowDialog();

            if (result.HasValue && result.Value)
            {
                string fileName = openFileDialog.FileName;

                XmlSerializer xmlSerializer = new XmlSerializer(typeof(EntitiesPreset));

                using (Stream fStream = new FileStream(fileName, FileMode.Open))
                {
                    EntitiesPreset preset;

                    try
                    {
                        preset = (EntitiesPreset)xmlSerializer.Deserialize(fStream);

                        MessageBoxResult messageBoxResult = MessageBox.Show("Загрузить симуляцию?", "Внимание!", MessageBoxButton.YesNo, MessageBoxImage.Question, MessageBoxResult.Yes);

                        if (messageBoxResult == MessageBoxResult.Yes)
                        {
                            if (preset != null)
                            {
                                entitiesPreset.AreaWidth = preset.AreaWidth;
                                entitiesPreset.PredatorsCount = preset.PredatorsCount;
                                entitiesPreset.PreysCount = preset.PreysCount;
                                entitiesPreset.DeathByOverpopulationPredator = preset.DeathByOverpopulationPredator;
                                entitiesPreset.DeathByOverpopulationPrey = preset.DeathByOverpopulationPrey;
                                entitiesPreset.CriticalAmountOfNeighborsPredator = preset.CriticalAmountOfNeighborsPredator;
                                entitiesPreset.CriticalAmountOfNeighborsPrey = preset.CriticalAmountOfNeighborsPrey;
                                entitiesPreset.BreedingWith2ParentsPrey = preset.BreedingWith2ParentsPrey;
                                entitiesPreset.BreedingWith2ParentsPredator = preset.BreedingWith2ParentsPredator;
                                entitiesPreset.MovingIterationsPredator = preset.MovingIterationsPredator;
                                entitiesPreset.MovingIterationsPrey = preset.MovingIterationsPrey;
                                entitiesPreset.BreedingIterationsPredatorMin = preset.BreedingIterationsPredatorMin;
                                entitiesPreset.BreedingIterationsPredatorMax = preset.BreedingIterationsPredatorMax;
                                entitiesPreset.BreedingIterationsPreyMin = preset.BreedingIterationsPreyMin;
                                entitiesPreset.BreedingIterationsPreyMax = preset.BreedingIterationsPreyMax;
                                entitiesPreset.LifeTimePredatorMin = preset.LifeTimePredatorMin;
                                entitiesPreset.LifeTimePredatorMax = preset.LifeTimePredatorMax;
                                entitiesPreset.LifeTimePreyMin = preset.LifeTimePreyMin;
                                entitiesPreset.LifeTimePreyMax = preset.LifeTimePreyMax;
                                entitiesPreset.AmountOfEnergyPredatorMax = preset.AmountOfEnergyPredatorMax;
                                entitiesPreset.AmountOfEnergyPredatorMin = preset.AmountOfEnergyPredatorMin;

                                SetSimulationData();
                                chart.ClearAllCharts();
                                CoordsManager.CreateInfoFile();
                            }
                            else
                            {
                                throw new InvalidOperationException("БЛЯТЬ");
                            }
                        }
                    }
                    catch (InvalidOperationException)
                    {
                        MessageBox.Show("Ошибка загрузки симуляции!", "Ошибка!", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
        }

        private void CommandBindingSaveFile_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "Файл предустановки симуляции (*.sim)|*.sim| XML-файл (*.xml)|*.xml";
            saveFileDialog.InitialDirectory = Environment.CurrentDirectory;

            var result = saveFileDialog.ShowDialog();

            if (result.HasValue && result.Value)
            {
                string fileName = saveFileDialog.FileName;

                XmlSerializer xmlSerializer = new XmlSerializer(typeof(EntitiesPreset));

                using (Stream fStream = new FileStream(fileName, FileMode.Create))
                {
                    xmlSerializer.Serialize(fStream, entitiesPreset);
                }
            }
        }

        private void CommandBindingOpenChart_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = !chart.isChartsEmpty && isPaused;
        }

        private void CommandBindingReturnDefault_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = !isRunning;
        }

        private void CommandBindingExportToExcel_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            var excelApp = new Excel.Application();
            excelApp.Visible = true;
            excelApp.Workbooks.Add();

            Excel._Worksheet worksheet = (Excel._Worksheet)excelApp.ActiveSheet;
            worksheet.Cells[1, "A"] = "Итерация";
            worksheet.Cells[1, "B"] = "Количество хищников";
            worksheet.Cells[1, "C"] = "Количество жертв";

            var entitiesInfo = CoordsManager.ReadInfoFromFile();

            for (int i = 1; i < entitiesInfo.numberAndPrey.Count; i++)
            {
                worksheet.Cells[i + 1, "A"] = i;
                worksheet.Cells[i + 1, "B"] = entitiesInfo.numberAndPredator[i];
                worksheet.Cells[i + 1, "C"] = entitiesInfo.numberAndPrey[i];
            }

            worksheet.Columns.AutoFit();

            var charts = worksheet.ChartObjects() as Excel.ChartObjects;
            var chartObject = charts.Add(280, 20, 500, 250);
            var excelChart = chartObject.Chart;
            var range = worksheet.get_Range("A1", "C" + entitiesInfo.numberAndPrey.Count.ToString());
            excelChart.SetSourceData(range);
            excelChart.ChartType = Excel.XlChartType.xlXYScatterSmoothNoMarkers;
            excelChart.ChartWizard(Source: range, Title: "Изменение численности популяций со временем", CategoryTitle: "Итерация", ValueTitle: "Количество");
        }

        private void CommandBindingClose_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            Close();
        }

        private void NumericBox_ValueChanged(object sender, EventArgs e)
        {
            SetSimulationData();
        }

        private void CheckBox_Clicked(object sender, RoutedEventArgs e)
        {
            SetSimulationData();
        }

        private void TextBox_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            SetSimulationData();
        }

        private void fieldSizeSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (sim is not null) SetSimulationData();
        }

        private void CommandBindingReturnDefault_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            entitiesPreset.AreaWidth = 100;
            entitiesPreset.PredatorsCount = 50;
            entitiesPreset.PreysCount = 4000;
            entitiesPreset.CriticalAmountOfNeighborsPredator = 8;
            entitiesPreset.CriticalAmountOfNeighborsPrey = 8;
            entitiesPreset.DeathByOverpopulationPredator = false;
            entitiesPreset.DeathByOverpopulationPrey = false;
            entitiesPreset.BreedingWith2ParentsPredator = false;
            entitiesPreset.BreedingWith2ParentsPrey = false;
            entitiesPreset.MovingIterationsPredator = 1;
            entitiesPreset.MovingIterationsPrey = 5;
            entitiesPreset.BreedingIterationsPredatorMin = 10;
            entitiesPreset.BreedingIterationsPredatorMax = 12;
            entitiesPreset.BreedingIterationsPreyMin = 5;
            entitiesPreset.BreedingIterationsPreyMax = 8;
            entitiesPreset.LifeTimePredatorMin = 30;
            entitiesPreset.LifeTimePredatorMax = 40;
            entitiesPreset.LifeTimePreyMin = 15;
            entitiesPreset.LifeTimePreyMax = 20;
            entitiesPreset.AmountOfEnergyPredatorMin = 8;
            entitiesPreset.AmountOfEnergyPredatorMax = 10;
            entitiesPreset.AmountOfConsumingEnergy = 2;

            SetSimulationData();
        }

        private void CommandBindingInfo_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            MessageBox.Show("Разработчик: RomatoPotato\nНа этом всё", "Инфо", MessageBoxButton.OK, MessageBoxImage.Information);
        }
    }
}
