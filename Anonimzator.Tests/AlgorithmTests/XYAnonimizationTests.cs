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
    public class XYAnonimizationTests
    {
        private readonly List<Person> _people = new List<Person>()
        {
            new Person("M", "Inzynier", "Wroclaw", "Jan", "Nowak", "7"),
            new Person("M", "Inzynier", "Katowice", "Jan", "Nowak", "11"),
            new Person("M", "Inzynier", "Warszawa", "Jan", "Nowak", "15"),
            new Person("M", "Inzynier", "Wroclaw", "Karol", "Nowak", "15"),
            new Person("M", "Inzynier", "Warszawa", "Jacek", "Nowak", "15"),
            new Person("M", "Inzynier", "Katowice", "Marcin", "Nowak", "15"),

            new Person("M", "Inzynier", "Czestochowa", "Jakub", "Jagiel", "15"),

            new Person("M", "Programista", "Wroclaw", "Szymon", "Osowski", "19"),
            new Person("M", "Programista", "Wroclaw", "Filip", "Nowak", "22"),
            new Person("M", "Malarz", "Rybnik", "Mikolaj", "Kowalski", "30"),
            new Person("K", "Malarz", "Olawa", "Julia", "Wojcik", "49"),
        };

        private readonly List<Person> _peopleFromFile;
        private readonly List<List<string>> _cityDictionary;
        private readonly List<List<string>> _jobDictionary;

        public XYAnonimizationTests()
        {
            var fileService = new FileService();
            _peopleFromFile = fileService.GetPeopleData();
            _cityDictionary = fileService.GetDictionaryData(ConstantStrings.FILE_WITH_CITY_GENERALIZATION_DICTIONARY);
            _jobDictionary = fileService.GetDictionaryData(ConstantStrings.FILE_WITH_JOB_GENERALIZATION_DICTIONARY);
        }

        [TestMethod]
        [ExpectedException(typeof(Exception))]
        public void GivenJointXYSets_ShouldThrowException()
        {
            var xProperties = new Expression<Func<Person, object>>[] { p => p.FirstName, p => p.Surname, p => p.Age };
            var yProperties = new Expression<Func<Person, object>>[] { p => p.FirstName, p => p.Job};
            var algorithm = new XYAnonimization(3, _jobDictionary, _cityDictionary, xProperties, yProperties);
        }

        [TestMethod]
        public void GivenEmptyPeopleList_ShouldReturn_EmptyList()
        {
            var xProperties = new Expression<Func<Person, object>>[] { p => p.Job, p => p.Surname };
            var yProperties = new Expression<Func<Person, object>>[] { p => p.Age};
            var algorithm = new XYAnonimization(3, _jobDictionary, _cityDictionary, xProperties, yProperties);
            var anonymized = algorithm.GetAnonymizedData(new List<Person>());
            Assert.IsTrue(!anonymized.Any());
        }

        [TestMethod]
        public void GivenKParameterOne_ShouldReturnTheSameList()
        {
            var xProperties = new Expression<Func<Person, object>>[] { p => p.Job, p => p.Surname };
            var yProperties = new Expression<Func<Person, object>>[] { p => p.Age };
            var algorithm = new XYAnonimization(1, _jobDictionary, _cityDictionary, xProperties, yProperties);
            var anonymzed = algorithm.GetAnonymizedData(_people);
            Assert.IsTrue(_people.Count == anonymzed.Count);
            Assert.IsTrue(_people.All(p => anonymzed.Exists(x => x.FirstName == p.FirstName && x.Surname == p.Surname &&
                                                                 x.Age == p.Age && x.Job == p.Job && x.City == p.City &&
                                                                 x.Gender == p.Gender)));
        }

        [DataRow(2)]
        [DataRow(3)]
        [DataRow(4)]
        [DataRow(5)]
        [DataRow(6)]
        [DataRow(7)]
        [DataTestMethod]
        public void GivenKParameter_GreaterThan_1_ShouldReturnAnonymizedList(int parameterK)
        {
            var xProperties = new Expression<Func<Person, object>>[] { p => p.Job, p => p.Surname };
            var yProperties = new Expression<Func<Person, object>>[] { p => p.Age };
            var algorithm = new XYAnonimization(parameterK, _jobDictionary, _cityDictionary, xProperties, yProperties);

            var anonymzed = algorithm.GetAnonymizedData(_people);

            Assert.AreEqual(_people.Count, anonymzed.Count);
            //All Groups grouped by X set have at least K unique Y set attribute values
            Assert.IsTrue(anonymzed.GroupBy(x => new { x.Job, x.Surname })
                                   .All(g =>
                                    {
                                        var uniqueYValues = g.GroupBy(p => p.Age).Count();
                                        return uniqueYValues >= parameterK;
                                    }));

        }

        [DataRow(2)]
        [DataRow(3)]
        [DataRow(4)]
        [DataRow(5)]
        [DataRow(6)]
        [DataRow(7)]
        [DataRow(8)]
        [DataTestMethod]
        public void GivenKParameter_GreaterThan_1_AndDataFromFile_ShouldReturnAnonymizedList(int parameterK)
        {
            var xProperties = new Expression<Func<Person, object>>[] { p => p.Job, p => p.Surname };
            var yProperties = new Expression<Func<Person, object>>[] { p => p.Age };
            var algorithm = new XYAnonimization(parameterK, _jobDictionary, _cityDictionary, xProperties, yProperties);

            var anonymzed = algorithm.GetAnonymizedData(_peopleFromFile);

            Assert.AreEqual(_peopleFromFile.Count, anonymzed.Count);
            //All Groups grouped by X set have at least K unique Y set attribute values
            Assert.IsTrue(anonymzed.GroupBy(x => new { x.Job, x.Surname })
                .All(g =>
                {
                    var uniqueYValues = g.GroupBy(p => p.Age).Count();
                    return uniqueYValues >= parameterK;
                }));
        }
    }
}
