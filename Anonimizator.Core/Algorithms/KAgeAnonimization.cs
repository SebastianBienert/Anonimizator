using System.Collections.Generic;
using System.Linq;
using Anonimizator.Core.Models;

namespace Anonimizator.Core.Algorithms
{
    public class KAgeAnonimization : IKAnonimization
    {
        public int ParameterK { get;}

        public KAgeAnonimization(int parameterK)
        {
            ParameterK = parameterK;
        }

        public List<Person> GetAnonymizedData(IEnumerable<Person> people)
        {
            var lp = people.OrderBy(c => int.Parse(c.Age)).ToList();
            List<Person> tmpList = new List<Person>();
            List<Person> newCollection = new List<Person>();

            int k = ParameterK;
            for (int i = 0; i < lp.Count; i++)
            {
                var p = lp[i];
                if (k > 0 || (tmpList.LastOrDefault() != null && tmpList.Last().Age == p.Age) || (lp.Count - i < ParameterK) || (tmpList.First().Age == tmpList.Last().Age))
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
                    tmpList = new List<Person> { p };
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

            return newCollection;
        }
    }
}
