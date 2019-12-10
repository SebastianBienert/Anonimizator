using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
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
    public class ParameterKCalculateViewModel : ViewModelBase
    {
        private readonly FileService _fileService;
        private readonly List<List<string>> _cityDictionary;
        private readonly List<List<string>> _jobDictionary;
        private RecognitionParameterK _recognitionParameterK;

        public ParameterKCalculateViewModel(FileService fileService)
        {
            _fileService = fileService;
            People = new ObservableCollection<Person>(_fileService.GetPeopleData());
            XColumnNames = new ObservableCollection<string> { "Age", "City", "FirstName", "Surname", "Job", "Gender" };
            _cityDictionary = _fileService.GetDictionaryData(ConstantStrings.FILE_WITH_CITY_GENERALIZATION_DICTIONARY);
            _jobDictionary = _fileService.GetDictionaryData(ConstantStrings.FILE_WITH_JOB_GENERALIZATION_DICTIONARY);

            _recognitionParameterK = new RecognitionParameterK(People);
            XSelectedColumns = new[] {"Age"};
            CalculateKParameterCommand = new RelayCommand(CalculateKParameter);
            RestartDataCommand = new RelayCommand(ReadData);
            CalculateKParameter();
        }

        private ObservableCollection<string> _xColumnNames;
        public ObservableCollection<string> XColumnNames
        {
            get => _xColumnNames;
            set
            {
                _xColumnNames = value;
                RaisePropertyChanged(nameof(XColumnNames));
            }
        }

        private ObservableCollection<Person> _people;
        public ObservableCollection<Person> People
        {
            get => _people;
            set
            {
                _people = value;
                _recognitionParameterK = new RecognitionParameterK(_people);
                RaisePropertyChanged(nameof(People));
            }
        }

        private IList _xSelectedColumns;
        public IList XSelectedColumns
        {
            get => _xSelectedColumns;
            set
            {
                _xSelectedColumns = value;
                RaisePropertyChanged(nameof(XSelectedColumns));
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

        public ICommand CalculateKParameterCommand
        {
            get;
            private set;
        }

        public ICommand RestartDataCommand
        {
            get;
            private set;
        }

        private void CalculateKParameter()
        {
            var pid = GetPID(XSelectedColumns);
            ParameterK = _recognitionParameterK.CalculateParameterK(pid);
        }

        private void ReadData()
        {
            People = new ObservableCollection<Person>(_fileService.GetPeopleData());
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
    }
}
