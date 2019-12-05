using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Anonimizator.Core;
using Anonimizator.Core.Algorithms;
using Anonimizator.Core.Helpers;
using Anonimizator.Core.Models;
using Anonimizator.Core.Services;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Anonimzator.Tests.AlgorithmTests
{
    [TestClass]
    public class RecognitionParameterKTests
    {
        private readonly List<Person> _people = new List<Person>()
        {
            new Person("K", "Inzynier", "Lipsko", "Julia", "Nowak", "7"),
            new Person("K", "Inzynier", "Lipsko", "Julia", "Nowak", "7"),
            new Person("M", "Inzynier", "Wyszkow", "Jan", "Nowak", "11"),
            new Person("M", "Inzynier", "Wyszkow", "Jan", "Nowak", "11"),
            new Person("M", "Inzynier", "Wyszkow", "Jan", "Nowak", "13"),
            new Person("M", "Inzynier", "Wyszkow", "Jan", "Nowak", "15"),
            new Person("M", "Inzynier", "Wyszkow", "Jan", "Nowak", "13"),
            new Person("M", "Inzynier", "Wyszkow", "Jan", "Nowak", "15"),

            new Person("K", "Inzynier", "Lipsko", "Julia", "Nowak", "7"),
            new Person("K", "Inzynier", "Lipsko", "Julia", "Nowak", "7"),
            new Person("M", "Inzynier", "Wyszkow", "Jan", "Nowak", "11"),
            new Person("M", "Inzynier", "Wyszkow", "Jan", "Nowak", "11"),
            new Person("M", "Inzynier", "Wyszkow", "Jan", "Nowak", "13"),
            new Person("M", "Inzynier", "Wyszkow", "Jan", "Nowak", "15"),
            new Person("M", "Inzynier", "Wyszkow", "Jan", "Nowak", "13"),
            new Person("M", "Inzynier", "Wyszkow", "Jan", "Nowak", "15"),
        };

        public RecognitionParameterKTests()
        {
        }

        [DataTestMethod]
        public void GivenEmptyPeopleList_ShouldReturn_0()
        {
            var pid = new Expression<Func<Person, object>>[] { p => p.FirstName, p => p.Surname, p => p.Age };
            var recognition = new RecognitionParameterK(new List<Person>());
            var result = recognition.CalculateParameterK(pid);
            Assert.IsTrue(result == 0);
        }

        [DataRow(1, 1)]
        [DataRow(2, 2)]
        [DataRow(3, 1)]
        [DataRow(4, 2)]
        [DataRow(5, 1)]
        [DataRow(6, 1)]
        [DataRow(7, 1)]
        [DataRow(8, 2)]
        [DataRow(16, 4)]
        [DataTestMethod]
        public void GivenGreaterThan1_PersonFromPeopleList_ShouldReturnExpectationValueParameterK(int numberFirstPeople, int expectationResult)
        {
            var pid = new Expression<Func<Person, object>>[] { p => p.FirstName, p => p.Surname, p => p.Age, p => p.Gender, p => p.Job, p => p.City};
            var recognition = new RecognitionParameterK(_people.GetRange(0, numberFirstPeople));
            var result = recognition.CalculateParameterK(pid);
            Assert.IsTrue(result == expectationResult);
        }
    }
}
