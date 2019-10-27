using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using Anonimizator.Algorithms;
using Anonimizator.Models;
using Anonimizator.Services;
using GalaSoft.MvvmLight;
using Microsoft.Win32;
using GalaSoft.MvvmLight.Command;

namespace Anonimizator.ViewModel
{
    public class KAnonimizationViewModel : ViewModelBase
    {
        public readonly string DEFAULT_FILE_NAME = @"data.csv";
        public readonly string FILE_WITH_DATA = @"data.csv";
        public readonly string FILE_WITH_CITY_GENERALIZATION_DICTIONARY = @"cityDictionary.csv";
        public readonly string FILE_WITH_JOB_GENERALIZATION_DICTIONARY = @"jobDictionary.csv";
        private readonly FileService _fileService;
        private IKAnonimization _anonimizationAlgortihm;
        private List<List<string>> _cityDictionary;
        private List<List<string>> _jobDictionary;

        public KAnonimizationViewModel(FileService fileService)
        {
            _parameterK = 1;
            _fileService = fileService;
            People = new ObservableCollection<Person>(_fileService.GetPeopleData(FILE_WITH_DATA));
            ColumnNames = new ObservableCollection<string>{"Age", "City", "FirstName", "Surname", "Job"};
            _cityDictionary = _fileService.GetDictionaryData(FILE_WITH_CITY_GENERALIZATION_DICTIONARY);
            _jobDictionary = _fileService.GetDictionaryData(FILE_WITH_JOB_GENERALIZATION_DICTIONARY);
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
                RaisePropertyChanged("People");
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
                RaisePropertyChanged("SelectedColumnName");
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
                    return new KAttributeLengthAnonimization<string>(ParameterK, p => p.Surname);
                case "Job":
                    return new KJobAnonimization(ParameterK, _jobDictionary);
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
                RaisePropertyChanged("ParameterK");
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
            _fileService.SavePeopleData(People, DEFAULT_FILE_NAME);
        }

        private void ReadData()
        {
            People = new ObservableCollection<Person>(_fileService.GetPeopleData(FILE_WITH_DATA));
        }
       
    }
}
