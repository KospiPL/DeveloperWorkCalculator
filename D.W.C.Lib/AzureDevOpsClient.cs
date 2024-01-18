using D.W.C.Lib.D.W.C.Models;
using DevWorkCalc.D.W.C.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace D.W.C.Lib
{
    public class AzureDevOpsClient
    {
        private readonly HttpClient _httpClient;
        private readonly string _baseUrl;

        public AzureDevOpsClient(string personalAccessToken, string organization, string project, string team)
        {
            _httpClient = new HttpClient();
            _baseUrl = $"https://dev.azure.com/{organization}/{project}/{team}/";
            _httpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Basic",
                    Convert.ToBase64String(
                        System.Text.Encoding.ASCII.GetBytes($":{personalAccessToken}")));
        }

        public async Task<(string id, string name)> GetLatestIterationIdAsync()
        {
            var url = $"{_baseUrl}_apis/work/teamsettings/iterations?api-version=7.2-preview.1";
            var response = await _httpClient.GetAsync(url);
            response.EnsureSuccessStatusCode();
            var jsonString = await response.Content.ReadAsStringAsync();

            var iterations = JsonConvert.DeserializeObject<IterationsList>(jsonString);

            if (iterations?.Value != null && iterations.Value.Any())
            {
                var latestIteration = iterations.Value
                    .Where(i => i.Attributes.FinishDate != null)
                    .OrderByDescending(i => i.Attributes.FinishDate)
                    .FirstOrDefault();

                if (latestIteration != null)
                {
                    return (latestIteration.Id, latestIteration.Name);
                }
            }

            throw new InvalidOperationException("No iterations found or iterations data is null.");
        }

        public async Task<string> GetWorkItemsFromSprintAsync(string iterationId)
        {
            var url = $"{_baseUrl}_apis/work/teamsettings/iterations/{iterationId}/workitems?api-version=7.2-preview.1";
            var response = await _httpClient.GetAsync(url);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadAsStringAsync();
        }

        public async Task<string> GetWorkItemDetailsAsync(int workItemId)
        {
            var url = $"{_baseUrl}_apis/wit/workitems/{workItemId}?fields=System.Id,System.Title,System.WorkItemType,Microsoft.VSTS.Common.ActivatedDate,Microsoft.VSTS.Common.ResolvedDate,Microsoft.VSTS.Scheduling.StoryPoints&api-version=7.2-preview.3";
            var response = await _httpClient.GetAsync(url);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadAsStringAsync();
        }

        public async Task<WorkItemDetails> GetWorkItemDetailsExtendedAsync(int workItemId)
        {
            var url = $"{_baseUrl}_apis/wit/workitems/{workItemId}";
            var response = await _httpClient.GetAsync(url);
            response.EnsureSuccessStatusCode();
            var jsonString = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<WorkItemDetails>(jsonString);
        }

        public async Task UpdateWorkItemAsync(int workItemId, string jsonPatchDocument)
        {
            var url = $"{_baseUrl}_apis/wit/workitems/{workItemId}?api-version=7.2-preview.3";
            var content = new StringContent(jsonPatchDocument, System.Text.Encoding.UTF8, "application/json-patch+json");
            var response = await _httpClient.PatchAsync(url, content);
            response.EnsureSuccessStatusCode();
        }
    }
}
