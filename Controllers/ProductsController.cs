using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using ardu_store.Models;

namespace ardu_store.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProductsController : ControllerBase
{
    private static readonly List<Product> _products;

    static ProductsController()
    {
        // Hardcoded JSON data
        var jsonData = @"[
            {
                ""Id"": 1,
                ""Name"": ""Arduino Uno R3"",
                ""Description"": ""Microcontroller board based on the ATmega328P"",
                ""Price"": 27.50,
                ""Category"": ""Boards"",
                ""Stock"": 45
            },
            {
                ""Id"": 2,
                ""Name"": ""Raspberry Pi 4 Model B"",
                ""Description"": ""8GB RAM single-board computer"",
                ""Price"": 75.00,
                ""Category"": ""Boards"",
                ""Stock"": 32
            },
            {
                ""Id"": 3,
                ""Name"": ""Jumper Wires Set"",
                ""Description"": ""120pcs multicolored breadboard jumper wires"",
                ""Price"": 8.99,
                ""Category"": ""Accessories"",
                ""Stock"": 150
            },
            {
                ""Id"": 4,
                ""Name"": ""HC-SR04 Ultrasonic Sensor"",
                ""Description"": ""Distance measuring sensor module"",
                ""Price"": 4.50,
                ""Category"": ""Sensors"",
                ""Stock"": 89
            },
            {
                ""Id"": 5,
                ""Name"": ""Servo Motor SG90"",
                ""Description"": ""Micro servo motor 9g"",
                ""Price"": 3.25,
                ""Category"": ""Motors"",
                ""Stock"": 67
            },
            {
                ""Id"": 6,
                ""Name"": ""LED Assortment Kit"",
                ""Description"": ""500pcs 3mm and 5mm LEDs in various colors"",
                ""Price"": 12.99,
                ""Category"": ""Components"",
                ""Stock"": 28
            },
            {
                ""Id"": 7,
                ""Name"": ""ESP32 Development Board"",
                ""Description"": ""WiFi and Bluetooth microcontroller"",
                ""Price"": 15.99,
                ""Category"": ""Boards"",
                ""Stock"": 54
            },
            {
                ""Id"": 8,
                ""Name"": ""DHT22 Temperature Sensor"",
                ""Description"": ""Digital temperature and humidity sensor"",
                ""Price"": 9.75,
                ""Category"": ""Sensors"",
                ""Stock"": 73
            }
        ]";

        _products = JsonSerializer.Deserialize<List<Product>>(jsonData) ?? new List<Product>();
    }

    [HttpGet]
    public ActionResult<List<Product>> GetProducts()
    {
        return Ok(_products);
    }

    [HttpGet("{id}")]
    public ActionResult<Product> GetProduct(int id)
    {
        var product = _products.FirstOrDefault(p => p.Id == id);
        
        if (product == null)
        {
            return NotFound();
        }

        return Ok(product);
    }

    [HttpPost]
    public ActionResult<Product> CreateProduct(Product product)
    {
        product.Id = _products.Max(p => p.Id) + 1;
        _products.Add(product);
        return CreatedAtAction(nameof(GetProduct), new { id = product.Id }, product);
    }

    [HttpPut("{id}")]
    public ActionResult UpdateProduct(int id, Product product)
    {
        var existingProduct = _products.FirstOrDefault(p => p.Id == id);
        
        if (existingProduct == null)
        {
            return NotFound();
        }

        existingProduct.Name = product.Name;
        existingProduct.Description = product.Description;
        existingProduct.Price = product.Price;
        existingProduct.Category = product.Category;
        existingProduct.Stock = product.Stock;

        return NoContent();
    }

    [HttpDelete("{id}")]
    public ActionResult DeleteProduct(int id)
    {
        var product = _products.FirstOrDefault(p => p.Id == id);
        
        if (product == null)
        {
            return NotFound();
        }

        _products.Remove(product);
        return NoContent();
    }
}
