using D.W.C.Lib.D.W.C.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace D.W.C.Lib
{
    public class AzureDevOpsClient
    {
        private readonly HttpClient _httpClient;
        private readonly string _baseUrl;

        // Konstruktor i inne podstawowe metody pozostają bez zmian

        public async Task<WorkItemDetails> GetWorkItemDetailsExtendedAsync(int workItemId)
        {
            var url = $"{_baseUrl}_apis/wit/workitems/{workItemId}";
            var response = await _httpClient.GetAsync(url);
            response.EnsureSuccessStatusCode();
            var jsonString = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<WorkItemDetails>(jsonString);
        }
    }
}
