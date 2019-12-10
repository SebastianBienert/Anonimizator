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
    public class KAnonimizationViewModel : BaseAnonimizationViewModel
    {
        private IKAnonimization _anonimizationAlgortihm;

        public KAnonimizationViewModel(FileService fileService) : base(fileService)
        {
            _parameterK = 1;
            ColumnNames = new ObservableCollection<string>{"Age", "City", "FirstName", "Surname", "Job", "All columns"};
            _selectedColumnName = "Age";
            _anonimizationAlgortihm = new KAgeAnonimization(ParameterK);
            KAnonimizationCommand = new RelayCommand(KAnonimizationAlgorithm);
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
                    return new KCombinedAnonimization(ParameterK, _jobDictionary, _cityDictionary,
                        p => p.FirstName, p => p.Surname, p => p.Age, p => p.Job, p => p.City, p => p.Gender);
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

        #region Commands
        public ICommand KAnonimizationCommand
        {
            get;
            private set;
        }

        private void KAnonimizationAlgorithm()
        {
            _anonimizationAlgortihm = DetermineAlgorithm();
            People = new ObservableCollection<Person>(_anonimizationAlgortihm.GetAnonymizedData(People));
            _fileService.SavePeopleDataInTemporaryFile(People);
        }
        #endregion
    }
}
