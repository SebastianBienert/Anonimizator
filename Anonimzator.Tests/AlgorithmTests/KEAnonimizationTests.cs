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
    public class KEAnonimizationTests
    {
        private readonly List<Person> _people = new List<Person>()
        {
            new Person("M", "Inzynier", "Wroclaw", "Jan", "Nowak", "7"),
            new Person("M", "Inzynier", "Katowice", "Adam", "Nowakowski", "11"),
            new Person("M", "Inzynier", "Wroclaw", "Kamil", "Nowak", "15"),
            new Person("M", "Inzynier", "Czestochowa", "Jakub", "Kowal", "15"),

            new Person("M", "Programista", "Wroclaw", "Szymon", "Kowal", "19"),
            new Person("M", "Programista", "Wroclaw", "Filip", "Kowalski", "22"),
            new Person("M", "Malarz", "Rybnik", "Mikolaj", "Kowalski", "30"),
            new Person("K", "Malarz", "Olawa", "Julia", "Wojcik", "49"),

            new Person("K", "Malarz", "Olawa", "Julia", "Rehocik", "49"),
            new Person("K", "Malarz", "Olawa", "Julia", "Mothas", "49"),
            new Person("K", "Malarz", "Olawa", "Julia", "Albert", "49"),
            new Person("K", "Malarz", "Olawa", "Julia", "Nazerska", "49"),
        };

        private readonly List<Person> _peopleFromFile;
        private readonly List<List<string>> _cityDictionary;
        private readonly List<List<string>> _jobDictionary;

        public KEAnonimizationTests()
        {
            var fileService = new FileService();
            _peopleFromFile = fileService.GetPeopleData();
            _cityDictionary = fileService.GetDictionaryData(ConstantStrings.FILE_WITH_CITY_GENERALIZATION_DICTIONARY);
            _jobDictionary = fileService.GetDictionaryData(ConstantStrings.FILE_WITH_JOB_GENERALIZATION_DICTIONARY);
        }

        [TestMethod]
        public void GivenEmptyPeopleList_ShouldReturn_EmptyList()
        {
            var pid = new Expression<Func<Person, object>>[] { p => p.Job, p => p.City };
            var algorithm = new KEAnonimization(2, 3, _jobDictionary, _cityDictionary, pid, p => p.Age);
            var anonymized = algorithm.GetAnonymizedData(new List<Person>());
            Assert.IsTrue(!anonymized.Any());
        }

        [TestMethod]
        [ExpectedException(typeof(Exception))]
        public void GivenNotIntegerPropertyToAnonymzed_ShouldThrowException()
        {
            var pid = new Expression<Func<Person, object>>[] { p => p.Job, p => p.City };
            var algorithm = new KEAnonimization(2, 3, _jobDictionary, _cityDictionary, pid, p => p.Surname);
            var anonymized = algorithm.GetAnonymizedData(_people);
            Assert.IsTrue(!anonymized.Any());
        }

        [TestMethod]
        public void GivenKParameterOneAndEParametr0_ShouldReturnTheSameList()
        {
            var pid = new Expression<Func<Person, object>>[] { p => p.Job, p => p.City };
            var algorithm = new KEAnonimization(1, 0, _jobDictionary, _cityDictionary, pid, p => p.Age);
            var anonymized = algorithm.GetAnonymizedData(_people);
            Assert.IsTrue(anonymized.Any());
            Assert.IsTrue(_people.Count == anonymized.Count);
            Assert.IsTrue(_people.All(p => anonymized.Exists(x => x.FirstName == p.FirstName && x.Surname == p.Surname &&
                                                                 x.Age == p.Age && x.Job == p.Job && x.City == p.City &&
                                                                 x.Gender == p.Gender)));
        }

        [DataRow(2, 2)]
        [DataRow(2, 3)]
        [DataRow(2, 4)]
        [DataRow(2, 5)]
        [DataRow(3, 2)]
        [DataRow(3, 2)]
        [DataRow(3, 3)]
        [DataRow(3, 4)]
        [DataRow(4, 2)]
        [DataRow(4, 4)]
        [DataRow(4, 5)]
        [DataRow(4, 10)]
        [DataTestMethod]
        public void GivenKParameter_GreaterThan_1_ShouldReturnAnonymizedList(int parameterK, int parameterE)
        {
            var pid = new Expression<Func<Person, object>>[] { p => p.Job, p => p.City };
            var algorithm = new KEAnonimization(parameterK, parameterE, _jobDictionary, _cityDictionary, pid, p => p.Age);

            var anonymzed = algorithm.GetAnonymizedData(_people);

            Assert.AreEqual(_people.Count, anonymzed.Count);
            //All Groups grouped by X set have at least K unique Y set attribute values
            Assert.IsTrue(anonymzed.GroupBy(x => new { x.Job, x.City })
                                   .All(g =>
                                   {
                                       var uniqueYValues = g.GroupBy(p => p.Age).Count();
                                       return uniqueYValues >= parameterK;
                                   }));
            Assert.IsTrue(anonymzed.GroupBy(x => new { x.Job, x.City })
                .All(g =>
                {
                    var max = g.Select(p => Convert.ToInt32(p.Age)).Max();
                    var min = g.Select(p => Convert.ToInt32(p.Age)).Min();
                    return (max - min) >= parameterE;
                }));

        }

        [DataRow(3, 4)]
        [DataRow(4, 2)]
        [DataRow(4, 4)]
        [DataRow(4, 5)]
        [DataRow(4, 10)]
        [DataTestMethod]
        public void GivenKParameter_GreaterThan_1_AndDataFromFile_ShouldReturnAnonymizedList(int parameterK, int parameterE)
        {
            var pid = new Expression<Func<Person, object>>[] { p => p.Job, p => p.City };
            var algorithm = new KEAnonimization(parameterK, parameterE, _jobDictionary, _cityDictionary, pid, p => p.Age);

            var anonymzed = algorithm.GetAnonymizedData(_peopleFromFile);

            Assert.AreEqual(_peopleFromFile.Count, anonymzed.Count);
            //All Groups grouped by X set have at least K unique Y set attribute values
            Assert.IsTrue(anonymzed.GroupBy(x => new { x.Job, x.City })
                .All(g =>
                {
                    var uniqueYValues = g.GroupBy(p => p.Age).Count();
                    return uniqueYValues >= parameterK;
                }));
            Assert.IsTrue(anonymzed.GroupBy(x => new { x.Job, x.City })
                .All(g =>
                {
                    var max = g.Select(p => Convert.ToInt32(p.Age)).Max();
                    var min = g.Select(p => Convert.ToInt32(p.Age)).Min();
                    return (max - min) >= parameterE;
                }));
        }
    }
}
