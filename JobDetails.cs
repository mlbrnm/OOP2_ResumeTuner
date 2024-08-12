using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinalProjectResumeTuner
{
    public class JobDetails
    {
        public string Details { get; set; }
        public int DetailID { get; set; }
        public Job job { get; set; }

        public JobDetails(string details, int detailid)
        {
            Details = details;
            DetailID = detailid;
        }
    }
}
