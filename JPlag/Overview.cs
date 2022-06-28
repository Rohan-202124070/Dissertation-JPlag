using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JPlag
{
    class Overview
    {
        public List<string> submission_folder_path { get; set; }
        public string base_code_folder_path { get; set; }
        public string language { get; set; }
        public List<string> file_extensions { get; set; }
        public List<string> submission_ids { get; set; }
        public List<object> failed_submission_names { get; set; }
        public List<object> excluded_files { get; set; }
        public int match_sensitivity { get; set; }
        public string date_of_execution { get; set; }
        public int execution_time { get; set; }
        public List<string> comparison_names { get; set; }
        public List<Metric> metrics { get; set; }
        public List<object> clusters { get; set; }
    }
}
