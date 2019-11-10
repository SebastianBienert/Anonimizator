using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Anonimizator.Helpers;
using Anonimizator.Models;

namespace Anonimizator.Algorithms
{
    public class KTableAnonimzation : IKAnonimization
    {
        private readonly int _maxNumberAddToParameterK = 40;
        private readonly int _parameterK;
        private int _stepKTableAnonimization;

        private readonly List<List<string>> _cityDictionary;
        private readonly List<List<string>> _jobDictionary;

        private readonly (string algorithmName, string firstParameter, string secondParameter) _firstKTableAnonimizationAlgorithm = ("Gender", "", "");
        private readonly IEnumerable<(string algorithmName, string firstParameter, string secondParameter)> _middleKTableAnonimizationAlgorithms =
            new List<(string algorithmName, string firstParameter, string secondParameter)>()
            {
                ("Surname" , "", ""),
                ("FirstName" , "", ""),
                ("Age" , "", ""),
                ("City" , "", ""),
                ("Job" , "", "")
            };

        private readonly List<(string algorithmName, string firstParameter, string secondParameter)> _finalKTableAnonimizationAlgorithms =
            new List<(string algorithmName, string firstParameter, string secondParameter)>()
            {
                ("CharacterMasking" , "Age", ""),
                ("CharacterMasking" , "Surname", ""),
                ("CharacterMasking" , "FirstName", ""),
                ("CharacterMasking" , "City", ""),
                ("CharacterMasking" , "Job", "")
            };
        private List<List<(string algorithmName, string firstParameter, string secondParameter)>> _allPermutation = new List<List<(string algorithmName, string firstParameter, string secondParameter)>>();

        public KTableAnonimzation(int parameterK, List<List<string>> cityDictionary, List<List<string>> jobDictionary)
        {
            _stepKTableAnonimization = 0;
            _parameterK = parameterK;
            _cityDictionary = cityDictionary;
            _jobDictionary = jobDictionary;
            CreateListAlgorithms();
        }

        private void CreateListAlgorithms()
        {
            var listPermutationKTableAnonimizationAlgorithms = Permutation.GetPermutations(_middleKTableAnonimizationAlgorithms).ToList();

            // dodajemy do rozpatrywanych algorytmów takie gdzie K dla kolumn Age, Surname i FirstName jest większe
            for (int i = 0; i <= _maxNumberAddToParameterK; i++)
            {
                foreach (var kTableAnonimizationAlgorithms  in listPermutationKTableAnonimizationAlgorithms)
                {
                    var permutation = kTableAnonimizationAlgorithms.ToList();
                    permutation.Insert(0, _firstKTableAnonimizationAlgorithm);
                    permutation.AddRange(_finalKTableAnonimizationAlgorithms);
                    var result = permutation.Select(p =>
                    {
                        if (p.algorithmName == "Age" || p.algorithmName == "Surname" || p.algorithmName == "FirstName")
                            p.firstParameter = i.ToString();
                        return p;
                    }).ToList();
                    _allPermutation.Add(result);
                }
            }
        }

        public List<Person> GetAnonymizedData(IEnumerable<Person> people)
        {
            return PerformAnonimization(people, SearchForTheBestPermutation(people)).ToList();
        }

        private List<(string, string, string)> SearchForTheBestPermutation(IEnumerable<Person> people)
        {
            var stepsBestAlgoritms = Int32.MaxValue;
            List<(string, string, string)> selectedAlgorithmsList = new List<(string, string, string)>();
            foreach (var algorithmsList in _allPermutation)
            {
                if (algorithmsList.Any(p =>
                    (p.algorithmName == "Age" || p.algorithmName == "Surname" || p.algorithmName == "FirstName") &&
                    Int32.Parse(p.firstParameter) + _parameterK > people.Count()))
                    continue;

                PerformAnonimization(people, algorithmsList);

                if (_stepKTableAnonimization < stepsBestAlgoritms)
                {
                    selectedAlgorithmsList = algorithmsList;
                    stepsBestAlgoritms = _stepKTableAnonimization;
                }

                _stepKTableAnonimization = 0;
            }

            return selectedAlgorithmsList;
        }

