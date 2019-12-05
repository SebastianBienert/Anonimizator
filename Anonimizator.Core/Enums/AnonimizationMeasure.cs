using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Anonimizator.Core.Enums
{
    public enum AnonimizationMeasure
    {
        [Description("Identyczne wartości")]
        NumberIdenticalElements = 0x4,

        [Description("Wartości o równej długości")]
        NumberIdenticalLengthElements = 0x8
    }
}
