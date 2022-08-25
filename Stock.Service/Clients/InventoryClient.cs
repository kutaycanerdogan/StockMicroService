using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Stock.Service.Entities;

namespace Stock.Service.Clients
{
    public class InventoryClient
    {
        private readonly HttpClient httpClient;

        public InventoryClient(HttpClient httpClient)
        {
            this.httpClient = httpClient;
        }

        public async Task<IReadOnlyCollection<Inventory>> GetCatalogItemsAsync()
        {
            var items = await httpClient.GetFromJsonAsync<IReadOnlyCollection<Inventory>>("/items");
            return items;
        }
    }
}