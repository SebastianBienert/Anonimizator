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

        List<Tuple<string, string, string>> dictionary = new List<Tuple<string, string, string>>()
        {
            Tuple.Create("Katowice", "Slaskie", "Polska" ),
            Tuple.Create("Czestochowa", "Slaskie", "Polska" ),
            Tuple.Create("Rybnik", "Slaskie", "Polska" ),
            Tuple.Create("Gliwice", "Slaskie", "Polska" ),
            Tuple.Create("Zabrze", "Slaskie", "Polska" ),
            Tuple.Create("Bytom", "Slaskie", "Polska" ),
            Tuple.Create("Pszczyna", "Slaskie", "Polska" ),
            Tuple.Create("Tychy", "Slaskie", "Polska" ),
            Tuple.Create("Cieszyn", "Slaskie", "Polska" ),
            Tuple.Create("Wroclaw", "Dolnoslaskie", "Polska" ),
            Tuple.Create("Olawa", "Dolnoslaskie", "Polska" ),
            Tuple.Create("Swidnica", "Dolnoslaskie", "Polska" ),
            Tuple.Create("Walbrzych", "Dolnoslaskie", "Polska" ),
            Tuple.Create("Klodzko", "Dolnoslaskie", "Polska" ),
            Tuple.Create("Lubin", "Dolnoslaskie", "Polska" ),
            Tuple.Create("Luban", "Dolnoslaskie", "Polska" ),
            Tuple.Create("Jelenia Gora", "Dolnoslaskie", "Polska" ),
            Tuple.Create("Warszawa", "Mazowieckie", "Polska" ),
            Tuple.Create("Siedlce", "Mazowieckie", "Polska" ),
            Tuple.Create("Radom", "Mazowieckie", "Polska" ),
            Tuple.Create("Ciechanow", "Mazowieckie", "Polska" ),
            Tuple.Create("Ostroleka", "Mazowieckie", "Polska" ),
            Tuple.Create("Wyszkow", "Mazowieckie", "Polska" ),
            Tuple.Create("Lipsko", "Mazowieckie", "Polska" ),
            Tuple.Create("Piaseczno", "Mazowieckie", "Polska" ),
            Tuple.Create("Proszkow", "Mazowieckie", "Polska" ),
            Tuple.Create("Opole", "Opolskie", "Polska" ),
            Tuple.Create("Brzeg", "Opolskie", "Polska" ),
            Tuple.Create("Prudnik", "Opolskie", "Polska" ),
            Tuple.Create("Nysa", "Opolskie", "Polska" ),
            Tuple.Create("Kluczbork", "Opolskie", "Polska" ),
            Tuple.Create("Glubczyce", "Opolskie", "Polska" ),
            Tuple.Create("Namyslow", "Opolskie", "Polska" ),
            Tuple.Create("Gdansk", "Pomorskie", "Polska" ),
            Tuple.Create("Gdynia", "Pomorskie", "Polska" ),
            Tuple.Create("Puck", "Pomorskie", "Polska" ),
            Tuple.Create("Sopot", "Pomorskie", "Polska" )
        };

        public KAnonimizationViewModel()
        {
            _parameterK = 1;
            ReadData();
            ColumnNames = new ObservableCollection<string> (){ "Age", "City", "Gender" };
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
            if(SelectedColumnName == "Age")
                KAnonimizationAgeColumn();
            else if(SelectedColumnName == "City")
                KAnonimizationCityColumn();
            else if (SelectedColumnName == "Gender")
                CharakterMaskingColumn();
        }

        private void KAnonimizationCityColumn()
        {
            ObservableCollection<Person> newCollection = new ObservableCollection<Person>();
            foreach (var p in People)
            {
                newCollection.Add(p);
            }

            bool needBetterAnonimization = false;

            do
            {
                needBetterAnonimization = false;
                var query = newCollection.GroupBy(
                    p => p.City,
                    p => p.City,
                    (baseCity, cities) => new
                    {
                        Key = baseCity,
                        Count = cities.Count()
                    });

                if (query.Count() == 1)
                    break;

                foreach (var q in query)
                {
                    if (q.Count < ParameterK)
                    {
                        needBetterAnonimization = true;
                        break;
                    }
                }

                if (needBetterAnonimization)
                {
                    foreach (var person in newCollection)
                    {
                        var cityName = person.City;
                        foreach (var row in dictionary)
                        {
                            if (person.City == row.Item1)
                            {
                                person.City = row.Item2;
                                break;
                            }
                            else if (person.City == row.Item2)
                            {
                                person.City = row.Item3;
                                break;
                            }
                        }

                        if (cityName == person.City)
                        {
                            person.City = "Europa";
                        }
                    }
                }
            } while (needBetterAnonimization);

            People = newCollection;
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

        private void CharakterMaskingColumn()
        {
            People = new ObservableCollection<Person>(People.Select(p =>
            {
                p.GetType().GetProperty(SelectedColumnName).SetValue(p, "*");
                return p;
            }));
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
