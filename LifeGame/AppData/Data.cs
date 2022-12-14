namespace LifeGame.AppData
{
    internal class Data
    {
		private int areaWidth;
		private int predatorsCount;
		private int preyCount;
        private bool deathByOverpopulatingPredator;
        private bool deathByOverpopulatingPrey;
        private int criticalAmountOfNeighborsPredator;
        private int criticalAmountOfNeighborsPrey;
        private bool breedingWith2ParentsPredator;
        private bool breedingWith2ParentsPrey;
        private int movingIterationsPredator;
        private int movingIterationsPrey;
        private int breedingIterationsPredatorMin;
        private int breedingIterationsPredatorMax;
        private int breedingIterationsPreyMin;
        private int breedingIterationsPreyMax;
        private int lifeTimePredatorMin;
        private int lifeTimePredatorMax;
        private int lifeTimePreyMin;
        private int lifeTimePreyMax;
        private int amountOfEnergyPredatorMin;
        private int amountOfEnergyPredatorMax;

        public int AreaWidth
		{
			get { return areaWidth; }
			set { areaWidth = value; }
        }

        public int PredatorsCount
        {
            get { return predatorsCount; }
            set { predatorsCount = value; }
        }

        public int PreyCount
        {
            get { return preyCount; }
            set { preyCount = value; }
        }

        public bool DeathByOverpopulatingPredator
        {
            get { return deathByOverpopulatingPredator; }
            set { deathByOverpopulatingPredator = value; }
        }

        public bool DeathByOverpopulatingPrey
        {
            get { return deathByOverpopulatingPrey; }
            set { deathByOverpopulatingPrey = value; }
        }

        public int CriticalAmountOfNeighborsPredator
        {
            get { return criticalAmountOfNeighborsPredator; }
            set { criticalAmountOfNeighborsPredator = value; }
        }
        
        public int CriticalAmountOfNeighborsPrey
        {
            get { return criticalAmountOfNeighborsPrey; }
            set { criticalAmountOfNeighborsPrey = value; }
        }

        public bool BreedingWith2ParentsPredator
        {
            get { return breedingWith2ParentsPredator; }
            set { breedingWith2ParentsPredator = value; }
        }

        public bool BreedingWith2ParentsPrey
        {
            get { return breedingWith2ParentsPrey; }
            set { breedingWith2ParentsPrey = value; }
        }

        public int MovingIterationsPredator
        {
            get { return movingIterationsPredator; }
            set { movingIterationsPredator = value; }
        }
        
        public int MovingIterationsPrey
        {
            get { return movingIterationsPrey; }
            set { movingIterationsPrey = value; }
        }

        public int BreedingIterationsPredatorMin
        {
            get { return breedingIterationsPredatorMin; }
            set { breedingIterationsPredatorMin = value; }
        }
        
        public int BreedingIterationsPredatorMax
        {
            get { return breedingIterationsPredatorMax; }
            set { breedingIterationsPredatorMax = value; }
        }
        
        public int BreedingIterationsPreyMin
        {
            get { return breedingIterationsPreyMin; }
            set { breedingIterationsPreyMin = value; }
        }
        
        public int BreedingIterationsPreyMax
        {
            get { return breedingIterationsPreyMax; }
            set { breedingIterationsPreyMax = value; }
        }

        public int LifeTimePredatorMin
        {
            get { return lifeTimePredatorMin; }
            set { lifeTimePredatorMin = value;}
        }
        
        public int LifeTimePredatorMax
        {
            get { return lifeTimePredatorMax; }
            set { lifeTimePredatorMax = value;}
        }
        
        public int LifeTimePreyMin
        {
            get { return lifeTimePreyMin; }
            set { lifeTimePreyMin = value;}
        }
        
        public int LifeTimePreyMax
        {
            get { return lifeTimePreyMax; }
            set { lifeTimePreyMax = value;}
        }
        
        public int AmountOfEnergyPredatorMin
        {
            get { return amountOfEnergyPredatorMin; }
            set { amountOfEnergyPredatorMin = value;}
        }
        
        public int AmountOfEnergyPredatorMax
        {
            get { return amountOfEnergyPredatorMax; }
            set { amountOfEnergyPredatorMax = value;}
        }
    }
}
