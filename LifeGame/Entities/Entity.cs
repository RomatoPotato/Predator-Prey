using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Media;

namespace LifeGame.Entities
{
    /*
     *  Главный класс для всех сущностей
     */
    public abstract class Entity
    {
        public abstract SolidColorBrush Color { get; set; }

        public int LifeTime { get; set; }
        public int BreedingIterations { get; set; }
        public double AmountOfEnergy { get; set; }
        public double AmountOfConsumingEnergy { get; set; }
        public int MovingIterations { get; set; }
        public int CriticalAmountOfNeighbors { get; set; }
        public bool BreedWith2Parents { get; set; }

        public bool IsBorn { get; set; } = false;
        public bool IsGaveBirth { get; set; } = false;
        public bool IsMoved { get; set; } = false;
        public bool IsEaten { get; set; } = false;

        private Random settingsRandom = new Random();

        // Проверка на совершённое сущностью действие
        public bool IsActed()
        {
            return IsBorn || IsEaten || IsGaveBirth || IsMoved;
        }

        // Индексы 8 возможных соседей соседей сущности
        protected HashSet<(int x, int y)> neighborsIndexes = new HashSet<(int, int)>();

        // Предустановки свойств сущности
        public EntityTemplate EntitySettings { get; set; }

        public Entity(EntityTemplate settings)
        {
            CriticalAmountOfNeighbors = settings.CriticalAmountOfNeighbors;
            BreedWith2Parents = settings.BreedWith2Parents;
            MovingIterations = settings.MovingIterations;
            BreedingIterations = settingsRandom.Next(settings.BreedingIterations.Min, settings.BreedingIterations.Max);
            LifeTime = settingsRandom.Next(settings.LifeTime.Min, settings.LifeTime.Max);
            AmountOfEnergy = settingsRandom.Next(settings.AmountOfEnergy.Min, settings.AmountOfEnergy.Max);
            AmountOfConsumingEnergy = settings.AmountOfConsumingEnergy;
        }

        public Entity()
        {

        }

        // Нахождение соседей сущности
        protected void SetNeigborIndexes(Entity[][] entities, int x, int y)
        {
            int left = Math.Max(x - 1, 0);
            int top = Math.Max(y - 1, 0);
            int right = Math.Min(x + 1, entities.Length - 1);
            int bottom = Math.Min(y + 1, entities[x].Length - 1);
            int middleX = x;
            int middleY = y;

            neighborsIndexes.Clear();

            if (left != x || top != y)
                neighborsIndexes.Add((left, top));

            if (top != y)
                neighborsIndexes.Add((middleX, top));

            if (right != x || top != y)
                neighborsIndexes.Add((right, top));

            if (left != x)
                neighborsIndexes.Add((left, middleY));

            if (right != x)
                neighborsIndexes.Add((right, middleY));

            if (left != x || bottom != y)
                neighborsIndexes.Add((left, bottom));

            if (bottom != y)
                neighborsIndexes.Add((middleX, bottom));

            if (right != x || bottom != y)
                neighborsIndexes.Add((right, bottom));
        }
        
        // Поиск соседей-союзников
        public abstract HashSet<(int, int)> FindRelativesCells(int x, int y, Entity[][] entities);
        // Поиск соседей-противников
        public abstract HashSet<(int, int)> FindOppositeCells(int x, int y, Entity[][] entities);
        // Поис свободного пространства вокруг 
        public virtual HashSet<(int, int)> FindClearCells(int x, int y, Entity[][] entities)
        {
            SetNeigborIndexes(entities, x, y);

            return (from a in neighborsIndexes where entities[a.x][a.y] is null select a).ToHashSet<(int, int)>();
        }
         
        // Метод для размножения сущностей
        public abstract void Breed(ref Entity[][] entities, int x, int y);

        // Метод для поедания сущностью других
        public abstract void Consume(ref Entity[][] entities, int x, int y);

        // Метод для движения сущности
        public void Move(ref Entity[][] entities, int x, int y)
        {
            HashSet<(int x, int y)> clearCells = FindClearCells(x, y, entities);

            if (clearCells.Count != 0 && !IsActed())
            {
                Random r = new Random();
                int n = r.Next(clearCells.Count);

                IsMoved = true;
                entities[clearCells.ToArray()[n].x][clearCells.ToArray()[n].y] = this;
                entities[x][y] = null;
            }
            else if (clearCells.Count == 0)
            {
                entities[x][y] = null;
            }
        }

        // Проверка на возможность смерти
        public void DeathCheck(ref Entity[][] entities, int x, int y)
        {
            HashSet<(int x, int y)> clearCells = FindClearCells(x, y, entities);
            --LifeTime;
            --AmountOfEnergy;

            if (LifeTime <= 0 || neighborsIndexes.Count - clearCells.Count >= CriticalAmountOfNeighbors + 1)
            {
                if (entities[x][y] is Predator && AmountOfEnergy <= 0)
                {
                    entities[x][y] = null;
                }

                entities[x][y] = null;
            }
        }
    }
}
