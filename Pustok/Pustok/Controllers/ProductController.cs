using Microsoft.AspNetCore.Mvc;
using Pustok.DAL;
using Pustok.Models;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System.Collections.Generic;
using Pustok.ViewModels.Basket;
using System.Linq;

namespace Pustok.Controllers
{
    public class ProductController : Controller
    {
        private readonly AppDbContext _context;

        public ProductController(AppDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> GetDetail(int? id)
        {
            Product product = await _context.Products
                .Include(p=>p.Author)
                .Include(p=>p.Genre)
                .Include(p=>p.ProductImages)
                .FirstOrDefaultAsync(p=>p.Id == id);

            return PartialView("_ProductDetailPartial", product);
        }

        public async Task<IActionResult> AddToBasket(int? id)
        {
            if (id == null) return BadRequest();

            Product product = await _context.Products.FirstOrDefaultAsync(p=>p.Id == id);

            if (product == null) return NotFound();

            BasketVM basketVM = new BasketVM
            {
                Id = product.Id,
                Count = 1
            };

            List<BasketVM> basketVMs = new List<BasketVM>();

            //string session = HttpContext.Session.GetString("basket");

            string coockie = HttpContext.Request.Cookies["basket"];

            if (coockie == null)
            {
                basketVMs.Add(basketVM);
            }
            else
            {
                basketVMs = JsonConvert.DeserializeObject<List<BasketVM>>(coockie);

                if (basketVMs.Any(b=>b.Id == basketVM.Id))
                {
                    basketVMs.FirstOrDefault(b => b.Id == basketVM.Id).Count += 1;
                }
                else
                {
                    basketVMs.Add(basketVM);
                }

                //string prod = JsonConvert.SerializeObject(basketVMs);

                ////HttpContext.Session.SetString("basket", prod);

                //HttpContext.Response.Cookies.Append("basket", prod);
            }

            string prod = JsonConvert.SerializeObject(basketVMs);

            //HttpContext.Session.SetString("basket", prod);

            HttpContext.Response.Cookies.Append("basket", prod);

            foreach (BasketVM item in basketVMs)
            {
                item.Title = _context.Products.FirstOrDefault(p => p.Id == item.Id).Title;
                item.MainImage = _context.Products.FirstOrDefault(p => p.Id == item.Id).MainImage;
                item.Price = _context.Products.FirstOrDefault(p => p.Id == item.Id).Price;
                item.GenreName = _context.Products.Include(p => p.Genre).FirstOrDefault(p => p.Id == item.Id).Genre.Name;
            }

            return PartialView("_BasketPartial", basketVMs);
        }

        public IActionResult GetSession()
        {

            //string session = HttpContext.Session.GetString("basket");

            string coockie = HttpContext.Request.Cookies["basket"];

            List<BasketVM> basketVMs = new List<BasketVM>();

            if (coockie != null)
            {
                basketVMs = JsonConvert.DeserializeObject<List<BasketVM>>(coockie);
            }

            foreach (BasketVM basketVM in basketVMs)
            {
                basketVM.Price = _context.Products.FirstOrDefault(p => p.Id == basketVM.Id).Price;
            }

            return Json(basketVMs);
        }

        public IActionResult ChangeBasketProductCount(int? id, int count)
        {
            if (id == null)
            {
                return BadRequest();
            }

            if (!_context.Products.Any(p=>p.Id == id))
            {
                return NotFound();
            }

            string basket = HttpContext.Request.Cookies["basket"];

            List<BasketVM> basketVMs = null;

            if(basket != null)
            {
                basketVMs = JsonConvert.DeserializeObject<List<BasketVM>>(basket);

                basketVMs.Find(p => p.Id == id).Count = count;

                basket = JsonConvert.SerializeObject(basketVMs);

                HttpContext.Response.Cookies.Append("basket", basket);

                foreach (BasketVM basketVM in basketVMs)
                {
                    basketVM.Title = _context.Products.FirstOrDefault(p => p.Id == basketVM.Id).Title;
                    basketVM.MainImage = _context.Products.FirstOrDefault(p => p.Id == basketVM.Id).MainImage;
                    basketVM.Price = _context.Products.FirstOrDefault(p => p.Id == basketVM.Id).Price;
                    basketVM.GenreName = _context.Products.Include(p => p.Genre).FirstOrDefault(p => p.Id == basketVM.Id).Genre.Name;
                }

                return PartialView("_BasketProductTablePartial", basketVMs);
            }
            else
            {
                return BadRequest();
            }
        }
    }
}
