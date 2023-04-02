using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimchaFund.Data
{
    public class Manager
    {
        private string _connectionString;
        public Manager(string connectionString)
        {
            _connectionString = connectionString;
        }
        public void AddContributor(Contributor contributor)
        {
            using SqlConnection connection = new SqlConnection(_connectionString);
            using SqlCommand cmd = connection.CreateCommand();
            cmd.CommandText = "INSERT into Contributors Values ( @fn, @ln, @cn, @dc,@ai)";
            cmd.Parameters.AddWithValue("@fn", contributor.FirstName);
            cmd.Parameters.AddWithValue("@ln", contributor.LastName);
            cmd.Parameters.AddWithValue("@cn", contributor.CellNumber);
            cmd.Parameters.AddWithValue("@dc", DateTime.Now);
            cmd.Parameters.AddWithValue("@ai", contributor.AlwaysInclude);

            connection.Open();
            cmd.ExecuteNonQuery();

        }
        public void AddSimcha(Simcha simcha)
        {
            using SqlConnection connection = new SqlConnection(_connectionString);
            using SqlCommand cmd = connection.CreateCommand();
            cmd.CommandText = "INSERT into Simchas Values ( @sn, @date)";
            cmd.Parameters.AddWithValue("@sn", simcha.SimchaName);
            cmd.Parameters.AddWithValue("@date", DateTime.Now);


            connection.Open();
            cmd.ExecuteNonQuery();

        }
        public void AddDeposit(Deposit deposit)
        {
            using SqlConnection connection = new SqlConnection(_connectionString);
            using SqlCommand cmd = connection.CreateCommand();
            cmd.CommandText = "INSERT into Deposits Values ( @ci, @ind,@date)";
            cmd.Parameters.AddWithValue("@ci", deposit.ContrubutorId);
            cmd.Parameters.AddWithValue("@ind", deposit.IntialDeposit);
            cmd.Parameters.AddWithValue("@date", DateTime.Now);



            connection.Open();
            cmd.ExecuteNonQuery();

        }
        public void DeleteContributionList(int simchaId)
        {
            using SqlConnection connection = new SqlConnection(_connectionString);
            using SqlCommand cmd = connection.CreateCommand();
            cmd.CommandText = "delete from ManytoMany where simchaid = @sid";
            cmd.Parameters.AddWithValue("@sid", simchaId);

            connection.Open();
            cmd.ExecuteNonQuery();


        }
        public void AddContributions(List<Contribution> contributions)
        {
            using SqlConnection connection = new SqlConnection(_connectionString);
            using SqlCommand cmd = connection.CreateCommand();
            foreach (var contribution in contributions)
            {
                if (contribution.YesContribute)
                {
                    cmd.CommandText = "INSERT into ManytoMany Values ( @si,@ci,@amount)";
                    cmd.Parameters.AddWithValue("@si", contribution.SimchaId);
                    cmd.Parameters.AddWithValue("@ci", contribution.ContributorId);
                    cmd.Parameters.AddWithValue("@date", contribution.Amount);
                }

            }



            connection.Open();
            cmd.ExecuteNonQuery();

        }
        public List<Simcha> GetSimchas()

        {
            using var connection = new SqlConnection(_connectionString);
            using var cmd = connection.CreateCommand();
            cmd.CommandText = "SELECT * FROM simchas order by date desc";
            connection.Open();
            var reader = cmd.ExecuteReader();
            var simchas = new List<Simcha>();
            while (reader.Read())
            {
                var Simcha = new Simcha
                {
                    Id = (int)reader["Id"],
                    SimchaName = (string)reader["SimchaName"],
                    Date = (DateTime)reader["Date"]


                };
                simchas.Add(Simcha);
            }


            return simchas;





        }
        public List<Contributor> GetContributors()

        {
            using var connection = new SqlConnection(_connectionString);
            using var cmd = connection.CreateCommand();
            cmd.CommandText = "SELECT * FROM contributors";
            connection.Open();
            var reader = cmd.ExecuteReader();
            var contributors = new List<Contributor>();
            while (reader.Read())
            {
                var Contributor = new Contributor
                {
                    Id = (int)reader["Id"],
                    FirstName = (string)reader["FirstName"],
                    LastName = (string)reader["LastName"],
                    CellNumber = (string)reader["CellNumber"],
                    DateCreated = (DateTime)reader["DateCreated"],
                    AlwaysInclude = (bool)reader["AlwaysInclude"],



                };
                contributors.Add(Contributor);
            }
            return contributors;
        }
        public List<Contributor> Search(string search)

        {
            using var connection = new SqlConnection(_connectionString);
            using var cmd = connection.CreateCommand();
            cmd.CommandText = "SELECT * FROM contributors WHERE firstname LIKE @search OR lastname LIKE @search";
            cmd.Parameters.AddWithValue("@search", search );
            connection.Open();
            var reader = cmd.ExecuteReader();
            
            var contributors = new List<Contributor>();
            while (reader.Read())
            {
                var Contributor = new Contributor
                {
                    Id = (int)reader["Id"],
                    FirstName = (string)reader["FirstName"],
                    LastName = (string)reader["LastName"],
                    CellNumber = (string)reader["CellNumber"],
                    DateCreated = (DateTime)reader["DateCreated"],
                    AlwaysInclude = (bool)reader["AlwaysInclude"],



                };
                contributors.Add(Contributor);
            }
            return contributors;
        }

    }
}
