using SimchaFund.Data;

namespace SimchaFund.Web.Models
{
    public class ViewModel
    {

       public List<Simcha> Simchas { get; set; }
       public   List<Contributor> Contributors { get; set; }
       public  List<Contribution> Contributions { get; set; }
      
    }
}
