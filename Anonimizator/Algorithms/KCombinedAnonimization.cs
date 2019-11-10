using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Anonimizator.Helpers;
using Anonimizator.Models;

namespace Anonimizator.Algorithms
{
    public class KCombinedAnonimization<T> : IKAnonimization
    {
        public int ParameterK { get; }
        public List<Func<Person,T>> _properties { get; set; }
        public List<Expression<Func<Person, T>>> _expressions { get; set; }

        public KCombinedAnonimization(int parameterK, params Expression<Func<Person, T>>[] pidProperties)
        {
            ParameterK = parameterK;
            _expressions = pidProperties.ToList();
            _properties = pidProperties.Select(x => x.Compile()).ToList();
        }

        public List<Person> GetAnonymizedData(IEnumerable<Person> people)
        {
            if (people == null || !people.Any())
                return new List<Person>();
            
            return new List<Person>();
        }
    }
}
