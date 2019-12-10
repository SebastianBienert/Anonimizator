using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Windows.Input;
using Anonimizator.Core.Helpers;
using Anonimizator.Core.Models;
using Anonimizator.Core.Services;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using Microsoft.Win32;

namespace Anonimizator.WPF.ViewModel
{
    public class AnonimizationViewModel : BaseAnonimizationViewModel
    {
        public AnonimizationViewModel(FileService fileService) : base(fileService)
        {
            ColumnNames = new ObservableCollection<string>(typeof(Person).GetProperties().Select(p => p.Name));
            _selectedColumnName = ColumnNames.First();
            CharacterMaskingCommand = new RelayCommand(CharakterMaskingColumn);
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

        #region command
        public ICommand CharacterMaskingCommand
        {
            get;
            private set;
        }

        public ICommand GeneralizationCommand
        {
            get;
            private set;
        }

        private void CharakterMaskingColumn()
        {
            //Not effecient, but it works
            People = new ObservableCollection<Person>(People.Select(p =>
            {
                p.GetType().GetProperty(SelectedColumnName).SetValue(p, "*");
                return p;
            }));
            _fileService.SavePeopleDataInTemporaryFile(People);
        }
        #endregion
    }
}
