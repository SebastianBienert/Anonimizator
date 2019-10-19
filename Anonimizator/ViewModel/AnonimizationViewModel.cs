﻿using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using GalaSoft.MvvmLight;
using Anonimizator.Models;
using GalaSoft.MvvmLight.CommandWpf;
using System.IO;
using System;
using Anonimizator.Services;
using Microsoft.Win32;

namespace Anonimizator.ViewModel
{
    public class AnonimizationViewModel : ViewModelBase
    {
        private readonly string DEFAULT_FILE_NAME = @"data.csv";
        private readonly string FILE_WITH_DATA = @"data.csv";
        private readonly FileService _fileService;

        public AnonimizationViewModel(FileService fileService)
        {
            _fileService = fileService;
            People = new ObservableCollection<Person>(_fileService.GetPeopleData(FILE_WITH_DATA));
            ColumnNames = new ObservableCollection<string>(typeof(Person).GetProperties().Select(p => p.Name));
            _selectedColumnName = ColumnNames.First();

            SaveDataCommand = new RelayCommand(SaveData);
            CharakterMaskingCommand = new RelayCommand(CharakterMaskingColumn);
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

        public ICommand CharakterMaskingCommand
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
            _fileService.SavePeopleData(People, DEFAULT_FILE_NAME);
        }
    }
}
