﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Input;
using Anonimizator.Algorithms;
using Anonimizator.Helpers;
using Anonimizator.Models;
using Anonimizator.Services;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;

namespace Anonimizator.ViewModel
{
    public class PIDKAnonimizationViewModel : ViewModelBase
    {
        private readonly FileService _fileService;
        private static object _lock = new object();

        public PIDKAnonimizationViewModel(FileService fileService)
        {
            _fileService = fileService;
            People = new ObservableCollection<Person>(_fileService.GetPeopleData(ConstantStrings.FILE_WITH_DATA));
            ColumnNames = new ObservableCollection<string> { "Age", "City", "FirstName", "Surname", "Job" };
            BindingOperations.EnableCollectionSynchronization(_columnNames, _lock);

            KAnonimizationCommand = new RelayCommand(KAnonimizationAlgorithm);
            SaveDataCommand = new RelayCommand(SaveData);
            RestartDataCommand = new RelayCommand(ReadData);
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

        private ObservableCollection<Person> _people;
        public ObservableCollection<Person> People
        {
            get => _people;
            set
            {
                _people = value;
                RaisePropertyChanged(nameof(People));
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

        public ICommand KAnonimizationCommand
        {
            get;
            private set;
        }

        public ICommand SaveDataCommand
        {
            get;
            private set;
        }

        public ICommand RestartDataCommand
        {
            get;
            private set;
        }

        private void KAnonimizationAlgorithm()
        {
            var pid = GetPID(SelectedColumns);
            var _anonimizationAlgortihm = new KCombinedAnonimization(ParameterK, new FileService(), pid);
            People = new ObservableCollection<Person>(_anonimizationAlgortihm.GetAnonymizedData(People));
        }

        private void SaveData()
        {
            _fileService.SavePeopleData(People, ConstantStrings.DEFAULT_FILE_NAME);
        }

        private void ReadData()
        {
            People = new ObservableCollection<Person>(_fileService.GetPeopleData(ConstantStrings.FILE_WITH_DATA));
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

            return pid.ToArray();
        }
    }
}
