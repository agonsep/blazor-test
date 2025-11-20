using System.Net.Http.Json;
using ardu_store.Models;

namespace ardu_store.Services;

public class ProductService
{
    private readonly HttpClient _httpClient;

    public ProductService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<List<Product>> GetProductsAsync()
    {
        try
        {
            var products = await _httpClient.GetFromJsonAsync<List<Product>>("api/products");
            return products ?? new List<Product>();
        }
        catch (HttpRequestException ex)
        {
            // Log the error (you could inject ILogger here)
            Console.WriteLine($"Error fetching products: {ex.Message}");
            return new List<Product>();
        }
    }

    public async Task<Product?> GetProductByIdAsync(int id)
    {
        try
        {
            var product = await _httpClient.GetFromJsonAsync<Product>($"api/products/{id}");
            return product;
        }
        catch (HttpRequestException ex)
        {
            Console.WriteLine($"Error fetching product {id}: {ex.Message}");
            return null;
        }
    }
}
