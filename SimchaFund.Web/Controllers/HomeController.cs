using Microsoft.AspNetCore.DataProtection.KeyManagement;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SimchaFund.Data;
using SimchaFund.Web.Models;
using System.Diagnostics;
using System.Transactions;
using Action = SimchaFund.Data.Action;

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
                Simchas = manager.GetSimchas(),
                TotalContributorAmount = manager.GetTotalContributorAmount(),
            };
            if (TempData["newsimcha"] != null)
            {
                vm.Message = (string)TempData["newsimcha"];

            }
            if (TempData["updatecontributions"] != null)
            {
                vm.Message = (string)TempData["updatecontributions"];

            }
           

            return View(vm);


        }
        public IActionResult Contributors(int simchaId)
        {
            var manager = new Manager(_connectionString);
            var vm = new ViewModel
            {

                Contributors = manager.GetContributors()


            };
            vm.TotalAmount = manager.GetTotalAmount(vm.Contributors);
            if (TempData["newcontributor"] != null)
            {
                vm.Message = (string)TempData["newcontributor"];

            }
            if (TempData["deposit"] != null)
            {
                vm.Message = (string)TempData["deposit"];

            }
            else if (TempData["edit"] != null)
            {
                vm.Message = (string)TempData["edit"];
            }
            return View(vm);

        }
        [HttpPost]
        public IActionResult Edit(Contributor c)
        {
            var mgr = new Manager(_connectionString);
            mgr.UpdateContributor(c);
            TempData["edit"] = "Edit successfully completed!";
            return Redirect("/home/contributors");
        }
        public IActionResult History(int id)
        {
            var manager = new Manager(_connectionString);
          
            List<Action> actions = manager.GetDepositsForContributor(id);
            actions.AddRange(manager.GetContributionsForContributor(id));
           
              var vm = new ViewModel
            {

                Actions = actions.OrderBy(d => d.Date).ToList(),
                Balance = manager.GetBalance(id),
                Name = manager.GetContributorName(id),

             };
            return View(vm);
        }
        public IActionResult Contributions(int simchaId)
        {

            var manager = new Manager(_connectionString);
            
            var vm = new ViewModel
            {

                Contributors = manager.GetContributors(),
                Simcha = manager.GetSimcha(simchaId),
                SimchaId = simchaId,
           };
            var index = 0;
            foreach(var c in vm.Contributors)
            {
                  c.Index = index;
                  c.AmountContributed = manager.GetContributionAmountForSimcha( c.Id, simchaId);
                  index++;
            }
            return View(vm);


        }
        [HttpPost]
        public IActionResult NewSimcha(Simcha simcha)

        {
            var manager = new Manager(_connectionString);
            manager.AddSimcha(simcha);
            TempData["newsimcha"] = "New Simcha Created!";
            return Redirect("/home/index");
        }
        [HttpPost]
        public IActionResult NewContributor(Contributor contributor)

        {
            var manager = new Manager(_connectionString);
            manager.AddContributor(contributor);
            TempData["newcontributor"] = "New Contributor Created";
            Deposit d = new Deposit
            {
                ContributorId = contributor.Id,
                Amount = contributor.InitialDeposit,
                Date = contributor.DateCreated
            };

            manager.AddDeposit(d);
             
            return Redirect("/home/contributors");
        }
        [HttpPost]
        public IActionResult UpdateContributions(List<Contribution> contributors, int simchaId)
        {

            var manager = new Manager(_connectionString);
            manager.DeleteContributionList(simchaId);
            TempData["updatecontributions"] = "Simcha Updated Succesfully";
            manager.AddContributions(contributors, simchaId);
            return Redirect("/home/index");
        }
        [HttpPost]
        public IActionResult Deposit(Deposit deposit)
        {
            var manager = new Manager(_connectionString);
            manager.AddDeposit(deposit);
            TempData["deposit"] = "Deposit Successfully recorded";
            return Redirect("/home/contributors");
        }



    }
}