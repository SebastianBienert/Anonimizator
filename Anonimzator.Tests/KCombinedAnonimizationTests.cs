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

        public KCombinedAnonimizationTests()
        {
            _peopleFromFile = new FileService().GetPeopleData(ConstantStrings.FILE_WITH_DATA);
        }
        

        [TestMethod]
        public void GivenEmptyPeopleList_ShouldReturn_EmptyList()
        {
            var algorithm = new KCombinedAnonimization(3, new FileService(), p => p.FirstName, p => p.Surname, p => p.Age);
            var anonymized = algorithm.GetAnonymizedData(new List<Person>());
            Assert.IsTrue(!anonymized.Any());
        }

        [TestMethod]
        public void GivenKParameterOne_ShouldReturnTheSameList()
        {
            var pid = new Expression<Func<Person, object>>[] { p => p.FirstName, p => p.Surname, p => p.Age };
            var algorithm = new KCombinedAnonimization(1, new FileService(), pid);
            var anonymzed = algorithm.GetAnonymizedData(_people);
            Assert.IsTrue(_people.All(p => anonymzed.Exists(x => x.FirstName == p.FirstName && x.Surname == p.Surname && x.Age == p.Age)));
        }

        [DataRow(2)]
        [DataRow(3)]
        [DataRow(4)]
        [DataRow(5)]
        [DataTestMethod]
        public void GivenKParameter_GreaterThan_1_ShouldReturnAnonymizedList(int parameterK)
        {
            var pid = new Expression<Func<Person, object>>[] { p => p.FirstName, p => p.Surname, p => p.Age };
            var algorithm = new KCombinedAnonimization(parameterK, new FileService(), pid);

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
            var algorithm = new KCombinedAnonimization(parameterK, new FileService(), pid);

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
            var algorithm = new KCombinedAnonimization(parameterK, new FileService(), pid);

            var anonymzed = algorithm.GetAnonymizedData(_peopleFromFile);

            Assert.AreEqual(_peopleFromFile.Count, anonymzed.Count);
            //All Groups have at least K people
            Assert.IsTrue(anonymzed.GroupBy(x => new { x.FirstName, x.Surname })
                .Select(g => g.Count())
                .All(c => c >= parameterK));
        }

    }
}
