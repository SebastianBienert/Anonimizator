using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Anonimizator.Enums
{
    public enum TypeAnonimizationColumn
    {
        [Description("Maskowanie znaków")]
        CharakterMasking = 0,
        [Description("Generalizacja")]
        Generalization = 1
    }
}
