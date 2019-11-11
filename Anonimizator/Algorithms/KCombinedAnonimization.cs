using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Anonimizator.Helpers;
using Anonimizator.Models;
using Anonimizator.Services;

namespace Anonimizator.Algorithms
{
    public class KCombinedAnonimization : IKAnonimization
    {
        private readonly FileService _fileService;
        public int ParameterK { get; }
        public List<Func<Person, object>> _properties { get; set; }
        public List<Expression<Func<Person, object>>> _expressions { get; set; }

        public KCombinedAnonimization(int parameterK, FileService fileService, params Expression<Func<Person, object>>[] pidProperties)
        {
            _fileService = fileService;
            ParameterK = parameterK;
            _expressions = pidProperties.ToList();
            _properties = pidProperties.Select(x => x.Compile()).ToList();
        }

        public List<Person> GetAnonymizedData(IEnumerable<Person> people)
        {
            if (people == null || !people.Any())
                return new List<Person>();

            var groups = GetGroupedPeople(people);
            var algoFactory = new KAnonimizationFactory(100, _fileService, _expressions.ToArray());

            foreach (var algorithms in algoFactory)
            {
                var anonymzedData = algorithms.Aggregate(people.Clone(), (acc, algo) => algo.GetAnonymizedData(acc));
                groups = GetGroupedPeople(anonymzedData);
                if (IsListAnonymized(groups))
                    break;
            }
  
            return groups.SelectMany(x => x.People).ToList();
        }

        private bool IsListAnonymized(IEnumerable<PeopleGroup<string>> groups)
        {
            return groups.All(g => g.Count >= ParameterK);
        }

        private List<PeopleGroup<string>> GetGroupedPeople(IEnumerable<Person> people)
        {
            var groups = people.GroupBy(p => p.GetPersonProperties(_expressions.ToArray()))
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
