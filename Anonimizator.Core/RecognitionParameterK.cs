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
    public class RecognitionParameterK
    {
        private readonly List<Person> _people;

        public RecognitionParameterK(IEnumerable<Person> people)
        {
            _people = people.ToList();
        }

        public int CalculateParameterK(IEnumerable<Expression<Func<Person, object>>> selectedProperties)
        {
            if (_people == null || !_people.Any())
                return 0;

            var groups = GetGroupedPeople(selectedProperties.ToArray());
            return groups.Min(g => g.Count);
        }

        private List<PeopleGroup<string>> GetGroupedPeople(Expression<Func<Person, object>>[] expressions)
        {
            var groups = _people.GroupBy(p => p.GetPersonProperties(expressions))
                .Select(gPeople =>
                {
                    var group = new PeopleGroup<string>
                    (
                        people: gPeople.ToList(),
                        value: gPeople.Key
                    );
                    return group;
                })
                .OrderBy(p => p.Value)
                .ToList();

            return groups;
        }
    }
}
