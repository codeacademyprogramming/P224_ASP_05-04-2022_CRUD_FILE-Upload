using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Pustok.DAL;
using Pustok.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Pustok.Areas.Manage.Controllers
{
    [Area("manage")]
    public class ProductController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _env;

        public ProductController(AppDbContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }

        public async Task<IActionResult> Index()
        {
            IEnumerable<Product> products = await _context.Products
                .Where(p=>!p.IsDeleted)
                .OrderByDescending(p=>p.Id)
                .Take(5)
                .ToListAsync();

            return View(products);
        }

        public async Task<IActionResult> Detail(int? id)
        {
            if (id == null) return BadRequest();

            Product product = await _context.Products
                .Include(p => p.Author)
                .Include(p => p.Genre)
                .FirstOrDefaultAsync(p=>p.Id == id && !p.IsDeleted);

            if (product == null) return NotFound();

            return View(product);
        }

        public async Task<IActionResult> Create()
        {
            ViewBag.Authors = await _context.Authors.ToListAsync();
            ViewBag.Genres = await _context.Genres.ToListAsync();

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Product product)
        {
            ViewBag.Authors = await _context.Authors.ToListAsync();
            ViewBag.Genres = await _context.Genres.ToListAsync();

            if (!ModelState.IsValid)
            {
                return View();
            }

            if (product.AuthorId != null && !await _context.Authors.AnyAsync(a => a.Id == product.AuthorId))
            {
                ModelState.AddModelError("AuthorId", "Daxil Edilen Muellif Sehfdir");
                return View();
            }

            if (product.GenreId != null && !await _context.Genres.AnyAsync(a => a.Id == product.GenreId))
            {
                ModelState.AddModelError("GenreId", "Daxil Edilen Janr Sehfdir");
                return View();
            }

            if (product.MainImgFile == null)
            {
                ModelState.AddModelError("MainImgFile", "Main Sekil Mutleq Secilmelidi");
                return View();
            }

            if (product.HoverImgFile == null)
            {
                ModelState.AddModelError("HoverImgFile", "Hover Sekil Mutleq Secilmelidi");
                return View();
            }

            if (product.MainImgFile.ContentType != "image/jpeg")
            {
                ModelState.AddModelError("MainImgFile", "Main Sekil Novu Ancaq jpe ve ya jpeg Mutleq Secilmelidi");
                return View();
            }

            if (product.HoverImgFile.ContentType != "image/jpeg")
            {
                ModelState.AddModelError("HoverImgFile", "Hover Sekil Novu Ancaq jpe ve ya jpeg Mutleq Secilmelidi");
                return View();
            }

            if (((double)product.MainImgFile.Length / 1024) > 50)
            {
                ModelState.AddModelError("MainImgFile", "Main Sekil 20 kb ola biler");
                return View();
            }

            if (((double)product.HoverImgFile.Length / 1024) > 50)
            {
                ModelState.AddModelError("HoverImgFile", "Hover Sekil 20 kb ola biler");
                return View();
            }

            string mainfilenam = Guid.NewGuid().ToString()+"_"+ DateTime.Now.ToString("yyyyMMddHHmmssfff")+"_"+product.MainImgFile.FileName;

            string pathMain = Path.Combine(_env.WebRootPath, "image", "products", mainfilenam);

            using (FileStream file = new FileStream(pathMain, FileMode.Create))
            {
                product.MainImgFile.CopyTo(file);
            }

            string pathHover = @"C:\Users\memme\Desktop\P224\Task\Pustok\Pustok\wwwroot\image\products\" + product.HoverImgFile.FileName;

            using (FileStream file = new FileStream(pathHover, FileMode.Create))
            {
                product.HoverImgFile.CopyTo(file);
            }

            product.MainImage = mainfilenam;
            product.HoverImage = product.HoverImgFile.FileName;

            await _context.Products.AddAsync(product);
            await _context.SaveChangesAsync();

            return RedirectToAction("Index");
            //return RedirectToAction("Index");
            //return RedirectToAction("Index","Home",new { area="manage"});
        }

        public async Task<IActionResult> Update(int? id)
        {
            if (id == null) return BadRequest();

            Product product = await _context.Products.FirstOrDefaultAsync(p => p.Id == id && !p.IsDeleted);

            if (product == null) return NotFound();

            ViewBag.Authors = await _context.Authors.ToListAsync();
            ViewBag.Genres = await _context.Genres.ToListAsync();

            return View(product);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(int? id, Product product)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }

            if (id == null || id != product.Id) return BadRequest();

            Product dbProduct = await _context.Products.FirstOrDefaultAsync(p => p.Id == id && !p.IsDeleted);

            if (dbProduct == null) return NotFound();

            ViewBag.Authors = await _context.Authors.ToListAsync();
            ViewBag.Genres = await _context.Genres.ToListAsync();

            if (product.AuthorId != null && !await _context.Authors.AnyAsync(a => a.Id == product.AuthorId))
            {
                ModelState.AddModelError("AuthorId", "Daxil Edilen Muellif Sehfdir");
                return View(product);
            }

            if (product.GenreId != null && !await _context.Genres.AnyAsync(a => a.Id == product.GenreId))
            {
                ModelState.AddModelError("GenreId", "Daxil Edilen Janr Sehfdir");
                return View(product);
            }

            dbProduct.Title = product.Title;
            dbProduct.Price = product.Price;
            dbProduct.DiscountPrice = product.DiscountPrice;
            dbProduct.MainImage = product.MainImage;
            dbProduct.HoverImage = product.HoverImage;
            dbProduct.AuthorId = product.AuthorId;
            dbProduct.GenreId = product.GenreId;
            dbProduct.IsFeature = product.IsFeature;
            dbProduct.IsArrival = product.IsArrival;
            dbProduct.IsMostView = product.IsMostView;

            await _context.SaveChangesAsync();

            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return BadRequest();

            Product product = await _context.Products
                .Include(p => p.Author)
                .Include(p => p.Genre)
                .FirstOrDefaultAsync(p => p.Id == id && !p.IsDeleted);

            if (product == null) return NotFound();

            return View(product);
        }

        public async Task<IActionResult> DeleteProduct(int? id)
        {
            if (id == null) return BadRequest();

            Product product = await _context.Products
                .FirstOrDefaultAsync(p => p.Id == id && !p.IsDeleted);

            if (product == null) return NotFound();

            //_context.Products.Remove(product);

            product.IsDeleted = true;

            await _context.SaveChangesAsync();

            return RedirectToAction("Index");
        }
    }
}
