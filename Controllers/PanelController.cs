using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Chat.Data;
using Chat.Models;
using System.Data.Entity;

namespace Chat.Controllers
{
    public class PanelController : Controller
    {
        private ApplicationDbContext _dbContext;
        // GET: PanelController
        public PanelController(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public ActionResult Index()
        {
            var currentUser = HttpContext.User;

            var aspNetUserCredit = _dbContext.GetAspNetUserCredit(currentUser, _dbContext).Result;
            ViewData["TotalUsedTokens"] = aspNetUserCredit.TotalUsedTokens;
            ViewData["CreditGranted"] = "infinite";  //aspNetUserCredit.CreditGranted
            ViewData["Cost"] = (aspNetUserCredit.TotalUsedTokens/1000)*0.002 + "$";
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
