using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Chat.Data;
using Chat.Models;

namespace Chat.Controllers
{
    public class PanelController : Controller
    {
        // GET: PanelController
        public ActionResult Index()
        {
            var currentUser = HttpContext.User;
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseSqlServer("Server=(localdb)\\mssqllocaldb;Database=aspnet-Chat-61f45cb0-34f4-4a5f-a897-fa595161cb00;Trusted_Connection=True;MultipleActiveResultSets=true")
                .Options;
            var dbContext = new ApplicationDbContext(options);
            var aspNetUserCredit = dbContext.GetAspNetUserCredit(currentUser, dbContext).Result;
            ViewData["TotalUsedTokens"] = aspNetUserCredit.TotalUsedTokens;
            ViewData["CreditGranted"] = aspNetUserCredit.CreditGranted;
            ViewData["Cost"] = (aspNetUserCredit.TotalUsedTokens*0.002)/1000 + "$";
            return View();
        }


        // GET: PanelController/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: PanelController/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: PanelController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: PanelController/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: PanelController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: PanelController/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: PanelController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }
    }
}
