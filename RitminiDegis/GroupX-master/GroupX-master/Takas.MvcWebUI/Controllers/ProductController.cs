﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Takas.Business.Abstract;
using Takas.Common;
using Takas.Common.Entities.Concrete;
using Takas.Common.Response;
using Takas.Common.SystemConstants;
using Takas.Common.SystemTools;
using Takas.MvcWebUI.Models;

namespace Takas.MvcWebUI.Controllers
{
    public class ProductController : Controller
    {
        IProductService _productService;
        IProductImageGalleryService _productImageGalleryService;

        public ProductController(IProductService productService, IProductImageGalleryService productImageGalleryService)
        {
            _productService = productService;
            _productImageGalleryService = productImageGalleryService;
        }

        public ActionResult Index()
        {
            HttpResponseMessage result;
            result = WebApiRequestOperation.WebApiRequestOperationMethodForUser(SystemConstannts.WebApiDomainAddress, "api/Product/ProductList",
                new { user = ((User)HttpContext.Session["User"]) });

            return View();
        }

        // Ürün detay sayfasın ıcın method
        public async Task<ActionResult> Detail(int id)
        {
            // Burada Model kullanmak istedik
            ProductDetailViewModel productModel = new ProductDetailViewModel();
            productModel.Product = _productService.Get(id);
            if (productModel.Product != null)
            {
                productModel.productImageGalleries = await _productImageGalleryService.GetImageGallery(productModel.Product.ID);
            }
            else
            {
                return View("/Views/Shared/NotFound.cshtml");
            }
            ViewBag.title = "Ürün Detay | Ürün1";
            return View(productModel);
        }

        public ActionResult AddProduct()
        {
            ViewBag.PageInfo = "ÜRÜN EKLE";
            ProductAddModel addModel = new ProductAddModel();
            HttpResponseMessage result;
            result = WebApiRequestOperation.WebApiRequestOperationMethodForUser(SystemConstannts.WebApiDomainAddress, "api/Category/GetCategories", null);

            if (result.StatusCode == HttpStatusCode.OK)
            {
                string resultString = result.Content.ReadAsStringAsync().Result;
                if (resultString != "{\"Categories\":null}")
                {
                    GetCategoryListResponse getCategory = Newtonsoft.Json.JsonConvert.DeserializeObject<GetCategoryListResponse>(resultString);
                    if (getCategory.Code == 1)
                    {
                        addModel.Categories = getCategory.Categories;
                    }
                    else
                        addModel.Categories = null;
                }
            }

            result = WebApiRequestOperation.WebApiRequestOperationMethodForUser(SystemConstannts.WebApiDomainAddress, "api/Brand/GetBrands", null);
            if (result.StatusCode == HttpStatusCode.OK)
            {
                string resultString = result.Content.ReadAsStringAsync().Result;
                if (resultString != "{\"Brands\":null}")
                {
                    GetBrandListResponse getBrand = Newtonsoft.Json.JsonConvert.DeserializeObject<GetBrandListResponse>(resultString);
                    if (getBrand.Code == 1)
                    {
                        addModel.Brands = getBrand.Brands;
                    }
                    else
                        addModel.Brands = null;
                }
            }
            addModel.Product = null;
            return View(addModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ProductAddComplete(Product product, HttpPostedFileBase PImage)
        {
            if (ModelState.IsValid)
            {
                product.Situation = SystemConstannts.Situation.BEKLEMEDE;
                product.Date = DateTime.Now;
                product.User_ID = (HttpContext.Session["User"] as User).ID;
                product.Image = PImage.FileName;
                HttpResponseMessage result;

                using (HttpClient client = new HttpClient())
                {

                    using (var content = new MultipartFormDataContent())
                    {
                        byte[] Bytes = new byte[PImage.InputStream.Length + 1];
                        PImage.InputStream.Read(Bytes, 0, Bytes.Length);
                        var fileContent = new ByteArrayContent(Bytes);
                        fileContent.Headers.ContentDisposition = new System.Net.Http.Headers.ContentDispositionHeaderValue("attachment") { FileName = PImage.FileName };
                        content.Add(fileContent);

                        client.DefaultRequestHeaders.Add(SystemConstannts.apiKey, SystemConstannts.apiValue);
                        var requestUri = "http://localhost:2765/api/Product/AddProductImageToFolder";
                        result = client.PostAsync(requestUri, content).Result;
                    }
                }
                if (result.StatusCode == HttpStatusCode.OK)
                {
                    using (HttpClient client = new HttpClient())
                    {
                        client.BaseAddress = new Uri("http://localhost:2765/");
                        client.DefaultRequestHeaders.Add(SystemConstannts.apiKey, SystemConstannts.apiValue);
                        result = client.PostAsJsonAsync("api/Product/AddProduct", product).Result;
                    }
                    if (result.StatusCode == HttpStatusCode.OK)
                    {
                        string resultString = result.Content.ReadAsStringAsync().Result;
                        if (resultString != "{\"Product\":null}")
                        {
                            ProductAddResponse addResponse = Newtonsoft.Json.JsonConvert.DeserializeObject<ProductAddResponse>(resultString);
                            if (addResponse.Code == 1)
                            {
                                return RedirectToAction("MyProducts", "UserProfil");
                            }
                        }
                    }
                }
            }
            return RedirectToAction("AddProduct");
        }


        [HttpPost]
        public ActionResult ProducFilter()
        {

            return null;
        }



    }
}