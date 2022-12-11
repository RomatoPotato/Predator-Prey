using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Media;

namespace LifeGame.Entities
{
    internal class Prey : Entity
    {
        public Prey(EntitySettings settings) : base(settings)
        {
            EntitySettings = settings;
        }

        public Prey() { }

        public override SolidColorBrush Color { get; set; } = Brushes.Green;

        public override void Consume(ref Entity[][] entities, int x, int y) { }

        public override HashSet<(int, int)> FindOppositeCells(int x, int y, Entity[][] entities)
        {
            SetNeigborIndexes(entities, x, y);

            return (from a in neighborsIndexes where entities[a.x][a.y] is Predator select a).ToHashSet<(int, int)>();
        }

        public override HashSet<(int, int)> FindRelativesCells(int x, int y, Entity[][] entities)
        {
            SetNeigborIndexes(entities, x, y);

            return (from a in neighborsIndexes where entities[a.x][a.y] is Prey select a).ToHashSet<(int, int)>();
        }

        public override void Breed(ref Entity[][] entities, int x, int y)
        {
            HashSet<(int x, int y)> clearCells = FindClearCells(x, y, entities);

            if (BreedWith2Parents)
            {
                HashSet<(int x, int y)> relativesCells = FindRelativesCells(x, y, entities);
                List<(int x, int y)> suitablePreys = new List<(int, int)>();

                if (relativesCells.Count >= 1 && relativesCells.Count < 4 && clearCells.Count >= 1 && !IsActed())
                {
                    for (int i = 0; i < relativesCells.Count; i++)
                    {
                        (int x, int y) index = (relativesCells.ToArray()[i].x, relativesCells.ToArray()[i].y);
                        Prey choosedPrey = (Prey)entities[index.x][index.y];

                        if (!choosedPrey.IsGaveBirth && !choosedPrey.IsBorn && !choosedPrey.IsMoved && !choosedPrey.IsEaten)
                        {
                            suitablePreys.Add(index);
                        }
                    }

                    if (suitablePreys.Count != 0)
                    {
                        Random r = new Random();
                        int clearCellIndex = r.Next(clearCells.Count);
                        int giveBirthPreyIndex = r.Next(suitablePreys.Count);

                        IsGaveBirth = true;
                        entities[clearCells.ToArray()[clearCellIndex].x][clearCells.ToArray()[clearCellIndex].y] = new Prey(EntitySettings) { IsBorn = true };
                        entities[suitablePreys[giveBirthPreyIndex].x][suitablePreys[giveBirthPreyIndex].y].IsGaveBirth = true;
                    }
                }
            }
            else
            {
                if (clearCells.Count >= 1 && !IsActed())
                {
                    Random r = new Random();
                    int clearCellIndex = r.Next(clearCells.Count);

                    IsGaveBirth = true;
                    entities[clearCells.ToArray()[clearCellIndex].x][clearCells.ToArray()[clearCellIndex].y] = new Prey(EntitySettings) { IsBorn = true };
                }
            }
        }
    }
}
