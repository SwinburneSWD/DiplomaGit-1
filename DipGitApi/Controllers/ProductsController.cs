using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using RestSharp;
using Microsoft.Extensions.Configuration;
using DipGitApiLib;
using System.Text.Json;

namespace DipGitApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ProductsController : ControllerBase
    {
        private readonly IConfiguration _config;
        private RestClient _client;
        private string _accessKey;

        public ProductsController(IConfiguration config) {
            _config = config;
            _client = new RestClient(_config.GetConnectionString("RestDB_Url"));
            _accessKey = _config.GetConnectionString("key");
        }
   
        /// <summary>
        /// Searches Products for the value of a specific field
        /// </summary>
        /// <param name="field">Name of field to search on</param>
        /// <param name="value">value of field</param>
        /// <returns>Object if found</returns>
        [HttpGet("{field}/{value}")]
        public async Task<IActionResult> SearchProduct(string field, string value) {
            string search = $"{{\"{field}\":\"{value}\"}}";

            var request = new RestRequest();
            request.AddHeader("cache-control", "no-cache");
            request.AddHeader("x-apikey", _accessKey);
            request.AddHeader("content-type", "application/json");
            request.AddQueryParameter("q", search);
            var response = await _client.GetAsync(request);

            if(response.Content.Contains("_id")) {
                return Ok(response.Content);
            }

            return NotFound();
        }

        /// <summary>
        /// Gets all Products and returns them
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> GetAll() {
            // return all item as a Products object
            var request = new RestRequest(Method.GET);
            request.AddHeader("cache-control", "no-cache");
            request.AddHeader("x-apikey", _accessKey);
            request.AddHeader("content-type", "application/json");
            var response = await _client.Execute(request);

            if(response.Content.Contains("_id")) {
                return Ok(response.Content);
            } 

            return BadRequest();
        }

        /// <summary>
        /// Add a new Product
        /// </summary>
        /// <param name="newProduct"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> Add(Product newProduct) {
            // Add new product from a newProduct parameter
            var body = JsonSerializer.Serialize(newProduct);
            var request = new RestRequest(Method.POST);
            request.AddHeader("cache-control", "no-cache");
            request.AddHeader("x-apikey", "35ef07b4da07e33f8da131df3ef7b29b87d9e");
            request.AddHeader("content-type", "application/json");
            request.AddParameter("application/json", body, ParameterType.RequestBody);
            var response = await _client.Execute(request);

            if(response.Content.Contains("_id")) {
                return Ok(response.Content);
            } 

            return BadRequest();
        }

        /// <summary>
        /// Deletes a product based on id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete]
        public async Task<IActionResult> Delete(string id) {
            // delete a product via its ID
            var request = new RestRequest(Method.DELETE);
            request.AddHeader("cache-control", "no-cache");
            request.AddHeader("x-apikey", "35ef07b4da07e33f8da131df3ef7b29b87d9e");
            request.AddHeader("content-type", "application/json");
            var response = await _client.Execute(request);

            if(response.Content.Contains("_id")) {
                return Ok(response.Content);
            } 

            return BadRequest();
        }

        /// <summary>
        /// Returns the total qty of item from all products
        /// </summary>
        /// <returns></returns>
        [HttpGet("GetTotalQty")]
         [HttpGet("GetTotalQty")]
        public async Task<IActionResult> GetTotalQty()
        {
            // Read all products and create a Products object then find the total qty
            var request = new RestRequest()
            .AddHeader("cache-control", "no-cache")
            .AddHeader("x-apikey", "35ef07b4da07e33f8da131df3ef7b29b87d9e")
            .AddHeader("content-type", "application/json");
            var response = await _client.ExecuteAsync(request);

            if (response.Content.Contains("_id"))
            {
                var products = new Products();
                var responseContent = JsonSerializer.Deserialize<List<Product>>(response.Content);
                products.ProductList = responseContent;
                return Ok("Total Qty of All Products: " + products.GetTotalQtyProducts());
            }

            return NotFound();
        }

        /// <summary>
        /// Returns the total value of all item prices summed.
        /// </summary>
        /// <returns></returns>
        [HttpGet("GetTotalValue")]
        public async Task<IActionResult> GetTotalValue()
        {
            // Read all products and create a Products object then determine total value
            var request = new RestRequest()
            .AddHeader("cache-control", "no-cache")
            .AddHeader("x-apikey", "35ef07b4da07e33f8da131df3ef7b29b87d9e")
            .AddHeader("content-type", "application/json");
            IRestResponse response = await _client.ExecuteAsync(request);

            if (response.Content.Contains("_id"))
            {
                var products = new Products();
                var responseContent = JsonSerializer.Deserialize<List<Product>>(response.Content);
                products.ProductList = responseContent;
                return Ok("Total Value of All Products: " + products.GetTotalValueProducts());
            }

            return NotFound();
        }
    }
}