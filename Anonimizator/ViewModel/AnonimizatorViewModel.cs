using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using GalaSoft.MvvmLight;
using Anonimizator.Models;
using GalaSoft.MvvmLight.CommandWpf;
using System.IO;
using System;
using Microsoft.Win32;

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
    public class AnonimizatorViewModel : ViewModelBase
    {
        public readonly string DEFAULT_FILE_NAME = @"data.csv";
        public readonly string FILE_WITH_DATA = @"data.csv";

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

        public AnonimizatorViewModel()
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
            ReadData();
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
            var pathDataFile = SelectFileToSaveData();
            using (var textWriter = File.CreateText(pathDataFile))
            {
                foreach (var line in Utils.ToCsv(People))
                {
                    textWriter.WriteLine(line);
                }
            }
        }

        private void ReadData()
        {
            People = new ObservableCollection<Person>();
            using (var reader = new StreamReader(Path.Combine(Directory.GetParent(Environment.CurrentDirectory).Parent.FullName, FILE_WITH_DATA)))
            {
                while (!reader.EndOfStream)
                {
                    var line = reader.ReadLine();
                    var person = Utils.PersonFromCsv(line);
                    People.Add(person);
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

        private string SelectFileToSaveData()
        {
            var sfd = new SaveFileDialog
            {
                Filter = "Text Files (*.csv)|*.csv|All files (*.*)|*.*",
            };
            if (sfd.ShowDialog() == true)
            { 
                return sfd.FileName;
            }

            return Path.Combine(AppDomain.CurrentDomain.BaseDirectory, DEFAULT_FILE_NAME);
        }

    }
}