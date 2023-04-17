using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection.PortableExecutable;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
using System.Xml.Linq;

namespace SimchaFund.Data
{
    public class Manager
    {
        private string _connectionString;
        public Manager(string connectionString)
        {
            _connectionString = connectionString;
        }
        public int AddContributor(Contributor contributor)
        {
            using SqlConnection connection = new SqlConnection(_connectionString);
            using SqlCommand cmd = connection.CreateCommand();
            cmd.CommandText = "INSERT into Contributors Values ( @fn, @ln, @cn, @dc,@ai) select Scope_identity()";
            cmd.Parameters.AddWithValue("@fn", contributor.FirstName);
            cmd.Parameters.AddWithValue("@ln", contributor.LastName);
            cmd.Parameters.AddWithValue("@cn", contributor.CellNumber);
            cmd.Parameters.AddWithValue("@dc", DateTime.Now);
            cmd.Parameters.AddWithValue("@ai", contributor.AlwaysInclude);



            connection.Open();

            contributor.Id = (int)(decimal)cmd.ExecuteScalar();
            return contributor.Id;





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
            cmd.CommandText = "INSERT into Deposits Values ( @ci, @amount ,@date)";
            cmd.Parameters.AddWithValue("@ci", deposit.ContributorId);
            cmd.Parameters.AddWithValue("@amount", deposit.Amount);
            cmd.Parameters.AddWithValue("@date", DateTime.Now);



            connection.Open();
            cmd.ExecuteNonQuery();

        }
        public void AddContributions(List<Contribution> contributions, int simchaId)
        {
            foreach (var c in contributions.Where(c => c.DidContribute))
            {
                AddContribution(c, simchaId);
            }
        }
        private void AddContribution(Contribution contribution, int simchaId)
        {
            using SqlConnection connection = new SqlConnection(_connectionString);
            using SqlCommand cmd = connection.CreateCommand();

            cmd.CommandText = "INSERT into SimchasContributors Values ( @si,@ci,@amount)";
            cmd.Parameters.AddWithValue("@si", simchaId);
            cmd.Parameters.AddWithValue("@ci", contribution.ContributorId);
            cmd.Parameters.AddWithValue("@amount", contribution.Amount);

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
                    Date = (DateTime)reader["Date"],
                    ContributerCount = GetContributorCount((int)reader["Id"]),
                    Total = TotalForSimcha((int)reader["Id"]),



                };
                simchas.Add(Simcha);
            }


