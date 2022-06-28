using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JPlag
{
    class Metric
    {
        public string name { get; set; }
        public double threshold { get; set; }
        public List<int> distribution { get; set; }
        public List<TopComparison> topComparisons { get; set; }
    }
}
