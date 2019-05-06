using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using ShoppingCardAPI_.Models;

namespace ShoppingCardAPI_.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ValuesController : ControllerBase
    {
        bool _affected = false;
        /// <summary>
        /// bool değişken kontrolüyle OK ya da Not Found dönüş değeri sağlar  
        /// </summary>
        /// <returns>StatusCodeResult</returns>
        public StatusCodeResult AffectedStatus_Checking()
        {
            if (_affected == true)
            {
                return Ok();
            }
            else
                return NotFound();
        }

        /// <summary>
        /// Stok kontrollerıyle bırlıkte product listesi döndürür.
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        public List<Product> ReturnWithStockControls(List<Product> list)
        {
            for (int i = 0; i < list.Count; i++)
            {
                if (list[i].UnitsInStock == 0)
                {
                    list[i].StockNote = "ÜRÜN TÜKENDİ...";
                }
                else if (list[i].UnitsInStock < 5)
                {
                    list[i].StockNote = "SON " + list[i].UnitsInStock + " ÜRÜN!";
                }
            }
            return list;
        }

        /// <summary>
        /// Test kolaylığı adına veri tabani yerine static List kullanımı tercih edilmiştir.
        /// </summary>
        #region Data
        public static List<Product> _shoppingCard = new List<Product>();
        public static List<Product> _productList = new List<Product>
        {
            new Product { ID=0, ProductName="Yenilebilir Çiçek Buketi", Price=149.90m, UnitsInStock=4, StockNote=""},
            new Product { ID=1, ProductName="Çiçek Buketi", Price=129.90m, UnitsInStock=0,StockNote=""},
            new Product { ID=2, ProductName="Kolye", Price=29.90m, UnitsInStock=120,StockNote=""},
            new Product { ID=3, ProductName="Telefon Kılıfı", Price=99.90m, UnitsInStock=85,StockNote=""},
            new Product { ID=4, ProductName="Masa Lambası", Price=159.90m, UnitsInStock=2,StockNote=""},
            new Product { ID=5, ProductName="Hediye Paketi", Price=49.90m, UnitsInStock=55,StockNote=""}
        };
        #endregion

        /// <summary>
        /// Bütün ürünlerin listesini getirir 
        /// </summary>
        /// <returns></returns>
        #region GetAllProducts               /*GET api/values*/

        [HttpGet]
        public List<Product> Get()
        {
            return ReturnWithStockControls(_productList);
        }
        #endregion

        /// <summary>
        /// Belirtilen IDdeki ürünü sepete ekler.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        #region AddToShoppingCard            /*POST api/values/sepeteekle/5 */
        [HttpPost]
        [Route("sepeteekle/{id}")]
        public StatusCodeResult Post(int id)
        {
                Product product = _productList.Where(x => x.ID == id).FirstOrDefault();

                if ((product != null) && (product.UnitsInStock > 0))
                {
                    product.UnitsInStock--;
                    _shoppingCard.Add(product);
                    _affected = true;
                }

            return AffectedStatus_Checking();
        }
        #endregion



        /// <summary>
        /// Sepeti görüntüler.
        /// </summary>
        /// <returns></returns>
        #region SeeShoppingList              /*GET api/values/sepetim*/

        [HttpGet]
        [Route("Sepetim")]
        public List<Product> GetShoppingCardList()
        {

            return ReturnWithStockControls(_shoppingCard);
        }
        #endregion


        /// <summary>
        /// Sepet Totalini belirtir
        /// </summary>
        /// <returns>decimal</returns>
        #region SeeShoppingListTOTAL         /* GET api/values/sepettoplami*/

        [HttpGet]
        [Route("SepetToplami")]
        public decimal GetShoppingCardListTOTAL()
        {
            decimal sum = 0;
            foreach (var item in _shoppingCard)
            {
                sum += item.Price;
            }
            return sum;
        }
        #endregion


        /// <summary>
        /// Belirtilen IDdeki ürünü sepetten çıkarır.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        #region DeleteItemFromShoppingList   /* DELETE api/values/5*/

        [HttpDelete]
        [Route("SepettenCikar/{id}")]
        public StatusCodeResult Delete(int id)
        {
                if (_shoppingCard != null)
                {
                    Product product = _shoppingCard.Where(x => x.ID == id).FirstOrDefault();
                    product.UnitsInStock++;
                    _shoppingCard.Remove(product);
                    _affected = true;
                }

            return AffectedStatus_Checking();
        }
        #endregion
    }
}
