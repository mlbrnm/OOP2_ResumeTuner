using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinalProjectResumeTuner
{
    public class Job
    {
        public int Id { get; set; }
        public string JobTitle { get; set; }
        public string Company { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string Description { get; set; }

        public List<JobDetails> DetailList = new List<JobDetails>();

        public Job(int id, string jobTitle, string company, DateTime startDate, DateTime endDate, string description)
        {
            Id = id;
            JobTitle = jobTitle;
            Company = company;
            StartDate = startDate;
            EndDate = endDate;
            Description = description;
        }

        public void AddJobDetails(JobDetails jobDetails)
        {
            DetailList.Add(jobDetails);
            jobDetails.job = this;
        }
    }
}
