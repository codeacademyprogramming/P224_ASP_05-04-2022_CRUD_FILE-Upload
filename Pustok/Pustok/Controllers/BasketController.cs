using Microsoft.AspNetCore.Mvc;
using Pustok.DAL;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using Pustok.ViewModels.Basket;
using Newtonsoft.Json;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace Pustok.Controllers
{
    public class BasketController : Controller
    {
        private readonly AppDbContext _context;
        public BasketController(AppDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            string basket = HttpContext.Request.Cookies["basket"];

            List<BasketVM> basketVMs = null;

            if (basket != null)
            {
                basketVMs = JsonConvert.DeserializeObject<List<BasketVM>>(basket);
            }
            else
            {
                basketVMs = new List<BasketVM>();
            }

            foreach (BasketVM basketVM in basketVMs)
            {
                basketVM.Title = _context.Products.FirstOrDefault(p => p.Id == basketVM.Id).Title;
                basketVM.MainImage = _context.Products.FirstOrDefault(p => p.Id == basketVM.Id).MainImage;
                basketVM.Price = _context.Products.FirstOrDefault(p => p.Id == basketVM.Id).Price;
            }


            return View(basketVMs);
        }

        public IActionResult GetBasket()
        {
            string basket = HttpContext.Request.Cookies["basket"];

            List<BasketVM> basketVMs = null;

            if (basket != null)
            {
                basketVMs = JsonConvert.DeserializeObject<List<BasketVM>>(basket);
            }
            else
            {
                basketVMs = new List<BasketVM>();
            }

            foreach (BasketVM basketVM in basketVMs)
            {
                basketVM.Title = _context.Products.FirstOrDefault(p => p.Id == basketVM.Id).Title;
                basketVM.MainImage = _context.Products.FirstOrDefault(p => p.Id == basketVM.Id).MainImage;
                basketVM.Price = _context.Products.FirstOrDefault(p => p.Id == basketVM.Id).Price;
                basketVM.GenreName = _context.Products.Include(p => p.Genre).FirstOrDefault(p => p.Id == basketVM.Id).Genre.Name;
            }


            return PartialView("_BasketPartial", basketVMs);
        }

        public IActionResult GetTotalSum()
        {
            string basket = HttpContext.Request.Cookies["basket"];

            List<BasketVM> basketVMs = null;

            if (basket != null)
            {
                basketVMs = JsonConvert.DeserializeObject<List<BasketVM>>(basket);
            }
            else
            {
                basketVMs = new List<BasketVM>();
            }

            foreach (BasketVM basketVM in basketVMs)
            {
                basketVM.Title = _context.Products.FirstOrDefault(p => p.Id == basketVM.Id).Title;
                basketVM.MainImage = _context.Products.FirstOrDefault(p => p.Id == basketVM.Id).MainImage;
                basketVM.Price = _context.Products.FirstOrDefault(p => p.Id == basketVM.Id).Price;
                basketVM.GenreName = _context.Products.Include(p => p.Genre).FirstOrDefault(p => p.Id == basketVM.Id).Genre.Name;
            }


            return PartialView("_BasketTotalSumPartial", basketVMs);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return BadRequest();

            if (!await _context.Products.AnyAsync(p => p.Id == id)) return NotFound();

            List<BasketVM> basketVMs = null;

            string coockie = HttpContext.Request.Cookies["basket"];

            if(coockie != null)
            {
                basketVMs = JsonConvert.DeserializeObject<List<BasketVM>>(coockie);

                if (!basketVMs.Any(b => b.Id == id)) return NotFound();

                BasketVM basketVM = basketVMs.FirstOrDefault(b => b.Id == id);

                basketVMs.Remove(basketVM);
            }
            else
            {
                basketVMs = new List<BasketVM>();
            }

            coockie = JsonConvert.SerializeObject(basketVMs);

            HttpContext.Response.Cookies.Append("basket",coockie);

            foreach (BasketVM basketVM in basketVMs)
            {
                basketVM.Title = _context.Products.FirstOrDefault(p => p.Id == basketVM.Id).Title;
                basketVM.MainImage = _context.Products.FirstOrDefault(p => p.Id == basketVM.Id).MainImage;
                basketVM.Price = _context.Products.FirstOrDefault(p => p.Id == basketVM.Id).Price;
                basketVM.GenreName = _context.Products.Include(p => p.Genre).FirstOrDefault(p => p.Id == basketVM.Id).Genre.Name;
            }


            return PartialView("_BasketProductTablePartial", basketVMs);
        }
    }
}
