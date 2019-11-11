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
    public class PIDKAnonimizationViewModel : ViewModelBase
    {
        private readonly FileService _fileService;
        private readonly List<List<string>> _cityDictionary;
        private readonly List<List<string>> _jobDictionary;

        public PIDKAnonimizationViewModel(FileService fileService)
        {
            _fileService = fileService;
            People = new ObservableCollection<Person>(_fileService.GetPeopleData(ConstantStrings.FILE_WITH_DATA));
            ColumnNames = new ObservableCollection<string> { "Age", "City", "FirstName", "Surname", "Job" };
            _cityDictionary = _fileService.GetDictionaryData(ConstantStrings.FILE_WITH_CITY_GENERALIZATION_DICTIONARY);
            _jobDictionary = _fileService.GetDictionaryData(ConstantStrings.FILE_WITH_JOB_GENERALIZATION_DICTIONARY);

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
            var _anonimizationAlgortihm = new KCombinedAnonimization(ParameterK, _jobDictionary, _cityDictionary, pid);
            People = new ObservableCollection<Person>(_anonimizationAlgortihm.GetAnonymizedData(People));
        }

        private void SaveData()
        {
            var selectedFileName = GetSelectedFileName(ConstantStrings.DEFAULT_FILE_NAME);
            _fileService.SavePeopleData(People, selectedFileName);
        }

        private string GetSelectedFileName(string defaultFileName)
        {
            var sfd = new SaveFileDialog
            {
                Filter = "Text Files (*.csv)|*.csv|All files (*.*)|*.*",
            };
            if (sfd.ShowDialog() == true)
            {
                return sfd.FileName;
            }

            return Path.Combine(AppDomain.CurrentDomain.BaseDirectory, defaultFileName);
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
