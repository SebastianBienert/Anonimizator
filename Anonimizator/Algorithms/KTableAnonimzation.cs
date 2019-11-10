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
        private readonly int NumberAddToParameterK = 40;
        public int ParameterK { get; }
        private int stepKTableAnonimization;

        private List<List<string>> _cityDictionary;
        private List<List<string>> _jobDictionary;

        private (string algorithmName, string firstParameter, string secondParameter) firstAlgorithm = ("Gender", "", "");
        private IEnumerable<(string algorithmName, string firstParameter, string secondParameter)> _queueKTableAnonimization =
            new List<(string algorithmName, string firstParameter, string secondParameter)>()
            {
                ("Surname" , "", ""),
                ("FirstName" , "", ""),
                ("Age" , "", ""),
                ("City" , "", ""),
                ("Job" , "", "")
            };

        private List<(string algorithmName, string firstParameter, string secondParameter)> _queueEndKTableAnonimization =
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
            stepKTableAnonimization = 0;
            ParameterK = parameterK;
            _cityDictionary = cityDictionary;
            _jobDictionary = jobDictionary;
            CreateListAlgorithms();
        }

        private void CreateListAlgorithms()
        {
            var tmp = Permutation.GetPermutations(_queueKTableAnonimization).ToList();

            // dodajemy do rozpatrywanych algorytmów takie gdzie K dla kolumn Age, Surname i FirstName jest większe
            for (int i = 0; i <= NumberAddToParameterK; i++)
            {
                foreach (var e in tmp)
                {
                    var permutation = e.ToList();
                    permutation.Insert(0, firstAlgorithm);
                    permutation.AddRange(_queueEndKTableAnonimization);
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
                    Int32.Parse(p.firstParameter) + ParameterK > people.Count()))
                    continue;

                PerformAnonimization(people, algorithmsList);

                if (stepKTableAnonimization < stepsBestAlgoritms)
                {
                    selectedAlgorithmsList = algorithmsList;
                    stepsBestAlgoritms = stepKTableAnonimization;
                }

                stepKTableAnonimization = 0;
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

        private bool CheckConditionOfKAnonimization(List<Person> people, List<(string algorithmName, string firstParameter, string secondParameter)> algorithmsList)
        {
            if (stepKTableAnonimization >= algorithmsList.Count)
                return false;

            var grups = people.GroupBy(p => p.City + p.Age + p.FirstName + p.Job + p.Surname + p.Gender, 
                                     p => p, 
                                     (key, g) => new { key = key, elems = g.ToList()});
            return grups.Any(g => g.elems.Count < ParameterK);
        }

        private List<Person> PerformAnotherAnonimizationMethod(List<Person> people, List<(string, string, string)> algorithmsList)
        {
            people = DetermineAlgorithm(algorithmsList).GetAnonymizedData(people);
            return people;
        }

        #region Algorithm selection functions 

        private IKAnonimization DetermineAlgorithm(List<(string algorithmName, string firstParameter, string secondParameter)> algorithmsList)
        {
            var kAnonimizationAlgorithm = algorithmsList[stepKTableAnonimization++];
            switch (kAnonimizationAlgorithm.algorithmName)
            {
                case "Age":
                    return new KAgeAnonimzation_V2(ParameterK + CreateNumberAddToParameterK(kAnonimizationAlgorithm));
                case "City":
                    return SelectCityAlgorithm(kAnonimizationAlgorithm);
                case "FirstName":
                    return new KAttributeLengthAnonimization<string>(ParameterK + CreateNumberAddToParameterK(kAnonimizationAlgorithm),
                        p => p.FirstName);
                case "Surname":
                    return new KAttributeLengthAnonimization<string>(ParameterK + CreateNumberAddToParameterK(kAnonimizationAlgorithm),
                        p => p.Surname);
                case "Job":
                    return SelectJobAlgorithm(kAnonimizationAlgorithm);
                case "Gender":
                    return new CharacterMasking();
                case "CharacterMasking":
                    return new CharacterMasking(kAnonimizationAlgorithm.firstParameter);
                default:
                    return new KAgeAnonimzation_V2(ParameterK + CreateNumberAddToParameterK(kAnonimizationAlgorithm));
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
            return new KCityAnonimization(ParameterK, _cityDictionary, dictionaryColumnRange.firstColumn, dictionaryColumnRange.endColumn);
        }

        private IKAnonimization SelectJobAlgorithm((string, string, string) kAnonimizationAlgorithm)
        {
            var dictionaryColumnRange = CreateDictionaryColumnRange(kAnonimizationAlgorithm);
            return new KJobAnonimization(ParameterK, _jobDictionary, dictionaryColumnRange.firstColumn, dictionaryColumnRange.endColumn);
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
