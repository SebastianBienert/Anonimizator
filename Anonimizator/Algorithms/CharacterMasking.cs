using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Anonimizator.Models;

namespace Anonimizator.Algorithms
{
    public class CharacterMasking : IKAnonimization
    {
        private string ColumnName { get; }

        public CharacterMasking()
        {
            ColumnName = "Gender";
        }

        public CharacterMasking(string columnName)
        {
            ColumnName = columnName;
        }

        public List<Person> GetAnonymizedData(IEnumerable<Person> people)
        {
            return people.Select(p =>
                {
                    p.GetType().GetProperty(ColumnName).SetValue(p, "*");
                    return p;
                })
                .ToList();
        }
    }
}
