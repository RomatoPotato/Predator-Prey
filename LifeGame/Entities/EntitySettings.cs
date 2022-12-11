using System.Windows.Media;

namespace LifeGame.Entities
{
    internal class EntitySettings
    {
        public SolidColorBrush Color { get; set; } = Brushes.Black;

        public (int Min, int Max) LifeTime { get; set; }
        public (int Min, int Max) BreedingIterations { get; set; }
        public (int Min, int Max) AmountOfEnergy { get; set; }
        public int MovingIterations { get; set; }
        public int CriticalAmountOfNeighbors { get; set; }
        public bool BreedWith2Parents { get; set; }
    }
}
