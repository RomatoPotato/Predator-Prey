﻿using LifeGame.Charting;
using LifeGame.Entities;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace LifeGame
{
    /* 
     *  Управление симуляцией
     */
    internal class Simulation
    {
        // Кол-во итераций
        public int Iterations { get; private set; } = 0;
        // Размер поля в пикселях
        public int SimulationFieldSize { get; } = 600;
        // Размер ячейки
        public double CellSize { get; set; }

        // Количество сущностей
        public int PreysCount { get; set; }
        public int PredatorsCount { get; set; }

        // Свойства сущностей
        public EntityTemplate PredatorSettings { get; set; }
        public EntityTemplate PreySettings { get; set; }

        private Entity[][] entities;

        public Entity[][] Entities
        {
            get { return entities; }
            private set { entities = value; }
        }

        private Image simulationField;

        public Simulation(Image simulationField)
        {
            this.simulationField = simulationField;
        }

        // Расстановка сущностей на поле
        public void PlaceEntities()
        {
            Entities = new Entity[(int)(SimulationFieldSize / CellSize)][];
            Iterations = 0;

            int predators = PredatorsCount;
            int preys = PreysCount;

            for (int i = 0; i < Entities.Length; i++)
            {
                Entities[i] = new Entity[Entities.Length];

                for (int j = 0; j < Entities[i].Length; j++)
                {
                    if (predators != 0)
                    {
                        Entities[i][j] = new Predator(PredatorSettings);
                        predators--;
                    }
                    else if (preys != 0)
                    {
                        Entities[i][j] = new Prey(PreySettings);
                        preys--;
                    }
                }
            }

            Random random = new Random();

            for (int i = Entities.Length - 1; i >= 0; i--)
            {
                for (int j = Entities[i].Length - 1; j >= 0; j--)
                {
                    (int x, int y) = (random.Next(i + 1), random.Next(j + 1));

                    Entity temp = Entities[x][y];
                    Entities[x][y] = Entities[j][i];
                    Entities[j][i] = temp;
                }
            }

            DrawSimulation();
        }


        // Расстановка сущностей на поле на указанные места
        public void PlaceEntities(Entity[][] entitiesArray)
        {
            Iterations = 0;
            Entities = new Entity[entitiesArray.Length][];

            for (int i = 0; i < entitiesArray.Length; i++)
            {
                Entities[i] = new Entity[entitiesArray.Length];

                for (int j = 0; j < entitiesArray[i].Length; j++)
                {
                    if (entitiesArray[i][j] is Predator)
                    {
                        Entities[i][j] = new Predator(PredatorSettings);
                    }
                    else if (entitiesArray[i][j] is Prey)
                    {
                        Entities[i][j] = new Prey(PreySettings);
                    }
                }
            }

            DrawSimulation();
        }

        // Один проход по всем сущностям
        public void Step(ref Chart chart)
        {
            Iterations++;

            (int predator, int prey) entitiesCount = (0, 0);

            for (int x = 0; x < Entities.Length; x++)
            {
                for (int y = 0; y < Entities[x].Length; y++)
                {
                    // Сначала поедание
                    Entities[x][y]?.Consume(ref entities, x, y);

                    if (Iterations % Entities[x][y]?.BreedingIterations == 0)
                    {
                        // Затем размножение
                        Entities[x][y]?.Breed(ref entities, x, y);
                    }

                    if (Iterations % Entities[x][y]?.MovingIterations == 0)
                    {
                        // И движение
                        Entities[x][y]?.Move(ref entities, x, y);
                    }
                }
            }

            for (int x = 0; x < Entities.Length; x++)
            {
                for (int y = 0; y < Entities[x].Length; y++)
                {
                    if (Entities[x][y] is not null)
                    {
                        // Сброс флагов активностей
                        Entities[x][y].IsBorn = false;
                        Entities[x][y].IsGaveBirth = false;
                        Entities[x][y].IsMoved = false;
                        Entities[x][y].IsEaten = false;

                        // Проверка на возможнсть смерти
                        Entities[x][y].DeathCheck(ref entities, x, y);
                    }
                }
            }

            // Подсчёт оставшихся сущностей
            for (int x = 0; x < Entities.Length; x++)
            {
                for (int y = 0; y < Entities[x].Length; y++)
                {
                    if (Entities[x][y] is Predator) entitiesCount.predator++;
                    if (Entities[x][y] is Prey) entitiesCount.prey++;
                }
            }

            // Заполнение графиков
            chart.AddChartElement("Predator", entitiesCount.predator);
            chart.AddChartElement("Prey", entitiesCount.prey);

            DrawSimulation();
        }

        // Отрисовка симуляции
        private void DrawSimulation()
        {
            DrawingVisual drawingVisual = new DrawingVisual();

            using (DrawingContext drawingContext = drawingVisual.RenderOpen())
            {
                double padding = (SimulationFieldSize - Entities.Length * CellSize) / 2;
                drawingContext.DrawRectangle(Brushes.White, null, new Rect(padding, padding, SimulationFieldSize - padding * 2.5, SimulationFieldSize - padding * 2.5));

                for (int x = 0; x < Entities.Length; x++)
                {
                    for (int y = 0; y < Entities[x].Length; y++)
                    {
                        drawingContext.DrawRectangle(Entities[x][y]?.Color, null, new Rect(x * CellSize + padding + 0.1, y * CellSize + padding + 0.1, CellSize - 0.2, CellSize - 0.2));
                    }
                }
            }

            var bmp = new RenderTargetBitmap(SimulationFieldSize, SimulationFieldSize, 0, 0, PixelFormats.Pbgra32);
            bmp.Render(drawingVisual);

            simulationField.Width = Entities.Length * CellSize;
            simulationField.Height = Entities.Length * CellSize;
            simulationField.Source = bmp;
        }
    }
}
