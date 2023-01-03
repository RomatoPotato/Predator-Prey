using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace LifeGame.AppData
{
    internal partial class EntitiesPreset : IDataErrorInfo, INotifyDataErrorInfo, INotifyPropertyChanged
    {
		private int areaWidth;
		private int predatorsCount;
		private int preysCount;
        private bool deathByOverpopulationPredator;
        private bool deathByOverpopulationPrey;
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

        private readonly Dictionary<string, List<string>> errors = new Dictionary<string, List<string>>();

        public string Error { get; set; }
        public bool HasErrors => errors.Count != 0;

        private bool hasNoErrors = true;

        public bool HasNoErrors
        {
            get { return hasNoErrors; }
            set 
            {
                hasNoErrors = value;
                OnPropertyChanged();
            }
        }

        public string this[string columnName]
        {
            get
            {
                switch (columnName)
                {
                    case nameof(AreaWidth):
                    case nameof(PredatorsCount):
                    case nameof(PreysCount):
                        if (Math.Pow(areaWidth, 2) < predatorsCount + preysCount)
                        {
                            AddError(nameof(AreaWidth), "Общее количество хищников и жертв превышает размер поля!");
                        }
                        else
                        {
                            ClearErrors(nameof(AreaWidth));
                        }
                        break;
                }

                return string.Empty;
            }
        }

        public int AreaWidth
		{
			get { return areaWidth; }
			set
            {
                areaWidth = value;
                OnPropertyChanged();
            }
        }

        public int PredatorsCount
        {
            get { return predatorsCount; }
            set
            { 
                predatorsCount = value;
                OnPropertyChanged();
            }
        }

        public int PreysCount
        {
            get { return preysCount; }
            set
            {
                preysCount = value;
                OnPropertyChanged();
            }
        }

        public bool DeathByOverpopulationPredator
        {
            get { return deathByOverpopulationPredator; }
            set 
            {
                deathByOverpopulationPredator = value;
                OnPropertyChanged();
            }
        }

        public bool DeathByOverpopulationPrey
        {
            get { return deathByOverpopulationPrey; }
            set
            {
                deathByOverpopulationPrey = value;
                OnPropertyChanged();
            }
        }

        public int CriticalAmountOfNeighborsPredator
        {
            get { return criticalAmountOfNeighborsPredator; }
            set
            {
                criticalAmountOfNeighborsPredator = value;
                OnPropertyChanged();
            }
        }
        
        public int CriticalAmountOfNeighborsPrey
        {
            get { return criticalAmountOfNeighborsPrey; }
            set 
            { 
                criticalAmountOfNeighborsPrey = value;
                OnPropertyChanged();
            }
        }

        public bool BreedingWith2ParentsPredator
        {
            get { return breedingWith2ParentsPredator; }
            set 
            {
                breedingWith2ParentsPredator = value;
                OnPropertyChanged();
            }
        }

        public bool BreedingWith2ParentsPrey
        {
            get { return breedingWith2ParentsPrey; }
            set 
            {
                breedingWith2ParentsPrey = value;
                OnPropertyChanged();
            }
        }

        public int MovingIterationsPredator
        {
            get { return movingIterationsPredator; }
            set 
            {
                movingIterationsPredator = value;
                OnPropertyChanged();
            }
        }
        
        public int MovingIterationsPrey
        {
            get { return movingIterationsPrey; }
            set
            {
                movingIterationsPrey = value;
                OnPropertyChanged();
            }
        }

        public int BreedingIterationsPredatorMin
        {
            get { return breedingIterationsPredatorMin; }
            set 
            {
                breedingIterationsPredatorMin = breedingIterationsPredatorMax == 0 ? value : Math.Min(value, Math.Max(1, breedingIterationsPredatorMax));
                OnPropertyChanged();
            }
        }
        
        public int BreedingIterationsPredatorMax
        {
            get { return breedingIterationsPredatorMax; }
            set
            { 
                breedingIterationsPredatorMax = Math.Max(breedingIterationsPredatorMin, value);
                OnPropertyChanged();
            }
        }
        
        public int BreedingIterationsPreyMin
        {
            get { return breedingIterationsPreyMin; }
            set
            {
                breedingIterationsPreyMin = breedingIterationsPreyMax == 0 ? value : Math.Min(value, Math.Max(1, breedingIterationsPreyMax));
                OnPropertyChanged();
            }
        }
        
        public int BreedingIterationsPreyMax
        {
            get { return breedingIterationsPreyMax; }
            set
            {
                breedingIterationsPreyMax = Math.Max(breedingIterationsPreyMin, value);
                OnPropertyChanged();
            }
        }

        public int LifeTimePredatorMin
        {
            get { return lifeTimePredatorMin; }
            set
            {
                lifeTimePredatorMin = lifeTimePredatorMax == 0 ? value : Math.Min(value, Math.Max(1, lifeTimePredatorMax));
                OnPropertyChanged();
            }
        }
        
        public int LifeTimePredatorMax
        {
            get { return lifeTimePredatorMax; }
            set
            {
                lifeTimePredatorMax = Math.Max(lifeTimePredatorMin, value);
                OnPropertyChanged();
            }
        }
        
        public int LifeTimePreyMin
        {
            get { return lifeTimePreyMin; }
            set
            {
                lifeTimePreyMin = lifeTimePreyMax == 0 ? value : Math.Min(value, Math.Max(1, lifeTimePreyMax));
                OnPropertyChanged();
            }
        }
        
        public int LifeTimePreyMax
        {
            get { return lifeTimePreyMax; }
            set
            {
                lifeTimePreyMax = Math.Max(lifeTimePreyMin, value);
                OnPropertyChanged();
            }
        }
        
        public int AmountOfEnergyPredatorMin
        {
            get { return amountOfEnergyPredatorMin; }
            set
            {
                amountOfEnergyPredatorMin = amountOfEnergyPredatorMax == 0 ? value : Math.Min(value, Math.Max(1, amountOfEnergyPredatorMax));
                OnPropertyChanged();
            }
        }
        
        public int AmountOfEnergyPredatorMax
        {
            get { return amountOfEnergyPredatorMax; }
            set
            {
                amountOfEnergyPredatorMax = Math.Max(amountOfEnergyPredatorMin, value);
                OnPropertyChanged();
            }
        }

        public event EventHandler<DataErrorsChangedEventArgs>? ErrorsChanged;
        public event PropertyChangedEventHandler? PropertyChanged;

        public IEnumerable GetErrors(string? propertyName)
        {
            if (string.IsNullOrEmpty(propertyName))
            {
                return errors.Values;
            }

            return errors.ContainsKey(propertyName) ? errors[propertyName] : null;
        }

        private void OnErrorsChanged(string propertyName)
        {
            ErrorsChanged?.Invoke(this, new DataErrorsChangedEventArgs(propertyName));
        }

        private void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void AddError(string propertyName, string error)
        {
            AddErrors(propertyName, new List<string> { error});
        }

        private void AddErrors(string propertyName, IList<string> errorsList)
        {
            bool changed = false;

            if (!errors.ContainsKey(propertyName))
            {
                errors.Add(propertyName, new List<string>());
                changed = true;
            }

            foreach (var err in errorsList)
            {
                if (errors[propertyName].Contains(err)) continue;
                errors[propertyName].Add(err);
                changed = true;
            }

            if (changed)
            {
                OnErrorsChanged(propertyName);
                HasNoErrors = false;
            }
        }

        private void ClearErrors(string propertyName = "")
        {
            if (string.IsNullOrEmpty(propertyName))
            {
                errors.Clear();
            }
            else
            {
                errors.Remove(propertyName);
            }

            if (errors.Count == 0)
            {
                HasNoErrors = true;
            }

            OnErrorsChanged(propertyName);
        }
    }
}
