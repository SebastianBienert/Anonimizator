using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using GalaSoft.MvvmLight;
using Anonimizator.Models;
using GalaSoft.MvvmLight.CommandWpf;
using System.IO;
using System;

namespace Anonimizator.ViewModel
{
    /// <summary>
    /// This class contains properties that the main View can data bind to.
    /// <para>
    /// Use the <strong>mvvminpc</strong> snippet to add bindable properties to this ViewModel.
    /// </para>
    /// <para>
    /// You can also use Blend to data bind with the tool's support.
    /// </para>
    /// <para>
    /// See http://www.galasoft.ch/mvvm
    /// </para>
    /// </summary>
    public class MainViewModel : ViewModelBase
    {
        public readonly string FILE_NAME = @"data.csv";

        private ObservableCollection<Person> _people;
        public ObservableCollection<Person> People
        {
            get { return _people;}
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

        public MainViewModel()
        {
            People = new ObservableCollection<Person>()
            {
                new Person()
                {
                    City = "Wroclaw",
                    FirstName = "Mati",
                    Gender = "M",
                    Job = "Programista",
                    Surname = "Thomas"
                }
            };
            ColumnNames = new ObservableCollection<string>(typeof(Person).GetProperties().Select(p => p.Name));
            _selectedColumnName = ColumnNames.First();
            SaveDataCommand = new RelayCommand(SaveData);
            AnonymizeColumnCommand = new RelayCommand(AnonymzeColumn);
        }

        public ICommand SaveDataCommand
        {
            get;
            private set;
        }

        public ICommand AnonymizeColumnCommand
        {
            get;
            private set;
        }

        private void SaveData()
        {
            using (var textWriter = File.CreateText(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, FILE_NAME)))
            {
                foreach (var line in Utils.ToCsv(People))
                {
                    textWriter.WriteLine(line);
                }
            }
        }

        private void AnonymzeColumn()
        {
            //Not effecient, but it works
            People = new ObservableCollection<Person>(People.Select(p =>
            {
                p.GetType().GetProperty(SelectedColumnName).SetValue(p, "*");
                return p;
            }));
        }

        


    }
}