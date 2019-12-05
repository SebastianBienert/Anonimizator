using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Anonimizator.Core.Algorithms;
using Anonimizator.Core.Helpers;
using Anonimizator.Core.Models;
using Anonimizator.Core.Services;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Anonimzator.Tests.AlgorithmTests
{
    [TestClass]
    public class AKAnonimizationTests
    {
        private readonly List<Person> _people = new List<Person>()
        {
            new Person("K", "Inzynier", "Lipsko", "Jan", "Nowak", "7"),
            new Person("M", "Inzynier", "Wyszkow", "Jan", "Nowak", "11"),
            new Person("M", "Inzynier", "Lipsko", "Jan", "Nowak", "15"),
            new Person("M", "Architekt", "Wyszkow", "Jakub", "Jagiel", "15"),

            new Person("M", "Tancerz", "Wroclaw", "Jakub", "Osowski", "19"),
            new Person("M", "Tancerz", "Wroclaw", "Jan", "Nowak", "22"),
            new Person("K", "Muzyk", "Olawa", "Jakub", "Kowalski", "30"),
            new Person("M", "Malarz", "Olawa", "Jan", "Wojcik", "49"),

            new Person("M", "Polonista", "Rybnik", "Julian", "Osowski", "19"),
            new Person("K", "Historyk", "Rybnik", "Julia", "Nowak", "22"),
            new Person("M", "Matematyk", "Gliwice", "Julian", "Kowalski", "30"),
            new Person("K", "Matematyk", "Gliwice", "Julia", "Wojcik", "49"),
        };
        
        private readonly List<List<string>> _cityDictionary;
        private readonly List<List<string>> _jobDictionary;

        public AKAnonimizationTests()
        {
            var fileService = new FileService();
            _cityDictionary = fileService.GetDictionaryData(ConstantStrings.FILE_WITH_CITY_GENERALIZATION_DICTIONARY);
            _jobDictionary = fileService.GetDictionaryData(ConstantStrings.FILE_WITH_JOB_GENERALIZATION_DICTIONARY);
        }

        [TestMethod]
        public void GivenEmptyPeopleList_ShouldReturn_EmptyList()
        {
            var pid = new Expression<Func<Person, object>>[] { p => p.FirstName, p => p.Surname, p => p.Age };
            var algorithm = new AKAnonimization(3, 0.5, "Programista", _jobDictionary, _cityDictionary, p => p.Job, pid);
            var anonymized = algorithm.GetAnonymizedData(new List<Person>());
            Assert.IsTrue(!anonymized.Any());
        }

        [TestMethod]
        public void GivenKParameterOne_AndAlphaParameterOne_AndValueAttributeProgramista_ShouldReturnTheSameList()
        {
            var pid = new Expression<Func<Person, object>>[] { p => p.FirstName, p => p.Surname, p => p.Age };
            var algorithm = new AKAnonimization(1, 1, "Programista", _jobDictionary, _cityDictionary, p => p.Job, pid);
            var anonymzed = algorithm.GetAnonymizedData(_people);
            Assert.IsTrue(_people.All(p => anonymzed.Exists(x => x.FirstName == p.FirstName && x.Surname == p.Surname && x.Age == p.Age)));
        }

        [DataRow(2)]
        [DataRow(3)]
        [DataRow(4)]
        [DataRow(5)]
        [DataRow(6)]
        [DataRow(7)]
        [DataRow(8)]
        [DataTestMethod]
        public void GivenKParameter_GreaterThan_1_AndAlphaParameter1_AndValueAttributeProgramista_ShouldReturnAnonymizedList(int parameterK)
        {
            var pid = new Expression<Func<Person, object>>[] { p => p.FirstName, p => p.Surname, p => p.Age };
            var algorithm = new AKAnonimization(parameterK, 1, "Programista", _jobDictionary, _cityDictionary, p => p.Job, pid);

            var anonymzed = algorithm.GetAnonymizedData(_people);

            Assert.AreEqual(_people.Count, anonymzed.Count);
            Assert.IsTrue(anonymzed.GroupBy(x => new { x.FirstName, x.Surname, x.Age })
                .Select(g => g.Count())
                .All(c => c >= parameterK));

        }

        [DataRow(0.5, "K")]
        [DataRow(0.75, "M")]
        [DataRow(0.75, "K")]
        [DataRow(1, "K")]
        [DataRow(1, "M")]
        [DataTestMethod]
        [TestMethod]
        public void GivenKParameterFour_AndAlphaParameterGreaterThenHalf_AndValueGenderKOrM_ShouldReturnAnonymizedList(double parameterAlpha, string valueAttribute)
        {
            var parameterK = 4;
            var pid = new Expression<Func<Person, object>>[] { p => p.Job, p => p.City, p => p.FirstName };
            var algorithm = new AKAnonimization(parameterK, parameterAlpha, valueAttribute, _jobDictionary, _cityDictionary, p => p.Gender, pid);
            var anonymzed = algorithm.GetAnonymizedData(_people);
            Assert.AreEqual(_people.Count, anonymzed.Count);
            Assert.IsTrue(anonymzed.GroupBy(x => new { x.Job, x.City, x.FirstName})
                .All(g =>
                {
                    var numberSearchValue = g.Count(p => p.Gender == valueAttribute);
                    var groupSize = g.Count();
                    var alphaParameterCondition = numberSearchValue / (double)groupSize <= parameterAlpha;
                    var kParameterCondition = groupSize >= parameterK;
                    return alphaParameterCondition && kParameterCondition;
                }));
        }
    }
}
