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
    public class XYAnonimzationViewModel : BaseAnonimizationViewModel
    {
        public XYAnonimzationViewModel(FileService fileService) : base(fileService)
        {
            XColumnNames = new ObservableCollection<string> { "Age", "City", "FirstName", "Surname", "Job", "Gender" };
            YColumnNames = new ObservableCollection<string> { "Age", "City", "FirstName", "Surname", "Job", "Gender" };
            KAnonimizationCommand = new RelayCommand(KAnonimizationAlgorithm);
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
            var xColumns = GetPID(XSelectedColumns);
            var yColumns = GetPID(YSelectedColumns);
            var _anonimizationAlgortihm = new XYAnonimization(ParameterK, _jobDictionary, _cityDictionary, xColumns, yColumns);
            People = new ObservableCollection<Person>(_anonimizationAlgortihm.GetAnonymizedData(People));
            _fileService.SavePeopleDataInTemporaryFile(People);
        }
        #endregion
    }
}
