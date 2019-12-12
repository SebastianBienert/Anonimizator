using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Windows.Input;
using Anonimizator.Core;
using Anonimizator.Core.Algorithms;
using Anonimizator.Core.Helpers;
using Anonimizator.Core.Models;
using Anonimizator.Core.Services;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using Microsoft.Win32;

namespace Anonimizator.WPF.ViewModel
{
    public class PIDKPredictionViewModel : BaseAnonimizationViewModel
    {
        private readonly List<string> ColumnNames;

        public PIDKPredictionViewModel(FileService fileService) : base(fileService)

        {
            _isShowParameterK1 = true;
            IsEnabledCheckBoxShowK1 = false;
            ColumnNames = new List<string> { "Age", "City", "FirstName", "Surname", "Job", "Gender" };
            KPredictionCommand = new RelayCommand(KPredictionAlgorithm);
        }

        private ObservableCollection<KPrediction> _predictionParameterK;
        public virtual ObservableCollection<KPrediction> PredictionParameterK
        {
            get => _predictionParameterK;
            set
            {
                _predictionParameterK = value;
                RaisePropertyChanged(nameof(PredictionParameterK));
            }
        }

        private bool _isShowParameterK1;

        public bool IsShowParameterK1
        {
            get => _isShowParameterK1;
            set
            {
                _isShowParameterK1 = value;
                RaisePropertyChanged(nameof(IsShowParameterK1));
                RaisePropertyChanged(nameof(PredictionParameterK));
                KPredictionAlgorithm();
            }
        }

        private bool _isEnabledCheckBoxShowK1;

        public bool IsEnabledCheckBoxShowK1
        {
            get => _isEnabledCheckBoxShowK1;
            set
            {
                _isEnabledCheckBoxShowK1 = value;
                RaisePropertyChanged(nameof(IsEnabledCheckBoxShowK1));
            }
        }

        private IList _selectedColumns;
        public IList SelectedColumns
        {
            get => _selectedColumns;
            set
            {
                _selectedColumns = value;
                RaisePropertyChanged(nameof(SelectedColumns));
            }
        }

        private int _parameterK;
        public int ParameterK
        {
            get { return _parameterK; }
            set
            {
                _parameterK = value;
                RaisePropertyChanged(nameof(ParameterK));
            }
        }

        private Expression<Func<Person, object>>[] GetPID(List<string> selectedColumns)
        {
            var pid = new List<Expression<Func<Person, object>>>();
            if (selectedColumns.Contains("Age"))
                pid.Add(p => p.Age);
            if (selectedColumns.Contains("Surname"))
                pid.Add(p => p.Surname);
            if (selectedColumns.Contains("FirstName"))
                pid.Add(p => p.FirstName);
            if (selectedColumns.Contains("City"))
                pid.Add(p => p.City);
            if (selectedColumns.Contains("Job"))
                pid.Add(p => p.Job);
            if (selectedColumns.Contains("Gender"))
                pid.Add(p => p.Gender);

            return pid.ToArray();
        }

        #region Commands
        public ICommand KPredictionCommand
        {
            get;
            private set;
        }

        private void KPredictionAlgorithm()
        {
            IsEnabledCheckBoxShowK1 = true;
            var predictionParameterK = new List<KPrediction>();
            var combinationsColumns = EnumerateAllCombinationsColumns();

            foreach (var combinationColumns in combinationsColumns)
            {
                var recognitionParameterK = new RecognitionParameterK(People);
                var pid = GetPID(combinationColumns);
                predictionParameterK.Add(new KPrediction(recognitionParameterK.CalculateParameterK(pid), string.Join(", ", combinationColumns.ToArray())));
            }
            PredictionParameterK = new ObservableCollection<KPrediction>(predictionParameterK.Where(x => IsShowParameterK1 ? x.K > 0 : x.K > 1).OrderByDescending(x => x.K));
        }
        #endregion

        private List<List<string>> EnumerateAllCombinationsColumns()
        {
            var combinationsColumns = new List<List<string>>();

            var list = ColumnNames;
            for (int i = 0; i < ColumnNames.Count; i++)
            {
                combinationsColumns.AddRange(GetPermutations(list, i).Select(x => x.ToList()));
            }
            combinationsColumns.Add(ColumnNames);

            return combinationsColumns;
        }

        static IEnumerable<IEnumerable<T>> GetPermutations<T>(IEnumerable<T> items, int count)
        {
            int i = 0;
            foreach (var item in items)
            {
                if (count == 1)
                    yield return new T[] { item };
                else
                {
                    foreach (var result in GetPermutations(items.Skip(i + 1), count - 1))
                        yield return new T[] { item }.Concat(result);
                }
                ++i;
            }
        }
    }
}
