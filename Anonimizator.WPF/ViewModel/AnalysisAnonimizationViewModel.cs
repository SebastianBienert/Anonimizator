using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Windows.Input;
using Anonimizator.Core;
using Anonimizator.Core.Enums;
using Anonimizator.Core.Helpers;
using Anonimizator.Core.Models;
using Anonimizator.Core.Services;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using Microsoft.Win32;

namespace Anonimizator.WPF.ViewModel
{
    public class AnalysisAnonimizationViewModel : BaseAnonimizationViewModel
    {
        public AnalysisAnonimizationViewModel(FileService fileService) : base(fileService)
        {
            ColumnNames = new ObservableCollection<string>(typeof(Person).GetProperties().Select(p => p.Name));
            _selectedColumnName = "City";
            CalculatedMetrics = new Dictionary<string, int>();
            MethodCalculateAnonimizationMeasure = AnonimizationMeasure.NumberIdenticalElements;
            CalculateMeasure();
        }


        private ObservableCollection<Person> _people;
        public override ObservableCollection<Person> People
        {
            get => _people;
            set
            {
                _people = value;
                RaisePropertyChanged(nameof(People));
                CalculateMeasure();
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
                CalculateMeasure();
            }
        }

        private Dictionary<string, int> _calculatedMetrics;
        public Dictionary<string, int> CalculatedMetrics
        {
            get { return _calculatedMetrics; }
            set
            {
                _calculatedMetrics = value;
                RaisePropertyChanged(nameof(CalculatedMetrics));
            }
        }

        private AnonimizationMeasure _methodCalculateAnonimizationMeasure;
        public AnonimizationMeasure MethodCalculateAnonimizationMeasure
        {
            get { return _methodCalculateAnonimizationMeasure; }
            set
            {
                _methodCalculateAnonimizationMeasure = value;
                RaisePropertyChanged(nameof(MethodCalculateAnonimizationMeasure));
                CalculateMeasure();
            }
        }

        private void CalculateMeasure()
        {
            if (MethodCalculateAnonimizationMeasure == AnonimizationMeasure.NumberIdenticalElements)
                CalculatedMetrics = AnalysisAnonimizationMethods.CalculateNumberIdenticalElements(People, GetSelectedProperty().Compile());

            if (MethodCalculateAnonimizationMeasure == AnonimizationMeasure.NumberIdenticalLengthElements)
                CalculatedMetrics = AnalysisAnonimizationMethods.CalculateNumberIdenticalLengthElements(People, GetSelectedProperty());
        }

        private Expression<Func<Person, object>> GetSelectedProperty()
        {
            if (SelectedColumnName.Contains("Age"))
                return p => p.Age;
            if (SelectedColumnName.Contains("Surname"))
                return p => p.Surname;
            if (SelectedColumnName.Contains("FirstName"))
                return p => p.FirstName;
            if (SelectedColumnName.Contains("City"))
                return p => p.City;
            if (SelectedColumnName.Contains("Job"))
                return p => p.Job;
            if (SelectedColumnName.Contains("Gender"))
                return p => p.Gender;

            return null;
        }
    }
}
