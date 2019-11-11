using System.Collections.Generic;
using Anonimizator.Core.Models;

namespace Anonimizator.Core.Algorithms
{
    public interface IKAnonimization
    {
        List<Person> GetAnonymizedData(IEnumerable<Person> people);
    }
}
