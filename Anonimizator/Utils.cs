﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
                yield return string.Join(",",
                    fields.Select(x => (x.GetValue(@object) ?? string.Empty).ToString())
                        .Concat(properties.Select(p => (p.GetValue(@object, null) ?? string.Empty).ToString()))
                        .ToArray());
            }
        }
    }
}
