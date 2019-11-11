using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
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
    public class KAnonimizationViewModel : ViewModelBase
    {
        private readonly FileService _fileService;
        private IKAnonimization _anonimizationAlgortihm;
        private readonly List<List<string>> _cityDictionary;
        private readonly List<List<string>> _jobDictionary;

        public KAnonimizationViewModel(FileService fileService)
        {
            _parameterK = 1;
            _fileService = fileService;
            People = new ObservableCollection<Person>(_fileService.GetPeopleData(ConstantStrings.FILE_WITH_DATA));
            ColumnNames = new ObservableCollection<string>{"Age", "City", "FirstName", "Surname", "Job", "All columns"};
            _cityDictionary = _fileService.GetDictionaryData(ConstantStrings.FILE_WITH_CITY_GENERALIZATION_DICTIONARY);
            _jobDictionary = _fileService.GetDictionaryData(ConstantStrings.FILE_WITH_JOB_GENERALIZATION_DICTIONARY);
            _selectedColumnName = "Age";
            _anonimizationAlgortihm = new KAgeAnonimization(ParameterK);

            SaveDataCommand = new RelayCommand(SaveData);
            KAnonimizationCommand = new RelayCommand(KAnonimizationAlgorithm);
            RestartDataCommand = new RelayCommand(ReadData); 
        }

        private ObservableCollection<Person> _people;
        public ObservableCollection<Person> People
        {
            get { return _people; }
            set
            {
                _people = value;
                RaisePropertyChanged(nameof(People));
            }
        }

        public ObservableCollection<string> ColumnNames { get; set; }

        private string _selectedColumnName;
        public string SelectedColumnName
        {
            get { return _selectedColumnName; }
            set
            {
                _selectedColumnName = value;
                RaisePropertyChanged(nameof(SelectedColumnName));
            }
        }

        private IKAnonimization DetermineAlgorithm()
        {
            switch (SelectedColumnName)
            {
                case "Age":
                    return new KAgeAnonimization(ParameterK);
                case "City":
                    return new KCityAnonimization(ParameterK, _cityDictionary);
                case "FirstName":
                    return new KAttributeLengthAnonimization<string>(ParameterK, p => p.FirstName);
                case "Surname":
                    return new CommonStartStringMasking<string>(ParameterK, p => p.Surname);
                case "Job":
                    return new KJobAnonimization(ParameterK, _jobDictionary);
                case "All columns":
                    return new KCombinedAnonimization(ParameterK, _jobDictionary, _cityDictionary, p => p.FirstName, p => p.Surname, p => p.Age);
                default:
                    return new KAgeAnonimization(ParameterK);
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

        public ICommand SaveDataCommand
        {
            get;
            private set;
        }

        public ICommand KAnonimizationCommand
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
            _anonimizationAlgortihm = DetermineAlgorithm();
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
       
    }
}
