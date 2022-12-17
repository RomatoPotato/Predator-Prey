using LifeGame.Charting;
using LifeGame.Entities;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace LifeGame
{
    internal class Simulation
    {
        public int Iterations { get; set; } = 0;

        private Entity[][] entities;
        private Image simulationField;

        public int SimulationFieldSize { get; set; } = 600;
        public double CellSize { get; set; }
        public int PreysCount { get; set; }
        public int PredatorsCount { get; set; }
        public EntityTemplate PredatorSettings { get; set; }
        public EntityTemplate PreySettings { get; set; }

        public Simulation(Image simulationField)
        {
            this.simulationField = simulationField;
        }

        public void PlaceEntities()
        {
            entities = new Entity[(int)(SimulationFieldSize / CellSize)][];
            Iterations = 0;

            int predators = PredatorsCount;
            int preys = PreysCount;

            for (int i = 0; i < entities.Length; i++)
            {
                entities[i] = new Entity[entities.Length];

                for (int j = 0; j < entities[i].Length; j++)
                {
                    if (predators != 0)
                    {
                        entities[i][j] = new Predator(PredatorSettings);
                        predators--;
                    }
                    else if (preys != 0)
                    {
                        entities[i][j] = new Prey(PreySettings);
                        preys--;
                    }
                }
            }

            Random random = new Random();

            for (int i = entities.Length - 1; i >= 0; i--)
            {
                for (int j = entities[i].Length - 1; j >= 0; j--)
                {
                    (int x, int y) = (random.Next(i + 1), random.Next(j + 1));

                    Entity temp = entities[x][y];
                    entities[x][y] = entities[j][i];
                    entities[j][i] = temp;
                }
            }

            DrawSimulation();
        }

        public void Step(ref Chart chart)
        {
            Iterations++;

            (int predator, int prey) entitiesCount = (0, 0);

            for (int x = 0; x < entities.Length; x++)
            {
                for (int y = 0; y < entities[x].Length; y++)
                {
                    Entity currentEntity = entities[x][y];

                    if (currentEntity is not null)
                    {
                        currentEntity.Consume(ref entities, x, y);

                        if (Iterations % currentEntity.BreedingIterations == 0)
                        {
                            currentEntity.Breed(ref entities, x, y);
                        }

                        if (Iterations % currentEntity.MovingIterations == 0)
                        {
                            currentEntity.Move(ref entities, x, y);
                        }
                    }
                }
            }

            for (int x = 0; x < entities.Length; x++)
            {
                for (int y = 0; y < entities[x].Length; y++)
                {
                    Entity currentEntity = entities[x][y];

                    if (currentEntity is not null)
                    {
                        currentEntity.IsBorn = false;
                        currentEntity.IsGaveBirth = false;
                        currentEntity.IsMoved = false;
                        currentEntity.IsEaten = false;

                        currentEntity.DeathCheck(ref entities, x, y);

                        //if (iterations % currentEntity.MovingIterations == 0)
                        //{
                        //    if (--currentEntity.LifeTime <= 0)
                        //    {
                        //        entities[x][y] = null;
                        //    }
                        //}
                    }
                }
            }

            for (int x = 0; x < entities.Length; x++)
            {
                for (int y = 0; y < entities[x].Length; y++)
                {
                    if (entities[x][y] is Predator) entitiesCount.predator++;
                    if (entities[x][y] is Prey) entitiesCount.prey++;
                }
            }

            chart.AddChartElement("Predator", entitiesCount.predator);
            chart.AddChartElement("Prey", entitiesCount.prey);

            DrawSimulation();
        }

        private void DrawSimulation()
        {
            DrawingVisual drawingVisual = new DrawingVisual();

            using (DrawingContext drawingContext = drawingVisual.RenderOpen())
            {
                double padding = (SimulationFieldSize - entities.Length * CellSize) / 2;
                drawingContext.DrawRectangle(Brushes.White, null, new Rect(padding, padding, SimulationFieldSize - padding * 2.5, SimulationFieldSize - padding * 2.5));

                for (int x = 0; x < entities.Length; x++)
                {
                    for (int y = 0; y < entities[x].Length; y++)
                    {
                        drawingContext.DrawRectangle(entities[x][y]?.Color, null, new Rect(x * CellSize + padding + 0.1, y * CellSize + padding + 0.1, CellSize - 0.2, CellSize - 0.2));
                    }
                }
            }

            var bmp = new RenderTargetBitmap(SimulationFieldSize, SimulationFieldSize, 0, 0, PixelFormats.Pbgra32);
            bmp.Render(drawingVisual);

            simulationField.Width = entities.Length * CellSize;
            simulationField.Height = entities.Length * CellSize;
            simulationField.Source = bmp;
        }
    }
}
