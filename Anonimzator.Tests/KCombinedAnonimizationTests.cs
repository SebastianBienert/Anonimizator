using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text.RegularExpressions;
using Anonimizator.Algorithms;
using Anonimizator.Models;
using Anonimizator.Services;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Anonimizator.Helpers;

namespace Anonimzator.Tests
{
    [TestClass]
    public class KCombinedAnonimizationTests
    {
        private readonly List<Person> _people = new List<Person>()
        {
            new Person("M", "Inzynier", "Wroclaw", "Jan", "Nowak", "7"),
            new Person("M", "Inzynier", "Katowice", "Jan", "Nowak", "11"),
            new Person("M", "Inzynier", "Wroclaw", "Jan", "Nowak", "15"),
            new Person("M", "Inzynier", "Czestochowa", "Jakub", "Jagiel", "15"),

            new Person("M", "Programista", "Wroclaw", "Szymon", "Osowski", "19"),
            new Person("M", "Programista", "Wroclaw", "Filip", "Nowak", "22"),
            new Person("M", "Malarz", "Rybnik", "Mikolaj", "Kowalski", "30"),
            new Person("K", "Malarz", "Olawa", "Julia", "Wojcik", "49"),
        };

        private readonly List<Person> _peopleFromFile;
        private List<List<string>> _cityDictionary;
        private List<List<string>> _jobDictionary;

        public KCombinedAnonimizationTests()
        {
            var fileService = new FileService();
            _peopleFromFile = fileService.GetPeopleData(ConstantStrings.FILE_WITH_DATA);
            _cityDictionary = fileService.GetDictionaryData(ConstantStrings.FILE_WITH_CITY_GENERALIZATION_DICTIONARY);
            _jobDictionary = fileService.GetDictionaryData(ConstantStrings.FILE_WITH_JOB_GENERALIZATION_DICTIONARY);
        }
        

        [TestMethod]
        public void GivenEmptyPeopleList_ShouldReturn_EmptyList()
        {
            var algorithm = new KCombinedAnonimization(3, _jobDictionary, _cityDictionary, p => p.FirstName, p => p.Surname, p => p.Age);
            var anonymized = algorithm.GetAnonymizedData(new List<Person>());
            Assert.IsTrue(!anonymized.Any());
        }

        [TestMethod]
        public void GivenKParameterOne_ShouldReturnTheSameList()
        {
            var pid = new Expression<Func<Person, object>>[] { p => p.FirstName, p => p.Surname, p => p.Age };
            var algorithm = new KCombinedAnonimization(1, _jobDictionary, _cityDictionary, pid);
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
        public void GivenKParameter_GreaterThan_1_ShouldReturnAnonymizedList(int parameterK)
        {
            var pid = new Expression<Func<Person, object>>[] { p => p.FirstName, p => p.Surname, p => p.Age };
            var algorithm = new KCombinedAnonimization(parameterK, _jobDictionary, _cityDictionary, pid);

            var anonymzed = algorithm.GetAnonymizedData(_people);

            Assert.AreEqual(_people.Count, anonymzed.Count);
            //All Groups have at least K people
            Assert.IsTrue(anonymzed.GroupBy(x => new {x.FirstName, x.Surname, x.Age})
                                   .Select(g => g.Count())
                                   .All(c => c >= parameterK));

        }

        [DataRow(2)]
        [DataRow(3)]
        [DataRow(4)]
        [DataRow(5)]
        [DataRow(6)]
        [DataRow(7)]
        [DataRow(8)]
        [DataTestMethod]
        public void GivenKParameter_GreaterThan_1_AndFullPID_ShouldReturnAnonymizedList(int parameterK)
        {
            var pid = new Expression<Func<Person, object>>[] { p => p.FirstName, p => p.Surname, p => p.Age, p => p.Job, p => p.City, p => p.Gender };
            var algorithm = new KCombinedAnonimization(parameterK, _jobDictionary, _cityDictionary, pid);

            var anonymzed = algorithm.GetAnonymizedData(_people);

            Assert.AreEqual(_people.Count, anonymzed.Count);
            //All Groups have at least K people
            Assert.IsTrue(anonymzed.GroupBy(x => new { x.FirstName, x.Surname, x.Age, x.Job, x.City, x.Gender})
                .Select(g => g.Count())
                .All(c => c >= parameterK));

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
            var pid = new Expression<Func<Person, object>>[] { p => p.FirstName, p => p.Surname };
            var algorithm = new KCombinedAnonimization(parameterK, _jobDictionary, _cityDictionary, pid);

            var anonymzed = algorithm.GetAnonymizedData(_peopleFromFile);

            Assert.AreEqual(_peopleFromFile.Count, anonymzed.Count);
            //All Groups have at least K people
            Assert.IsTrue(anonymzed.GroupBy(x => new { x.FirstName, x.Surname })
                .Select(g => g.Count())
                .All(c => c >= parameterK));
        }

    }
}
