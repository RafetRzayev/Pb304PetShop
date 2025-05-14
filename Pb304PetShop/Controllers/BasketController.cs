using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Pb304PetShop.DataContext;
using Pb304PetShop.Models;

namespace Pb304PetShop.Controllers
{
    public class BasketController : Controller
    {
        private readonly AppDbContext _dbContext;
        private const string BasketCookieKey = "Basket";

        public BasketController(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult AddToBasket(int id)
        {
            var product = _dbContext.Products.Find(id);

            if (product == null)
            {
                return BadRequest();
            }

            var basket = GetBasket();

            var exitBasketItem = basket.Find(x => x.ProductId == id);

            if (exitBasketItem == null)
            {
                basket.Add(new BasketItem { ProductId = id, Quantity = 1 });

            }
            else
            {
                exitBasketItem.Quantity++;
            }
            
            var basketJson = JsonConvert.SerializeObject(basket);

            Response.Cookies.Append(BasketCookieKey, basketJson, new CookieOptions
            {
                Expires = DateTimeOffset.Now.AddHours(1)
            });

            return RedirectToAction("Index", "Home");
        }

        private List<BasketItem> GetBasket()
        {
            var basket = Request.Cookies[BasketCookieKey];
           
            if (string.IsNullOrEmpty(basket))
            {
                return new List<BasketItem>();
            }

            var basketItems = JsonConvert.DeserializeObject<List<BasketItem>>(basket);

            if (basketItems == null)
            {
                return new List<BasketItem>();
            }

            return basketItems;
        }
    }
}
