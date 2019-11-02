using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Anonimizator.Helpers;
using Anonimizator.Models;

namespace Anonimizator.Algorithms
{
    public class CommonStartStringMasking<T> : IKAnonimization
    {
        public int ParameterK { get; }
        private readonly Expression<Func<Person, T>> _anonimizedExpression;
        private readonly Func<Person, T> _anonimizedProperty;


        public CommonStartStringMasking(int parameterK, Expression<Func<Person, T>> anonimizedProperty)
        {
            ParameterK = parameterK;
            _anonimizedExpression = anonimizedProperty;
            _anonimizedProperty = anonimizedProperty.Compile();
        }

        public List<Person> GetAnonymizedData(IEnumerable<Person> people)
        {
            if (people == null || !people.Any())
                return new List<Person>();

            var groups = people.GroupBy(p =>
                {
                    var propertyInfo = Reflections.GetPropertyInfo(p, _anonimizedExpression);
                    return propertyInfo.GetValue(p).ToString().Substring(0, 1);
                })
                .SelectMany(gPeople =>
                {
                    var group = new PeopleGroup<string>
                    (
                        people: gPeople.ToList(),
                        value: gPeople.Count() >= ParameterK
                            ? gPeople.Select(_anonimizedProperty).First().ToString().Substring(0, 1)
                            : "*"
                    );
                    return GetGroupedPeople(new List<PeopleGroup<string>> {group}, 2);
                })
                .OrderBy(p => p.Value)
                .ToList();

            //There may be a case where number of people without common group (makred as *) is lower than ParamaterK
            if (IsUncommonGroupToSmall(groups))
            {
                groups = GetMergedGroups(groups).ToList();
            }

            var anonymyzedPeople = GetPeopleWithAnnonymazedAgeRange(groups.ToList());
            return anonymyzedPeople;
        }

        private IEnumerable<PeopleGroup<string>> GetGroupedPeople(IEnumerable<PeopleGroup<string>> peopleGroup, int commonLength)
        {
            var groupsOrderdByFirstCharacter = peopleGroup.SelectMany(g => g.People.GroupBy(p =>
                {
                    var propertyInfo = Reflections.GetPropertyInfo(p, _anonimizedExpression);
                    return propertyInfo.GetValue(p).ToString().Substring(0, commonLength);
                })
                .Select(gPeople => new PeopleGroup<string>(
                    people: gPeople.ToList(),
                    value: gPeople.Select(_anonimizedProperty).First().ToString().Substring(0, commonLength))
                ));

            if (AreGroupAnonymzedMaximally(groupsOrderdByFirstCharacter, commonLength + 1))
                return peopleGroup.Select(g => g);

            return GetGroupedPeople(groupsOrderdByFirstCharacter, commonLength + 1);
        }

        private bool AreGroupAnonymzedMaximally(IEnumerable<PeopleGroup<string>> groups, int commonLength)
        {
            var result = groups.Any(g => g.Count < ParameterK ||
                                         g.People.Any(p =>
                                         {
                                             var propertyInfo = Reflections.GetPropertyInfo(p, _anonimizedExpression);
                                             return propertyInfo.GetValue(p).ToString().Length < commonLength;
                                         })
                                   );
            return result;
        }

        private bool IsUncommonGroupToSmall(IEnumerable<PeopleGroup<string>> peopleGroup)
        {
            var uncommonGroupsCount = peopleGroup.Where(g => g.Value == "*").Sum(g => g.Count);
            return uncommonGroupsCount > 0 && uncommonGroupsCount < ParameterK;
        }

        private IEnumerable<PeopleGroup<string>> GetMergedGroups(IEnumerable<PeopleGroup<string>> peopleGroup)
        {
            var uncommonGroupsCount = peopleGroup.Where(g => g.Value == "*").Sum(g => g.Count);
            var groupsToMerge = peopleGroup
                .Where(g => g.Value != "*")
                .TakeWhileAggregate(uncommonGroupsCount,
                    (count, group) => count + group.Count,
                    count => count <= ParameterK);

            var mergedGroups = peopleGroup.Select(g =>
                {
                    if (groupsToMerge.Select(x => x.Value).Contains(g.Value))
                        return new PeopleGroup<string>(g.People.ToList(), "*");
                    return g;
                })
                .ToList();

            return mergedGroups;
        }

        private List<Person> GetPeopleWithAnnonymazedAgeRange(List<PeopleGroup<string>> groups)
        {
            var propertyInfo = Reflections.GetPropertyInfo(new Person(), _anonimizedExpression);
            var anonymzedPeople = groups.SelectMany(g =>
            {
                var anonymzed = g.People.Select(p =>
                {
                    var clone = p.Clone();
                    var anonymyzedValue = g.Value == "*" ? "*" : $"{g.Value}...";
                    propertyInfo.SetValue(clone, Convert.ChangeType(anonymyzedValue, propertyInfo.PropertyType), null);
                    return clone;
                });
                return anonymzed;
            })
            .ToList();
            return anonymzedPeople;
        }
    }
}
