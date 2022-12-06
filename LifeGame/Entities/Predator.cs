using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Media;

namespace LifeGame.Entities
{
    internal class Predator : Entity
    {
        public Predator(Entity settings) : base(settings)
        {
            EntitySettings = settings;
        }

        public Predator() { }

        public override SolidColorBrush Color { get; set; } = Brushes.Black;

        public override HashSet<(int, int)> FindOppositeCells(int x, int y, Entity[][] entities)
        {
            SetNeigborIndexes(entities, x, y);

            return (from a in neighborsIndexes where entities[a.x][a.y] is Prey select a).ToHashSet<(int, int)>();
        }

        public override HashSet<(int, int)> FindRelativesCells(int x, int y, Entity[][] entities)
        {
            SetNeigborIndexes(entities, x, y);

            return (from a in neighborsIndexes where entities[a.x][a.y] is Predator select a).ToHashSet<(int, int)>();
        }

        public override void Consume(ref Entity[][] entities, int x, int y)
        {
            HashSet<(int x, int y)> oppositeCells = FindOppositeCells(x, y, entities);

            if (oppositeCells.Count >= 1 && !IsActed())
            {
                Random r = new Random();
                int consumePreyIndex = r.Next(oppositeCells.Count);

                Prey choosedPrey = (Prey)entities[oppositeCells.ToArray()[consumePreyIndex].x][oppositeCells.ToArray()[consumePreyIndex].y];

                if (!choosedPrey.IsBorn && !choosedPrey.IsMoved)
                {
                    AmountOfEnergy += 2;
                    IsEaten = true;

                    entities[oppositeCells.ToArray()[consumePreyIndex].x][oppositeCells.ToArray()[consumePreyIndex].y] = null;
                    //entities[x][y] = null;
                }
            }
        }

        public override void Breed(ref Entity[][] entities, int x, int y)
        {
            HashSet<(int x, int y)> clearCells = FindClearCells(x, y, entities);

            if (BreedWith2Parents)
            {
                List<(int x, int y)> suitablePredators = new List<(int, int)>();
                HashSet<(int x, int y)> relativesCells = FindRelativesCells(x, y, entities);

                if (relativesCells.Count >= 1 && relativesCells.Count < 4 && clearCells.Count >= 1 && !IsActed())
                {
                    for (int i = 0; i < relativesCells.Count; i++)
                    {
                        (int x, int y) index = (relativesCells.ToArray()[i].x, relativesCells.ToArray()[i].y);
                        Predator choosedPredator = (Predator)entities[index.x][index.y];

                        if (!choosedPredator.IsGaveBirth && !choosedPredator.IsBorn && !choosedPredator.IsMoved && !choosedPredator.IsEaten)
                        {
                            suitablePredators.Add(index);
                        }
                    }

                    if (suitablePredators.Count != 0)
                    {
                        Random r = new Random();
                        int clearCellIndex = r.Next(clearCells.Count);
                        int giveBirthPredatorIndex = r.Next(suitablePredators.Count);

                        IsGaveBirth = true;
                        entities[clearCells.ToArray()[clearCellIndex].x][clearCells.ToArray()[clearCellIndex].y] = new Predator(EntitySettings) { IsBorn = true };
                        entities[suitablePredators[giveBirthPredatorIndex].x][suitablePredators[giveBirthPredatorIndex].y].IsGaveBirth = true;
                    }
                }
            }
            else
            {
                if (clearCells.Count >= 1 && !IsActed() && AmountOfEnergy > 3)
                {
                    Random r = new Random();
                    int clearCellIndex = r.Next(clearCells.Count);

                    IsGaveBirth = true;
                    entities[clearCells.ToArray()[clearCellIndex].x][clearCells.ToArray()[clearCellIndex].y] = new Predator(EntitySettings) { IsBorn = true };
                }
            }
        }
    }
}
