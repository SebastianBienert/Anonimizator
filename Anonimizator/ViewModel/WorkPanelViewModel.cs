using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using Anonimizator.Algorithms;
using Anonimizator.Helpers;
using Anonimizator.Models;
using Anonimizator.Services;
using GalaSoft.MvvmLight;
using Microsoft.Win32;
using GalaSoft.MvvmLight.Command;

namespace Anonimizator.ViewModel
{
    public class WorkPanelViewModel : ViewModelBase
    {
        private readonly FileService _fileService;

        public WorkPanelViewModel(FileService fileService)
        {
            VisibilityPeopleDataGrid = false;
            _fileService = fileService;
            GenerateDataCommand = new RelayCommand(GenerateData);
        }

        private ObservableCollection<Person> _people;
        public ObservableCollection<Person> People
        {
            get { return _people; }
            set
            {
                _people = value;
                RaisePropertyChanged(nameof(People));
            }
        }

        public ICommand GenerateDataCommand
        {
            get;
            private set;
        }

        private void GenerateData()
        {
            VisibilityPeopleDataGrid = true;
            People = new ObservableCollection<Person>(DataGenerator.GenerateNewDataset(_fileService, SizeNewData));
        }

        private int _sizeNewData;

        public int SizeNewData
        {
            get => _sizeNewData;
            set
            {
                _sizeNewData = value > 0 ? value : 1;
                RaisePropertyChanged(nameof(SizeNewData));
            }
        }

        public bool VisibilityPeopleDataGrid { get; set; }
    }
}
