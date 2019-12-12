using System;

namespace Anonimizator.Core.Models
{
    public class KPrediction : ICloneable
    {
        public int K { get; set; }

        public string Columns { get; set; }

        public KPrediction(int k, string columns)
        {
            K = k;
            Columns = columns;
        }

        public KPrediction()
        {

        }

        public KPrediction Clone()
        {
            return new KPrediction(this.K, this.Columns);
        }

        object ICloneable.Clone()
        {
            return new KPrediction(this.K, this.Columns);
        }
    }
}
