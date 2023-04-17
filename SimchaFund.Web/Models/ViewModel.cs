using SimchaFund.Data;
using Action = SimchaFund.Data.Action;

namespace SimchaFund.Web.Models
{
    public class ViewModel
    {

       public List<Simcha> Simchas { get; set; }
       public   List<Contributor> Contributors { get; set; }
       public  List<Contribution> Contributions { get; set; }
       public string Message { get; set; }
       public Simcha Simcha { get; set; }
       public int SimchaId { get; set; }
       public bool DidContribute { get; set; }
        public decimal AmountContributed { get; set; }
        public int TotalContributorAmount { get; set; }
        public decimal TotalAmount { get; set; }
        public List<Action> Actions { get; set; }
        public  decimal Balance { get; set; }
        public string Name { get; set; }


    }
}
