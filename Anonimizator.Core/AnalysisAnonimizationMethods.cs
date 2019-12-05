using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Windows.Input;
using Anonimizator.Core.Helpers;
using Anonimizator.Core.Models;
using Anonimizator.Core.Services;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;

namespace Anonimizator.Core
{
    public static class AnalysisAnonimizationMethods
    {
        public static Dictionary<string, int> CalculateNumberIdenticalElements<T>(IEnumerable<Person> people, Func<Person, T> selectedProperty)
        {
            var groups = people.GroupBy(selectedProperty)
                .Select(gPeople => new 
                {
                    Value = gPeople.Select(selectedProperty).First().ToString(),
                    Count = gPeople.Count()
                }).ToDictionary(item => item.Value, item => item.Count);
            return groups;
        }

        public static Dictionary<string, int> CalculateNumberIdenticalLengthElements<T>(IEnumerable<Person> people, Expression<Func<Person, T>> selectedExpression)
        {
            var selectedProperty = selectedExpression.Compile();
            var groups = people.GroupBy(p =>
                {
                    var propertyInfo = Reflections.GetPropertyInfo(p, selectedExpression);
                    return propertyInfo.GetValue(p).ToString().Length;
                })
                .Select(gPeople => new
                {
                    Value = gPeople.Select(selectedProperty).First().ToString().Length + " liter",
                    Count = gPeople.Count()
                }).ToDictionary(item => item.Value, item => item.Count);

            return groups;
        }
    }
}
