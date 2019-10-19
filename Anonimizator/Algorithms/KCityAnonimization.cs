using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Anonimizator.Models;

namespace Anonimizator.Algorithms
{
    class KCityAnonimization : IKAnonimization
    {
        private List<Tuple<string, string, string>> _dictionary = new List<Tuple<string, string, string>>()
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
        public int ParameterK { get; }
        public IEnumerable<Person> People { get; }

        public KCityAnonimization(int parameterK, IEnumerable<Person> people)
        {
            ParameterK = parameterK;
            People = people;
        }

        public List<Person> GetAnonymizedData()
        {
            List<Person> newCollection = new List<Person>();
            foreach (var p in People)
            {
                newCollection.Add(p);
            }

            bool needBetterAnonimization;
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
                        foreach (var row in _dictionary)
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

            return newCollection;
        }
    }
}
