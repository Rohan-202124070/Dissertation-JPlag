using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JPlag
{
    class Match
    {
        public string first_file_name { get; set; }
        public string second_file_name { get; set; }
        public int start_in_first { get; set; }
        public int end_in_first { get; set; }
        public int start_in_second { get; set; }
        public int end_in_second { get; set; }
        public int tokens { get; set; }
    }
}
