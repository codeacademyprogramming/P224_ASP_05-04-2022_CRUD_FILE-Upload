using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Pustok.DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Pustok.ViewModels.Home;
using Microsoft.AspNetCore.Http;

namespace Pustok.Controllers
{
    public class HomeController : Controller
    {
        private readonly AppDbContext _context;

        public HomeController(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {

            //HttpContext.Session.SetString("P224", "Hello World");

            //HttpContext.Response.Cookies.Append("P224", "Hello World Coockie");

            HomeVm homeVm = new HomeVm
            {
                Sliders = await _context.Sliders.ToListAsync(),
                UpPromotions = await _context.UpPromotions.ToListAsync(),
                Feature = await _context.Products.Include(p => p.Author).Include(p => p.Genre).Where(p => p.IsFeature).OrderByDescending(p => p.Id).Take(8).ToListAsync(),
                Arrival = await _context.Products.Include(p => p.Author).Include(p => p.Genre).Where(p => p.IsArrival).OrderByDescending(p => p.Id).Take(8).ToListAsync(),
                MostView = await _context.Products.Include(p => p.Author).Include(p => p.Genre).Where(p => p.IsMostView).OrderByDescending(p => p.Id).Take(8).ToListAsync()
            };

            return View(homeVm);
    
        }

        //public  IActionResult GetCoockie()
        //{
        //    //var str =  HttpContext.Request.Cookies["P224"];

        //    return Content(HttpContext.Request.Cookies["P224"]);
        //}

        //public async Task<IActionResult> GetSession(string keysession)
        //{
        //    //var sess = HttpContext.Session.GetString(keysession);

        //    return Content(sess);
        //}
    }
}
