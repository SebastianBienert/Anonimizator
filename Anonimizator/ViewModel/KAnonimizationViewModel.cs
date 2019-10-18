using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using Anonimizator.Models;
using GalaSoft.MvvmLight;
using Microsoft.Win32;
using GalaSoft.MvvmLight.Command;

namespace Anonimizator.ViewModel
{
    public class KAnonimizationViewModel : ViewModelBase
    {
        public readonly string DEFAULT_FILE_NAME = @"data.csv";
        public readonly string FILE_WITH_DATA = @"data.csv";

        public KAnonimizationViewModel()
        {
            _parameterK = 1;
            ReadData();
            ColumnNames = new ObservableCollection<string>(typeof(Person).GetProperties().Select(p => p.Name));
            _selectedColumnName = ColumnNames.First();
            SaveDataCommand = new RelayCommand(SaveData);
            KAnonimizationCommand = new RelayCommand(KAnonimizationAlgorithm);
            RestartDataCommand = new RelayCommand(ReadData); 
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

        private int _parameterK;
        public int ParameterK
        {
            get { return _parameterK; }
            set
            {
                _parameterK = value;
            }
        }

        public ICommand SaveDataCommand
        {
            get;
            private set;
        }

        public ICommand KAnonimizationCommand
        {
            get;
            private set;
        }
        
        public ICommand RestartDataCommand
        {
            get;
            private set;
        }

        private void KAnonimizationAlgorithm()
        {
            KAnonimizationAgeColumn();
            //MessageBox.Show($"Parametr K: {ParameterK}");
            //int k = 4;
            //var lp = People.OrderBy(c => int.Parse(c.Age));

            //foreach (var p in lp)
            //{

            //}
            ////Not effecient, but it works
            //People = new ObservableCollection<Person>(People.Select(p =>
            //{
            //    p.GetType().GetProperty(SelectedColumnName).SetValue(p, "*");
            //    return p;
            //}));
        }

        private void KAnonimizationAgeColumn()
        {
            var lp = People.OrderBy(c => int.Parse(c.Age)).ToList();
            ObservableCollection<Person> tmpList = new ObservableCollection<Person>();
            ObservableCollection<Person> newCollection = new ObservableCollection<Person>();

            int k = ParameterK;
            int lengthList = lp.Count;
            for (int i = 0; i < lengthList; i++)
            {
                var p = lp[i];
                if (k > 0 || (tmpList.LastOrDefault() != null && tmpList.Last().Age == p.Age) || (lengthList - i < ParameterK) || (tmpList.First().Age == tmpList.Last().Age))
                {
                    tmpList.Add(p);
                    k--;
                }
                else
                {
                    string compartment = tmpList.First().Age + " - " + tmpList.Last().Age;
                    foreach (var e in tmpList)
                    {
                        e.Age = compartment;
                        newCollection.Add(e);
                    }

                    k = ParameterK - 1;
                    tmpList = new ObservableCollection<Person> {p};
                }
            }

            if (tmpList.Count > 0)
            {
                string compartment = tmpList.First().Age + " - " + tmpList.Last().Age;
                foreach (var e in tmpList)
                {
                    e.Age = compartment;
                    newCollection.Add(e);
                }
            }

            People = newCollection;
        }

        #region File operation

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
        #endregion
    }
}
