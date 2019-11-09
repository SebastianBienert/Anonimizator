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
        public int ParameterK { get; }
        private int stepKTableAnonimization;

        private (string algorithmName, string firstParameter, string secondParameter) firstAlgorithm = ("Gender", "", "");
        private IEnumerable<(string algorithmName, string firstParameter, string secondParameter)> _queueKTableAnonimization =
            new List<(string algorithmName, string firstParameter, string secondParameter)>()
            {
                ("Surname" , "", ""),
                ("FirstName" , "", ""),
                ("Age" , "", ""),
                ("City" , "0", "1"),
                ("Job" , "0", "1"),
                ("City" , "1", "2"),
                ("Job" , "1", "2")
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

        private List<List<string>> _cityDictionary;
        private List<List<string>> _jobDictionary;
        private List<List<(string algorithmName, string firstParameter, string secondParameter)>> _allPermutation = new List<List<(string algorithmName, string firstParameter, string secondParameter)>>();

        private List<(string algorithmName, string firstParameter, string secondParameter)> _selectedAlgorithmPermutation;

        public KTableAnonimzation(int parameterK, List<List<string>> cityDictionary, List<List<string>> jobDictionary)
        {
            stepKTableAnonimization = 0;
            ParameterK = parameterK;
            _cityDictionary = cityDictionary;
            _jobDictionary = jobDictionary;
            var tmp = Permutation.GetPermutations(_queueKTableAnonimization).ToList();
            foreach (var e in tmp)
            {
                var permutation = e.ToList();
                permutation.Insert(0, firstAlgorithm);
                permutation.AddRange(_queueEndKTableAnonimization);
                _allPermutation.Add(permutation);
            }
        }

        public List<Person> GetAnonymizedData(IEnumerable<Person> people)
        {
            SearchForTheBestPermutation(people);

            while (CheckConditionOfKAnonimization(people.ToList(), _selectedAlgorithmPermutation))
            {
                people = PerformAnotherAnonimizationMethod(people.ToList(), _selectedAlgorithmPermutation);
            }

            return people.ToList();
        }

        private void SearchForTheBestPermutation(IEnumerable<Person> people)
        {
            var stepsBestAlgoritms = Int32.MaxValue;
            foreach (var algorithmsList in _allPermutation)
            {
                var tmpPeopleList = people.ToList();
                while (CheckConditionOfKAnonimization(tmpPeopleList, algorithmsList))
                {
                    tmpPeopleList = PerformAnotherAnonimizationMethod(tmpPeopleList, algorithmsList);
                }

                if (stepKTableAnonimization < stepsBestAlgoritms)
                {
                    _selectedAlgorithmPermutation = algorithmsList;
                    stepsBestAlgoritms = stepKTableAnonimization;
                }

                stepKTableAnonimization = 0;
            }
        }

        private bool CheckConditionOfKAnonimization(List<Person> people, List<(string algorithmName, string firstParameter, string secondParameter)> algorithmsList)
        {
            if (stepKTableAnonimization >= algorithmsList.Count)
                return false;

            var tmp = people.GroupBy(p => p.City + p.Age + p.FirstName + p.Job + p.Surname + p.Gender, 
                                     p => p, 
                                     (key, g) => new { key = key, elems = g.ToList()});
            return tmp.Any(g => g.elems.Count < ParameterK);
        }

        private List<Person> PerformAnotherAnonimizationMethod(List<Person> people, List<(string algorithmName, string firstParameter, string secondParameter)> algorithmsList)
        {
            people = DetermineAlgorithm(algorithmsList).GetAnonymizedData(people);
            return people;
        }

        private IKAnonimization DetermineAlgorithm(List<(string algorithmName, string firstParameter, string secondParameter)> algorithmsList)
        {
            var kAnonimizationAlgorithm = algorithmsList[stepKTableAnonimization++];
            switch (kAnonimizationAlgorithm.algorithmName)
            {
                case "Age":
                    return new KAgeAnonimzation_V2(ParameterK);
                case "City":
                    return new KCityAnonimization(ParameterK, _cityDictionary, 
                                                  string.IsNullOrEmpty(kAnonimizationAlgorithm.firstParameter) ? 0 : Int32.Parse(kAnonimizationAlgorithm.firstParameter),
                                                  string.IsNullOrEmpty(kAnonimizationAlgorithm.secondParameter) ? 0 : Int32.Parse(kAnonimizationAlgorithm.secondParameter));
                case "FirstName":
                    return new KAttributeLengthAnonimization<string>(ParameterK, p => p.FirstName);
                case "Surname":
                    return new CommonStartStringMasking<string>(ParameterK, p => p.Surname);
                case "Job":
                    return new KJobAnonimization(ParameterK, _jobDictionary, 
                                                 string.IsNullOrEmpty(kAnonimizationAlgorithm.firstParameter) ? 0 : Int32.Parse(kAnonimizationAlgorithm.firstParameter),
                                                 string.IsNullOrEmpty(kAnonimizationAlgorithm.secondParameter) ? 0 : Int32.Parse(kAnonimizationAlgorithm.secondParameter));
                case "Gender":
                    return new CharacterMasking();
                case "CharacterMasking":
                    return new CharacterMasking(kAnonimizationAlgorithm.firstParameter);
                default:
                    return new KAgeAnonimization(ParameterK);
            }
        }

    }
}
