using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Anonimizator.Models;

namespace Anonimizator.Algorithms
{
    public interface IKAnonimization
    {
        List<Person> GetAnonymizedData();
    }
}
