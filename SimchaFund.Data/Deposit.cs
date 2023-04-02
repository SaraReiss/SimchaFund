using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimchaFund.Data
{
    public  class Deposit
    {
        public int Id { get; set; }
        public int ContrubutorId { get; set; }
        public decimal IntialDeposit { get; set; }
        public DateTime Date { get; set; }
    }
}
