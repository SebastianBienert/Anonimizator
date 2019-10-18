using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Anonimizator.Models;

namespace Anonimizator
{
    public static class Utils
    {
        public static IEnumerable<string> ToCsv<T>(IEnumerable<T> list)
        {
            var fields = typeof(T).GetFields();
            var properties = typeof(T).GetProperties();

            foreach (var @object in list)
            {
                yield return string.Join(";",
                    fields.Select(x => (x.GetValue(@object) ?? string.Empty).ToString())
                        .Concat(properties.Select(p => (p.GetValue(@object, null) ?? string.Empty).ToString()))
                        .ToArray());
            }
        }

        public static Person PersonFromCsv(string line)
        {
            var values = line.Split(';');
            var person = new Person()
            {
                Gender = values[0],
                Job = values[1],
                City = values[2],
                FirstName = values[3],
                Surname = values[4],
                Age = values[5]
            };

            return person;
        }
    }
}
