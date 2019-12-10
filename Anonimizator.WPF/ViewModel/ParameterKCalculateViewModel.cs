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
    public class ParameterKCalculateViewModel : BaseAnonimizationViewModel
    {
        private RecognitionParameterK _recognitionParameterK;

        public ParameterKCalculateViewModel(FileService fileService) : base(fileService)
        {
            XColumnNames = new ObservableCollection<string> { "Age", "City", "FirstName", "Surname", "Job", "Gender" };
            _recognitionParameterK = new RecognitionParameterK(People);
            XSelectedColumns = new[] {"Age"};
            CalculateKParameterCommand = new RelayCommand(CalculateKParameter);
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
        public ICommand CalculateKParameterCommand
        {
            get;
            private set;
        }

        private void CalculateKParameter()
        {
            var pid = GetPID(XSelectedColumns);
            ParameterK = _recognitionParameterK.CalculateParameterK(pid);
        }
        #endregion
    }
}
