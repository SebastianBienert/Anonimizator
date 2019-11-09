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

        private List<(string algorithmName, string parameter)> _queueKTableAnonimization =
            new List<(string algorithmName, string parameter)>()
            {
                ("Gender" , ""),
                ("Surname" , ""),
                ("FirstName" , ""),
                ("Age" , ""),
                ("City" , ""),
                ("Job" , ""),
                ("CharacterMasking" , "Age"),
                ("CharacterMasking" , "Surname"),
                ("CharacterMasking" , "FirstName"),
                ("CharacterMasking" , "City"),
                ("CharacterMasking" , "Job")
            };

        private List<List<string>> _cityDictionary;
        private List<List<string>> _jobDictionary;

        public KTableAnonimzation(int parameterK, List<List<string>> cityDictionary, List<List<string>> jobDictionary)
        {
            stepKTableAnonimization = 0;
            ParameterK = parameterK;
            _cityDictionary = cityDictionary;
            _jobDictionary = jobDictionary;
        }

        public List<Person> GetAnonymizedData(IEnumerable<Person> people)
        {
            while (CheckConditionOfKAnonimization(people.ToList()))
            {
                people = PerformAnotherAnonimizationMethod(people.ToList());
            }

            return people.ToList();
        }

        private bool CheckConditionOfKAnonimization(List<Person> people)
        {
            if (stepKTableAnonimization >= _queueKTableAnonimization.Count)
                return false;

            var tmp = people.GroupBy(p => p.City + p.Age + p.FirstName + p.Job + p.Surname + p.Gender, p => p.Age, (key, g) => new { key = key, elems = g.ToList()});
            return tmp.Any(g => g.elems.Count < ParameterK);
        }

        private List<Person> PerformAnotherAnonimizationMethod(List<Person> people)
        {
            people = DetermineAlgorithm().GetAnonymizedData(people);
            return people;
        }



        private IKAnonimization DetermineAlgorithm()
        {
            var kAnonimizationAlgorithm = _queueKTableAnonimization[stepKTableAnonimization++];
            switch (kAnonimizationAlgorithm.algorithmName)
            {
                case "Age":
                    return new KAgeAnonimization(ParameterK);
                case "City":
                    return new KCityAnonimization(ParameterK, _cityDictionary);
                case "FirstName":
                    return new KAttributeLengthAnonimization<string>(ParameterK, p => p.FirstName);
                case "Surname":
                    return new CommonStartStringMasking<string>(ParameterK, p => p.Surname);
                case "Job":
                    return new KJobAnonimization(ParameterK, _jobDictionary);
                case "Gender":
                    return new CharacterMasking();
                case "CharacterMasking":
                    return new CharacterMasking(kAnonimizationAlgorithm.parameter);
                default:
                    return new KAgeAnonimization(ParameterK);
            }
        }

    }
}
