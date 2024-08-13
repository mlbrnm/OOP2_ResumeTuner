using MySqlConnector;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinalProjectResumeTuner
{
	public class DatabaseBroker
	{
		public List<Job> JobList = new List<Job>();
		public List<JobDetails> JobDetailsList = new List<JobDetails>();
		public List<Edu> EduList = new List<Edu>();


		// Initialize the SQL related variables
		private MySqlConnection connection;
		private MySqlConnectionStringBuilder builder;

		// Initialize the connection builder and connection
		public void Initialize()
		{
			builder = new MySqlConnectionStringBuilder
			{
				Server = "localhost",
				UserID = "root",
				Password = "password",
				Database = "resumetuner"
			};

			connection = new MySqlConnection(builder.ConnectionString);
		}

		public void Reset()
        {
			this.JobList.Clear();
			this.JobDetailsList.Clear();
			this.EduList.Clear();
            this.LoadJobExp();
			this.LoadJobDetails();
			this.LoadEduExp();
        }

		// Open the connection to the database, return true if successful, false if not
		public bool OpenConnection()
		{
			try
			{
				connection.Open();
				return true;
			}
			catch (MySqlException ex)
			{
				switch (ex.Number)
				{
					case 0:
						Console.WriteLine("Cannot connect to server. Contact administrator");
						break;
					case 1045:
						Console.WriteLine("Invalid username/password, please try again");
						break;
				}
				return false;
			}
		}
		
		// Close the connection to the database, return true if successful, false if not
		public bool CloseConnection()
		{
			try
			{
				connection.Close();
				return true;
			}
			catch (MySqlException ex)
			{
				Console.WriteLine(ex.Message);
				return false;
			}
		}

		public void AddJob(string jobtitle, string company, DateTime startdate, DateTime enddate, string description)
        {
			int id = GetHighestJobID() + 1;
            string query = $"INSERT INTO jobexp (id, jobtitle, company, startdate, enddate, description) VALUES({id}, '{jobtitle}', '{company}', '{startdate.ToString("yyyy-MM-dd")}', '{enddate.ToString("yyyy-MM-dd")}', '{description}')";

            if (this.OpenConnection() == true)
            {
                MySqlCommand cmd = new MySqlCommand(query, connection);
                cmd.ExecuteNonQuery();
                this.CloseConnection();
            }
        }

		public void AddJobDetails(int jobid, string details)
        {
            string query = $"INSERT INTO jobdetails (job, details) VALUES({jobid}, '{details}')";

            if (this.OpenConnection() == true)
            {
                MySqlCommand cmd = new MySqlCommand(query, connection);
                cmd.ExecuteNonQuery();
                this.CloseConnection();
            }
        }

		public void AddEdu(int id, DateTime startdate, DateTime enddate, string schoolname, string programname)
        {
            string query = $"INSERT INTO eduexp (id, startdate, enddate, schoolname, programname) VALUES({id}, '{startdate.ToString("yyyy-MM-dd")}', '{enddate.ToString("yyyy-MM-dd")}', '{schoolname}', '{programname}')";

            if (this.OpenConnection() == true)
            {
                MySqlCommand cmd = new MySqlCommand(query, connection);
                cmd.ExecuteNonQuery();
                this.CloseConnection();
            }
        }

		public int GetHighestJobID()
        {
            string query = "SELECT MAX(id) FROM jobexp";
            int id = 0;

            if (this.OpenConnection() == true)
            {
                MySqlCommand cmd = new MySqlCommand(query, connection);
                MySqlDataReader dataReader = cmd.ExecuteReader();

                while (dataReader.Read())
                {
                    id = dataReader.GetInt32(0);
                }

                dataReader.Close();
                this.CloseConnection();
            }

            return id;
        }

		public int GetHighestJobDetailID()
        {
            string query = "SELECT MAX(detailid) FROM jobdetails";
            int id = 0;

            if (this.OpenConnection() == true)
            {
                MySqlCommand cmd = new MySqlCommand(query, connection);
                MySqlDataReader dataReader = cmd.ExecuteReader();

                while (dataReader.Read())
                {
                    id = dataReader.GetInt32(0);
                }

                dataReader.Close();
                this.CloseConnection();
            }

            return id;
        }

		// Load all customers from the database
		public void LoadJobExp()
		{
			string query = "SELECT id, jobtitle, company, startdate, enddate, description FROM jobexp";

			if (this.OpenConnection() == true)
			{
				MySqlCommand cmd = new MySqlCommand(query, connection);
				MySqlDataReader dataReader = cmd.ExecuteReader();

				while (dataReader.Read())
				{
					Job job = new Job(
						dataReader.GetInt32("id"),
						dataReader.GetString("jobtitle"),
						dataReader.GetString("company"),
						dataReader.GetDateTime("startdate"),
						dataReader.GetDateTime("enddate"),
						dataReader.GetString("description")
					);

					JobList.Add(job);
				}

				dataReader.Close();
				this.CloseConnection();
			}
		}

        public void LoadJobDetails()
        {
            string query = "SELECT job, details, detailid FROM jobdetails";

            if (this.OpenConnection() == true)
            {
                MySqlCommand cmd = new MySqlCommand(query, connection);
                MySqlDataReader dataReader = cmd.ExecuteReader();

                while (dataReader.Read())
                {
                    int jobId = dataReader.GetInt32("job");
                    string details = dataReader.GetString("details");
					int detailId = dataReader.GetInt32("detailid");

                    JobDetails jobDetails = new JobDetails(details, detailId);

                    // Find the job with the matching id and add the job details
                    Job job = JobList.FirstOrDefault(j => j.Id == jobId);
                    if (job != null)
                    {
                        job.AddJobDetails(jobDetails);
                    }

                    JobDetailsList.Add(jobDetails);
                }

                dataReader.Close();
                this.CloseConnection();
            }
        }

        public void LoadEduExp()
        {
            string query = "SELECT id, startdate, enddate, schoolname, programname FROM eduexp";

            if (this.OpenConnection() == true)
            {
                MySqlCommand cmd = new MySqlCommand(query, connection);
                MySqlDataReader dataReader = cmd.ExecuteReader();

                while (dataReader.Read())
                {
                    Edu edu = new Edu(
                        dataReader.GetInt32("id"),
                        dataReader.GetDateTime("startdate"),
                        dataReader.GetDateTime("enddate"),
                        dataReader.GetString("schoolname"),
						dataReader.GetString("programname")
                    );

                    EduList.Add(edu);
                }

                dataReader.Close();
                this.CloseConnection();
            }
        }

        public void UpdateJob(int id, string jobtitle, string company, DateTime startdate, DateTime enddate, string description)
		{
			string query = $"UPDATE jobexp SET jobtitle = '{jobtitle}', company = '{company}', startdate = '{startdate.ToString("yyyy-MM-dd")}', enddate = '{enddate.ToString("yyyy-MM-dd")}', description = '{description}' WHERE id = {id}";

			if (this.OpenConnection() == true)
			{
				MySqlCommand cmd = new MySqlCommand(query, connection);
				cmd.ExecuteNonQuery();
				this.CloseConnection();
			}
		}

		public void UpdateJobDetails(int jobid, string details, int detailid)
		{
			string query = $"UPDATE jobdetails SET details = '{details}' WHERE detailid = {detailid}";

			if (this.OpenConnection() == true)
			{
				MySqlCommand cmd = new MySqlCommand(query, connection);
				cmd.ExecuteNonQuery();
				this.CloseConnection();
			}

		}

		public void UpdateEdu(int id, DateTime startdate, DateTime enddate, string schoolname, string programname)
		{
			string query = $"UPDATE eduexp SET startdate = '{startdate.ToString("yyyy-MM-dd")}', enddate = '{enddate.ToString("yyyy-MM-dd")}', schoolname = '{schoolname}', programname = '{programname}' WHERE id = {id}";

			if (this.OpenConnection() == true)
			{
				MySqlCommand cmd = new MySqlCommand(query, connection);
				cmd.ExecuteNonQuery();
				this.CloseConnection();
			}
		}

		public void DeleteJob(int id)
		{
			string query = $"DELETE FROM jobexp WHERE id = {id}";

			if (this.OpenConnection() == true)
			{
				MySqlCommand cmd = new MySqlCommand(query, connection);
				cmd.ExecuteNonQuery();
				this.CloseConnection();
			}
		}

		public void DeleteJobDetails(int detailid)
		{
			string query = $"DELETE FROM jobdetails WHERE detailid = {detailid}";

			if (this.OpenConnection() == true)
			{
				MySqlCommand cmd = new MySqlCommand(query, connection);
				cmd.ExecuteNonQuery();
				this.CloseConnection();
			}
		}

    }
}
