using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JPlag
{
    class SubmissionMatch
    {
        public string first_submission_id { get; set; }
        public string second_submission_id { get; set; }
        public double match_percentage { get; set; }
        public List<FilesOfFirstSubmission> files_of_first_submission { get; set; }
        public List<FilesOfSecondSubmission> files_of_second_submission { get; set; }
        public List<Match> matches { get; set; }
    }
}
