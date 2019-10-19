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
        private readonly FileService _fileService;
        private IKAnonimization _anonimizationAlgortihm;

        public KAnonimizationViewModel(FileService fileService)
        {
            _parameterK = 1;
            _fileService = fileService;
            People = new ObservableCollection<Person>(_fileService.GetPeopleData(FILE_WITH_DATA));
            ColumnNames = new ObservableCollection<string>{"Age", "City"};
            _selectedColumnName = "Age";
            _anonimizationAlgortihm = new KAgeAnonimization(ParameterK, People);

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
                _anonimizationAlgortihm = DetermineAlgorithm();
            }
        }

        private IKAnonimization DetermineAlgorithm()
        {
            switch (SelectedColumnName)
            {
                case "Age":
                    return new KAgeAnonimization(ParameterK, People);
                case "City":
                    return new KCityAnonimization(ParameterK, People);
                default:
                    return new KAgeAnonimization(ParameterK, People);
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
                _anonimizationAlgortihm = DetermineAlgorithm();
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
            People = new ObservableCollection<Person>(_anonimizationAlgortihm.GetAnonymizedData());
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