            return simchas;





        }
        private decimal TotalForSimcha(int simchaId)
        {
            using SqlConnection connection = new SqlConnection(_connectionString);
            using SqlCommand command = connection.CreateCommand();
            command.CommandText = "SELECT SUM (Amount) AS 'total' FROM SimchasContributors WHERE SimchaId = @simchaId";
            command.Parameters.AddWithValue("@simchaId", simchaId);

            connection.Open();
            var reader = command.ExecuteReader();

            decimal amount = 0;


            while (reader.Read())
            {
                amount = reader.GetOrNull<decimal>("total");



            }
            return amount;
        }
        public Simcha GetSimcha(int id)
        {
            using var connection = new SqlConnection(_connectionString);
            using var cmd = connection.CreateCommand();
            cmd.CommandText = "SELECT * FROM simchas where id = @id";
            cmd.Parameters.AddWithValue("@id", id);
            connection.Open();
            var reader = cmd.ExecuteReader();
            var s = new Simcha();


            while (reader.Read())
            {
                s = new Simcha
                {
                    Id = (int)reader["Id"],
                    SimchaName = (string)reader["SimchaName"],
                    Date = (DateTime)reader["Date"],
                    //contributors = GetContributorsfromsimcha((int)reader["Id"]),
                    ContributerCount = GetContributorCount((int)reader["Id"]),
                    Total = TotalForSimcha((int)reader["Id"]),



                };

            }


            return s;
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
                    Balance = GetBalance((int)reader["Id"]),
                    DateCreated = (DateTime)reader["DateCreated"],
                    AlwaysInclude = (bool)reader["AlwaysInclude"],



                };
                contributors.Add(Contributor);
            }
            return contributors;
        }
        public void DeleteContributionList(int simchaId)
        {
            using SqlConnection connection = new SqlConnection(_connectionString);
            using SqlCommand cmd = connection.CreateCommand();
            cmd.CommandText = "delete from SimchasContributors where simchaid = @sid";
            cmd.Parameters.AddWithValue("@sid", simchaId);

            connection.Open();
            cmd.ExecuteNonQuery();


        }
        public void UpdateContributor(Contributor c)
        {
            using var connection = new SqlConnection(_connectionString);
            using var cmd = connection.CreateCommand();
            cmd.CommandText = @"UPDATE Contributors SET FirstName = @firstName, LastName = @lastName,
                             CellNumber = @cellNumber, AlwaysInclude = @alwaysInclude,
                             DateCreated = @dateCreated WHERE Id = @id";
            cmd.Parameters.AddWithValue("@firstName", c.FirstName);
            cmd.Parameters.AddWithValue("@lastName", c.LastName);
            cmd.Parameters.AddWithValue("@cellNumber", c.CellNumber);
            cmd.Parameters.AddWithValue("@alwaysInclude", c.AlwaysInclude);
            cmd.Parameters.AddWithValue("@dateCreated", c.DateCreated);
            cmd.Parameters.AddWithValue("@id", c.Id);
            connection.Open();
            cmd.ExecuteNonQuery();
            connection.Close();
        }
        //public List<Contributor> GetContributorsFromSimcha(int simchaId)
        //{
        //    using var connection = new SqlConnection(_connectionString);
        //    using var cmd = connection.CreateCommand();
        //    cmd.CommandText = @"SELECT * from Contributors c 
        //    LEFT JOIN SimchasContributors sc 
        //   ON c.Id = sc.ContributorId 
        //    where sc.SimchaId = @simchaid or sc.SimchaId is null";
        //    cmd.Parameters.AddWithValue("@simchaid", simchaId);
        //    connection.Open();
        //    var reader = cmd.ExecuteReader();
        //    var contributors = new List<Contributor>();
        //    while (reader.Read())
        //    {
        //        var contributor = new Contributor
        //        {

        //            Id = (int)reader["Id"],
        //            FirstName = (string)reader["FirstName"],
        //            LastName = (string)reader["LastName"],
        //            CellNumber = (string)reader["CellNumber"],
        //            Balance = GetBalance((int)reader["Id"]),
        //            DateCreated = (DateTime)reader["DateCreated"],
        //            AlwaysInclude = (bool)reader["AlwaysInclude"],
        //            AmountContributed = reader.GetOrNull<decimal>("amount"),


        //        };
        //        contributors.Add(contributor);
        //    }


        //    return contributors;


        //}
        public decimal GetBalance(int id)
        {

            return GetDepositsAmount(id) - GetContributionAmount(id);
        }
        public decimal GetContributionAmountForSimcha( int contributorId, int simchaId)
        {
            using var connection = new SqlConnection(_connectionString);
            using var cmd = connection.CreateCommand();
            cmd.CommandText = "SELECT amount from SimchasContributors where ContributorId =  @ci and SimchaId = @si ";
            cmd.Parameters.AddWithValue("@ci", contributorId);
            cmd.Parameters.AddWithValue("@si", simchaId);
            connection.Open();
            var reader = cmd.ExecuteReader();
            decimal amount = 0;
            if (reader.Read())
            {

                amount = reader.GetOrNull<decimal>("Amount"); ;

            }



            return amount;


        }
        private decimal GetContributionAmount(int id)
        {
            using var connection = new SqlConnection(_connectionString);
            using var cmd = connection.CreateCommand();
            cmd.CommandText = "SELECT sum(Amount) as 'total' from SimchasContributors where ContributorId =  @ci ";
            cmd.Parameters.AddWithValue("@ci", id);

            connection.Open();
            var reader = cmd.ExecuteReader();

            decimal amount = 0;


            while (reader.Read())
            {
                amount = reader.GetOrNull<decimal>("total");



            }
            return amount;
        }
        private decimal GetDepositsAmount(int id)
        {
            using var connection = new SqlConnection(_connectionString);
            using var cmd = connection.CreateCommand();
            cmd.CommandText = "SELECT sum(InitialDeposit) as 'total' from deposits where ContributorId =  @ci ";
            cmd.Parameters.AddWithValue("@ci", id);

            connection.Open();
            var reader = cmd.ExecuteReader();

            decimal amount = 0;


            while (reader.Read())
            {
                amount = reader.GetOrNull<decimal>("total");



            }
            return amount;
        }
        public List<Contributor> Search(string search)

        {
            using var connection = new SqlConnection(_connectionString);
            using var cmd = connection.CreateCommand();
            cmd.CommandText = "SELECT * FROM contributors WHERE firstname LIKE @search OR lastname LIKE @search";
            cmd.Parameters.AddWithValue("@search", search);
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
        public int GetTotalContributorAmount()
        {

            using var connection = new SqlConnection(_connectionString);
            using var cmd = connection.CreateCommand();
            cmd.CommandText = "SELECT COUNT(*) FROM Contributors";
            connection.Open();
            return (int)cmd.ExecuteScalar();
        }
        private int GetContributorCount(int id)
        {
            using SqlConnection connection = new(_connectionString);
            using SqlCommand command = connection.CreateCommand();
            command.CommandText = "select count(*) from SimchasContributors  WHERE SimchaId = @sid";
            command.Parameters.AddWithValue("@sid", id);
            connection.Open();
            return (int)command.ExecuteScalar();
        }
        public decimal GetTotalAmount(List<Contributor> contributors)
        {
            decimal total = 0;
            foreach (var c in contributors)
            {
                total += GetBalance(c.Id);
            }
            return total;
        }
        public string GetContributorName(int id)
        {
            using var connection = new SqlConnection(_connectionString);
            using var command = connection.CreateCommand();
            command.CommandText = @"SELECT FirstName, LastName From Contributors where Id = @id";
            command.Parameters.AddWithValue("@id", id);
            connection.Open();
            SqlDataReader reader = command.ExecuteReader();
            string name = "";
            if(!reader.Read())
            {
                return null;
            }
            else 
            {
                name  += (string)reader["FirstName"];
                name  += " ";
                name  += (string)reader["LastName"];
            };

            return name;
        }
        public List<Action> GetDepositsForContributor(int id)
        {
            using var connection = new SqlConnection(_connectionString);
            using var command = connection.CreateCommand();
            command.CommandText = "SELECT * From Deposits WHERE ContributorId = @id";
            command.Parameters.AddWithValue("@id",id);
            List<Action> deposits = new List<Action>();
            connection.Open();
            SqlDataReader reader = command.ExecuteReader();
            while (reader.Read())
            {
                deposits.Add(new Action
                {
                    Amount = (decimal)reader["InitialDeposit"],
                    Date = (DateTime)reader["Date"],
                    Act = "Deposit",
                });
            }

            return deposits;
        }
        public List<Action> GetContributionsForContributor(int id)
        {
            using var connection = new SqlConnection(_connectionString);
            using var command = connection.CreateCommand();
            command.CommandText = @"SELECT con.Amount, s.SimchaName, s.Date From SimchasContributors con
                                   JOIN Simchas s ON s.Id = con.SimchaId 
                                    WHERE con.ContributorId = @id";
            command.Parameters.AddWithValue("@id", id);
            List<Action> contributions = new List<Action>();
            connection.Open();
            SqlDataReader reader = command.ExecuteReader();
            while (reader.Read())
            {
                contributions.Add(new Action
                {
                    Amount = (decimal)reader["Amount"],
                    Date = (DateTime)reader["Date"],
                    Act = "Contribution for the " + (string)reader["SimchaName"] + " Simcha",
                });
            }

            return contributions;
        }



    }
}
