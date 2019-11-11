using System.Collections.ObjectModel;
using System.Windows.Input;
using Anonimizator.Core.Helpers;
using Anonimizator.Core.Models;
using Anonimizator.Core.Services;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;

namespace Anonimizator.WPF.ViewModel
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
