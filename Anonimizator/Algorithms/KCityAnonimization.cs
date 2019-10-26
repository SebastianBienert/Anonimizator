﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Anonimizator.Models;

namespace Anonimizator.Algorithms
{
    class KCityAnonimization : IKAnonimization
    {
        private readonly List<List<string>> _dictionary;
        public int ParameterK { get; }

        public KCityAnonimization(int parameterK, List<List<string>> dictionary)
        {
            ParameterK = parameterK;
            _dictionary = dictionary;
        }

        public List<Person> GetAnonymizedData(IEnumerable<Person> people)
        {
            List<Person> newCollection = new List<Person>();
            foreach (var p in people)
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
                    bool change;
                    foreach (var person in newCollection)
                    {
                        change = false;
                        foreach (var row in _dictionary)
                        {
                            for (int i = 0; !change && i < row.Count - 1; i++)
                            {
                                if (person.City == row[i])
                                {
                                    person.City = row[ i + 1 ];
                                    change = true;
                                }
                            }

                            if (change)
                                break;
                        }

                        if (!change)
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
