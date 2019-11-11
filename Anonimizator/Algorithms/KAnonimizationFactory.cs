using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Anonimizator.Helpers;
using Anonimizator.Models;
using Anonimizator.Services;
using Neleus.LambdaCompare;

namespace Anonimizator.Algorithms
{
    public class KAnonimizationFactory : IEnumerable<List<IKAnonimization>>
    {
        private int _parameterK;
        private readonly int _maxKParameterK;
        private readonly FileService _fileService;
        private int _index;
        private readonly List<IEnumerable<Expression<Func<Person, object>>>> _allCombinations;
        private readonly List<List<string>> _cityDictionary;
        private readonly List<List<string>> _jobDictionary;

        public KAnonimizationFactory(int maxKParameterK, FileService fileService, params Expression<Func<Person, object>>[] pidProperties)
        {
            _parameterK = 1;
            _allCombinations = pidProperties.ToList().GetAllCombinations().ToList();
            _maxKParameterK = maxKParameterK;
            _fileService = fileService;
            _cityDictionary = _fileService.GetDictionaryData(ConstantStrings.FILE_WITH_CITY_GENERALIZATION_DICTIONARY);
            _jobDictionary = _fileService.GetDictionaryData(ConstantStrings.FILE_WITH_JOB_GENERALIZATION_DICTIONARY);
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
            if(Lambda.Eq(property, p => p.Job))
                return new KJobAnonimization(_parameterK, _jobDictionary);
            if (Lambda.Eq(property, p => p.City))
                return new KCityAnonimization(_parameterK, _cityDictionary);
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
