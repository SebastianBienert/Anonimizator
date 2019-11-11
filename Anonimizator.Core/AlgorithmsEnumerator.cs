using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Anonimizator.Core.Algorithms;
using Anonimizator.Core.Helpers;
using Anonimizator.Core.Models;
using Neleus.LambdaCompare;

namespace Anonimizator.Algorithms
{
    public class AlgorithmsEnumerator : IEnumerable<List<IKAnonimization>>
    {
        private int _parameterK;
        private readonly int _maxKParameterK;
        private int _index;
        private readonly List<IEnumerable<Expression<Func<Person, object>>>> _allCombinations;
        private readonly Dictionary<Expression<Func<Person, object>>, List<List<string>>> _dictionaries;

        public AlgorithmsEnumerator(int maxKParameterK, 
            Dictionary<Expression<Func<Person, object>>, List<List<string>>> dictionaries,
            params Expression<Func<Person, object>>[] pidProperties)
        {
            _parameterK = 1;
            _allCombinations = pidProperties.ToList().GetAllCombinations().ToList();
            _maxKParameterK = maxKParameterK;
            _dictionaries = dictionaries;
        }

        public IEnumerator<List<IKAnonimization>> GetEnumerator()
        {
            while (_parameterK < _maxKParameterK)
            {
                var algorithmsImplementation = GetAlgorithmsFromCombination(_allCombinations[_index % _allCombinations.Count]);
                
               _index++;
                if (_index % _allCombinations.Count == 0)
                    _parameterK++;

                yield return algorithmsImplementation;
            }
        }

        private List<IKAnonimization> GetAlgorithmsFromCombination(
            IEnumerable<Expression<Func<Person, object>>> algorithms)
        {
            var implementations = algorithms.Select(GetAlgorithm).ToList();
            return implementations;
        }

        private IKAnonimization GetAlgorithm(Expression<Func<Person, object>> property)
        {
            if(Lambda.Eq(property, p => p.Age))
                return new KNumberAnonimization<string>(_parameterK, p => p.Age);
            if(Lambda.Eq(property, p => p.FirstName))
                return new KAttributeLengthAnonimization<string>(_parameterK, p => p.FirstName);
            if(Lambda.Eq(property, p => p.Surname))
                return new KAttributeLengthAnonimization<string>(_parameterK, p => p.Surname);
            if(Lambda.Eq(property, p => p.Job) && _dictionaries.ContainsKey(p => p.Job))
                return new KJobAnonimization(_parameterK, _dictionaries[p => p.Job]);
            if (Lambda.Eq(property, p => p.City) && _dictionaries.ContainsKey(p => p.City))
                return new KCityAnonimization(_parameterK, _dictionaries[p => p.City]);
            if (Lambda.Eq(property, p => p.Gender))
                return new CharacterMasking<string>(p => p.Gender);

            throw new Exception($"No algorithm for {property} defined");
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
