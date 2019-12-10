using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Anonimizator.Core.Helpers;
using Anonimizator.Core.Models;
using Anonimizator.Core.Services;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using Microsoft.Win32;

namespace Anonimizator.WPF.ViewModel
{
    public class BaseAnonimizationViewModel : ViewModelBase
    {
        protected readonly FileService _fileService;
        protected readonly List<List<string>> _cityDictionary;
        protected readonly List<List<string>> _jobDictionary;

        public BaseAnonimizationViewModel(FileService fileService)
        {
            _fileService = fileService;
            People = new ObservableCollection<Person>(_fileService.GetPeopleData());
            _cityDictionary = _fileService.GetDictionaryData(ConstantStrings.FILE_WITH_CITY_GENERALIZATION_DICTIONARY);
            _jobDictionary = _fileService.GetDictionaryData(ConstantStrings.FILE_WITH_JOB_GENERALIZATION_DICTIONARY);
            SaveDataCommand = new RelayCommand(SaveData);
            RestartDataCommand = new RelayCommand(ReadData);
            RefreshDataCommand = new RelayCommand(Refresh);
            LoadDataCommand = new RelayCommand(LoadData);
        }

        private ObservableCollection<Person> _people;
        public virtual ObservableCollection<Person> People
        {
            get => _people;
            set
            {
                _people = value;
                RaisePropertyChanged(nameof(People));
            }
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

        public ICommand RefreshDataCommand
        {
            get;
            private set;
        }

        public ICommand LoadDataCommand
        {
            get;
            private set;
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

        private void Refresh()
        {
            People = new ObservableCollection<Person>(_fileService.GetPeopleDataFromTemporaryFile());
        }

        private void LoadData()
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.InitialDirectory = Directory.GetParent(Directory.GetCurrentDirectory()).FullName;
            dialog.ShowDialog();
            People = new ObservableCollection<Person>(_fileService.GetPeopleData(dialog.FileName));
        }
    }
}
