using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinalProjectResumeTuner
{
    public class Edu
    {
        public int Id { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string School { get; set; }
        public string Program { get; set; }

        public Edu(int id, DateTime startDate, DateTime endDate, string school, string program)
        {
            Id = id;
            StartDate = startDate;
            EndDate = endDate;
            School = school;
            Program = program;
        }
    }
}
