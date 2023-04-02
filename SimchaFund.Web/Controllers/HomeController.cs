using Microsoft.AspNetCore.DataProtection.KeyManagement;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SimchaFund.Data;
using SimchaFund.Web.Models;
using System.Diagnostics;

namespace SimchaFund.Web.Controllers
{
    
    public class HomeController : Controller
    {
        private string _connectionString =
          @"Data Source=.\sqlexpress;Initial Catalog=SimchaFund;Integrated Security=True";

        public IActionResult Index()
        {
            
            var manager = new Manager(_connectionString);
            var vm = new ViewModel
            {
                Simchas = manager.GetSimchas()
              
               
            };
            return View(vm);

           
        }
        public IActionResult Contributors()
        {
            var manager = new Manager(_connectionString);
            var vm = new ViewModel
            {
                 Contributors = manager.GetContributors()


            };
            return View(vm);
           
        }
        public IActionResult History()
        {
            return View();
        }
        public IActionResult Contributions()
        {
            return View();
        }
        public IActionResult NewSimcha(Simcha simcha)

        {
            var manager = new Manager(_connectionString);
            manager.AddSimcha(simcha);
            return Redirect("/home/index"); ;
        }
        public IActionResult NewContributor(Contributor contributor )

        {
            var manager = new Manager(_connectionString);
            manager.AddContributor(contributor);
            return Redirect("/home/contributors"); ;
        }



    }
}