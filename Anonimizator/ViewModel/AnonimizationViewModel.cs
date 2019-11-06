using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using GalaSoft.MvvmLight;
using Anonimizator.Models;
using GalaSoft.MvvmLight.CommandWpf;
using System.IO;
using System;
using Anonimizator.Helpers;
using Anonimizator.Services;
using Microsoft.Win32;

namespace Anonimizator.ViewModel
{
    public class AnonimizationViewModel : ViewModelBase
    {
        private readonly FileService _fileService;

        public AnonimizationViewModel(FileService fileService)
        {
            _fileService = fileService;
            People = new ObservableCollection<Person>(_fileService.GetPeopleData(ConstantStrings.FILE_WITH_DATA));
            ColumnNames = new ObservableCollection<string>(typeof(Person).GetProperties().Select(p => p.Name));
            _selectedColumnName = ColumnNames.First();

            SaveDataCommand = new RelayCommand(SaveData);
            CharacterMaskingCommand = new RelayCommand(CharakterMaskingColumn);
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

        public ICommand SaveDataCommand
        {
            get;
            private set;
        }

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
        }

        private void SaveData()
        {
            _fileService.SavePeopleData(People, ConstantStrings.DEFAULT_FILE_NAME);
        }
    }
}
