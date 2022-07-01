using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JPlag
{
    class GroupsTopComaprision
    {
        public Dictionary<string, HashSet<string>> groups_names { set; get; }
        public Dictionary<string, HashSet<TopComparison>> groups_top_comparision { set; get; }
    }
}
