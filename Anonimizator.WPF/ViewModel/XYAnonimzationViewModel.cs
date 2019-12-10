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
using Anonimizator.Core.Algorithms;
using Anonimizator.Core.Helpers;
using Anonimizator.Core.Models;
using Anonimizator.Core.Services;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using Microsoft.Win32;

namespace Anonimizator.WPF.ViewModel
{
    public class XYAnonimzationViewModel : ViewModelBase
    {
        private readonly FileService _fileService;
        private readonly List<List<string>> _cityDictionary;
        private readonly List<List<string>> _jobDictionary;

        public XYAnonimzationViewModel(FileService fileService)
        {
            _fileService = fileService;
            People = new ObservableCollection<Person>(_fileService.GetPeopleData());
            XColumnNames = new ObservableCollection<string> { "Age", "City", "FirstName", "Surname", "Job", "Gender" };
            YColumnNames = new ObservableCollection<string> { "Age", "City", "FirstName", "Surname", "Job", "Gender" };
            _cityDictionary = _fileService.GetDictionaryData(ConstantStrings.FILE_WITH_CITY_GENERALIZATION_DICTIONARY);
            _jobDictionary = _fileService.GetDictionaryData(ConstantStrings.FILE_WITH_JOB_GENERALIZATION_DICTIONARY);

            KAnonimizationCommand = new RelayCommand(KAnonimizationAlgorithm);
            SaveDataCommand = new RelayCommand(SaveData);
            RestartDataCommand = new RelayCommand(ReadData);
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

        private ObservableCollection<string> _yColumnNames;
        public ObservableCollection<string> YColumnNames
        {
            get => _yColumnNames;
            set
            {
                _yColumnNames = value;
                RaisePropertyChanged(nameof(YColumnNames));
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

        private IList _ySelectedColumns;
        public IList YSelectedColumns
        {
            get => _ySelectedColumns;
            set
            {
                _ySelectedColumns = value;
                RaisePropertyChanged(nameof(YSelectedColumns));
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
            var xColumns = GetPID(XSelectedColumns);
            var yColumns = GetPID(YSelectedColumns);
            var _anonimizationAlgortihm = new XYAnonimization(ParameterK, _jobDictionary, _cityDictionary, xColumns, yColumns);
            People = new ObservableCollection<Person>(_anonimizationAlgortihm.GetAnonymizedData(People));
            _fileService.SavePeopleDataInTemporaryFile(People);
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
