using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq.Expressions;
using System.Windows.Input;
using Anonimizator.Core.Algorithms;
using Anonimizator.Core.Helpers;
using Anonimizator.Core.Models;
using Anonimizator.Core.Services;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using Microsoft.Win32;

namespace Anonimizator.WPF.ViewModel
{
    public class PIDKAnonimizationViewModel : BaseAnonimizationViewModel
    {
        public PIDKAnonimizationViewModel(FileService fileService) : base(fileService)
        {
            ColumnNames = new ObservableCollection<string> { "Age", "City", "FirstName", "Surname", "Job", "Gender" };
            KAnonimizationCommand = new RelayCommand(KAnonimizationAlgorithm);
        }

        private ObservableCollection<string> _columnNames;
        public ObservableCollection<string> ColumnNames
        {
            get => _columnNames;
            set
            {
                _columnNames = value;
                RaisePropertyChanged(nameof(ColumnNames));
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

        private Expression<Func<Person, object>>[] GetPID(IList selectedColumns)
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
        public ICommand KAnonimizationCommand
        {
            get;
            private set;
        }

        private void KAnonimizationAlgorithm()
        {
            var pid = GetPID(SelectedColumns);
            var _anonimizationAlgortihm = new KCombinedAnonimization(ParameterK, _jobDictionary, _cityDictionary, pid);
            People = new ObservableCollection<Person>(_anonimizationAlgortihm.GetAnonymizedData(People));
            _fileService.SavePeopleDataInTemporaryFile(People);
        }
        #endregion
    }
}
