using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Anonimizator.Models;

namespace Anonimizator.Algorithms
{
    class KJobAnonimization : IKAnonimization
    {
        private readonly List<List<string>> _dictionary;
        public int ParameterK { get; }
        public int FirstDictionaryColumn { get; }
        public int? LastDictionaryColumn { get; }

        public KJobAnonimization(int parameterK, List<List<string>> dictionary, int firstDictionaryColumn = 0, int? lastDictionaryColumn = null)
        {
            ParameterK = parameterK;
            _dictionary = dictionary;
            FirstDictionaryColumn = firstDictionaryColumn;
            LastDictionaryColumn = lastDictionaryColumn;
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
                    p => p.Job,
                    p => p.Job,
                    (baseJob, jobs) => new
                    {
                        Key = baseJob,
                        Count = jobs.Count()
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
                            var numberColumns = LastDictionaryColumn != null && LastDictionaryColumn <= row.Count - 1 ? LastDictionaryColumn : row.Count - 1;

                            for (int i = FirstDictionaryColumn; !change && i < numberColumns; i++)
                            {
                                if (person.Job == row[i])
                                {
                                    person.Job = row[ i + 1 ];
                                    change = true;
                                }
                            }

                            if (change)
                                break;
                        }

                        if (!change)
                        {
                            person.Job = "Praca";
                        }
                    }
                }
            } while (needBetterAnonimization);

            return newCollection;
        }
    }
}