        public IEnumerable<Person> PerformAnonimization(IEnumerable<Person> people, List<(string, string, string)> algorithmsList)
        {
            while (CheckConditionOfKAnonimization(people.ToList(), algorithmsList))
            {
                people = PerformAnotherAnonimizationMethod(people.ToList(), algorithmsList);
            }
            return people;
        }

        private bool CheckConditionOfKAnonimization(List<Person> people, List<(string, string, string)> algorithmsList)
        {
            if (_stepKTableAnonimization >= algorithmsList.Count)
                return false;

            return people.GroupBy(p => p.City + p.Age + p.FirstName + p.Job + p.Surname + p.Gender, 
                                     p => p, 
                                     (key, g) => new { key = key, elems = g.ToList()})
                         .Any(g => g.elems.Count < _parameterK);
        }

        private List<Person> PerformAnotherAnonimizationMethod(List<Person> people, List<(string, string, string)> algorithmsList)
        {
            people = DetermineAlgorithm(algorithmsList).GetAnonymizedData(people);
            return people;
        }

        #region Algorithm selection functions 

        private IKAnonimization DetermineAlgorithm(List<(string algorithmName, string firstParameter, string secondParameter)> algorithmsList)
        {
            var kAnonimizationAlgorithm = algorithmsList[_stepKTableAnonimization++];
            switch (kAnonimizationAlgorithm.algorithmName)
            {
                case "Age":
                    return new KNumberAnonimization<string>(_parameterK + CreateNumberAddToParameterK(kAnonimizationAlgorithm), p => p.Age);
                case "City":
                    return SelectCityAlgorithm(kAnonimizationAlgorithm);
                case "FirstName":
                    return new KAttributeLengthAnonimization<string>(_parameterK + CreateNumberAddToParameterK(kAnonimizationAlgorithm),
                        p => p.FirstName);
                case "Surname":
                    return new KAttributeLengthAnonimization<string>(_parameterK + CreateNumberAddToParameterK(kAnonimizationAlgorithm),
                        p => p.Surname);
                case "Job":
                    return SelectJobAlgorithm(kAnonimizationAlgorithm);
                case "Gender":
                    return new CharacterMasking<string>(p => p.Gender);
                case "CharacterMasking":
                    //return new CharacterMasking(kAnonimizationAlgorithm.firstParameter);
                default:
                    return new KNumberAnonimization<string>(_parameterK + CreateNumberAddToParameterK(kAnonimizationAlgorithm), p => p.Age);
            }
        }

        private int CreateNumberAddToParameterK((string, string firstParameter, string) kAnonimizationAlgorithm)
        {
            return string.IsNullOrEmpty(kAnonimizationAlgorithm.firstParameter)
                ? 0
                : Int32.Parse(kAnonimizationAlgorithm.firstParameter);
        }

        private IKAnonimization SelectCityAlgorithm((string, string, string) kAnonimizationAlgorithm)
        {
            var dictionaryColumnRange = CreateDictionaryColumnRange(kAnonimizationAlgorithm);
            return new KCityAnonimization(_parameterK, _cityDictionary, dictionaryColumnRange.firstColumn, dictionaryColumnRange.endColumn);
        }

        private IKAnonimization SelectJobAlgorithm((string, string, string) kAnonimizationAlgorithm)
        {
            var dictionaryColumnRange = CreateDictionaryColumnRange(kAnonimizationAlgorithm);
            return new KJobAnonimization(_parameterK, _jobDictionary, dictionaryColumnRange.firstColumn, dictionaryColumnRange.endColumn);
        }

        private (int firstColumn, int? endColumn) 
            CreateDictionaryColumnRange((string, string firstParameter, string secondParameter) kAnonimizationAlgorithm)
        {
            var firstColumn = string.IsNullOrEmpty(kAnonimizationAlgorithm.firstParameter)
                ? 0
                : Int32.Parse(kAnonimizationAlgorithm.firstParameter);

            var endColumn = string.IsNullOrEmpty(kAnonimizationAlgorithm.secondParameter)
                ? null
                : (int?)Int32.Parse(kAnonimizationAlgorithm.secondParameter);

            return (firstColumn, endColumn);
        }
        #endregion
    }
}
