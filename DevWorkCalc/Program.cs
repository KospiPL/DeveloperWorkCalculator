using Microsoft.TeamFoundation.TestManagement.WebApi;
using Microsoft.VisualStudio.Services.TestManagement.TestPlanning.WebApi;
using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json.Linq;

namespace DevWorkCalc
{
    class Program
    {
        static async Task Main(string[] args)
        {
            // Uwierzytelnianie i konfiguracja klienta
            string personalAccessToken = "lrvvjpr3wiwzh2kswmjnwufiqucwtfanz3klwz7itzdcb5qt5o5a";
            string organization = "gearcodegit";
            string project = "GC.BAT";
            string team = "GC.Flota Team";

            var devOpsClient = new AzureDevOpsClient(personalAccessToken, organization, project, team);

            Console.WriteLine("Pobieranie ID najnowszego sprintu...");
            var (iterationId, iterationName) = await devOpsClient.GetLatestIterationIdAsync();
            Console.WriteLine($"ID najnowszego sprintu: {iterationId}, Nazwa: {iterationName}");

            Console.WriteLine("Pobieranie zadań z sprintu...");
            var workItemsListJson = await devOpsClient.GetWorkItemsFromSprintAsync(iterationId);
            var workItemsList = JsonConvert.DeserializeObject<WorkItemsList>(workItemsListJson);

            if (workItemsList?.WorkItemRelations != null)
            {
                foreach (var relation in workItemsList.WorkItemRelations)
                {
                    int workItemId = relation.Target.Id;
                    var workItemDetailsJson = await devOpsClient.GetWorkItemDetailsAsync(workItemId);
                    DateTime dateActive = DateTime.MinValue;
                    DateTime dateResolved = DateTime.MinValue;
                    var workItemResponse = JsonConvert.DeserializeObject<WorkItemResponse>(workItemDetailsJson);


                    if (workItemResponse?.Value != null)
                    {
                        foreach (var workItem in workItemResponse.Value)
                        {
                            if (workItem.Fields.TryGetValue("Microsoft.VSTS.Common.ActivatedDate", out var activatedDateObj) &&
                                activatedDateObj != null && DateTime.TryParse(activatedDateObj.ToString(), out dateActive))
                            {
                                
                            }
                            else
                            {
                                Console.WriteLine("Nie znaleziono Microsoft.VSTS.Common.ActivatedDate lub jest ona null.");
                            }

                            if (workItem.Fields.TryGetValue("Microsoft.VSTS.Common.ResolvedDate", out var resolvedDateObj) &&
                                resolvedDateObj != null && DateTime.TryParse(resolvedDateObj.ToString(), out dateResolved))
                            {
                                
                            }
                            else
                            {
                                Console.WriteLine("Nie znaleziono Microsoft.VSTS.Common.ResolvedDate lub jest ona null.");
                            }
                        }


                        if (dateActive != DateTime.MinValue && dateResolved != DateTime.MinValue)
                        {
                            
                            double workingHours = CalculateWorkingHours(dateActive, dateResolved);

                           
                            int storyPoints = (int)Math.Ceiling(workingHours / 8.0);

                           
                            var jsonPatchDocument = new[]
                            {
                                new
                                {
                                    op = "replace",
                                    path = "/fields/Microsoft.VSTS.Scheduling.StoryPoints",
                                    value = workingHours
                                }
                            };

                           
                            string patchJson = JsonConvert.SerializeObject(jsonPatchDocument);

                            
                            try
                            {
                                await devOpsClient.UpdateWorkItemAsync(workItemId, patchJson);
                                Console.WriteLine($"Zaktualizowano zadanie o ID: {workItemId} z {storyPoints} punktami Story Points.");
                            }
                            catch (Exception ex)
                            {
                               
                                Console.WriteLine($"Wystąpił błąd podczas aktualizacji zadania o ID: {workItemId}. Błąd: {ex.Message}");
                            }
                        }
                        else
                        {
                            
                            Console.WriteLine($"Nie można obliczyć godzin pracy dla zadania o ID: {workItemId}.");
                        }
                    }
                }
            }
            else
            {
                Console.WriteLine("Brak zadań do przetworzenia.");
            }
        }

        public static double CalculateWorkingHours(DateTime startDate, DateTime endDate)
        {
            double totalHours = 0;
            var current = startDate;

            while (current < endDate)
            {
                if (current.Hour >= 8 && current.Hour < 16 && current.DayOfWeek != DayOfWeek.Saturday && current.DayOfWeek != DayOfWeek.Sunday)
                {
                    totalHours++;
                }
                current = current.AddHours(1);
            }

            return totalHours;
        }
    }

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

        public async Task<string> GetWorkItemsFromSprintAsync(string iterationId)
        {
            var url = $"{_baseUrl}_apis/work/teamsettings/iterations/{iterationId}/workitems?api-version=7.2-preview.1";
            var response = await _httpClient.GetAsync(url);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadAsStringAsync();
            Console.WriteLine($"Odpowiedź z API dla zadań sprintu {iterationId} została odebrana.");
        }

        public async Task<string> GetWorkItemDetailsAsync(int workItemId)
        {
            var url = $"https://dev.azure.com/gearcodegit/GC.BAT/_apis/wit/workitems?ids={workItemId}&fields=System.Id,System.Title,System.WorkItemType,Microsoft.VSTS.Common.ActivatedDate,Microsoft.VSTS.Common.ResolvedDate,Microsoft.VSTS.Scheduling.StoryPoints&api-version=7.2-preview.3";
            var response = await _httpClient.GetAsync(url);

            if (!response.IsSuccessStatusCode)
            {
                Console.WriteLine($"Nie udało się pobrać szczegółów zadania o ID: {workItemId}. Status: {response.StatusCode}");
                return null; 
            }

            var content = await response.Content.ReadAsStringAsync();
            if (content == null)
            {
                Console.WriteLine($"Treść odpowiedzi dla zadania o ID: {workItemId} jest pusta.");
                return null;
            }

            Console.WriteLine($"Szczegóły zadania o ID: {workItemId} zostały odebrane.");
            return content;
        }

        public async Task<string> UpdateWorkItemAsync(int workItemId, string jsonPatchDocument)
        {
            var url = $"https://dev.azure.com/gearcodegit/GC.BAT/_apis/wit/workitems/{workItemId}?api-version=7.2-preview.3";
            var content = new StringContent(jsonPatchDocument, System.Text.Encoding.UTF8, "application/json-patch+json");
            var response = await _httpClient.PatchAsync(url, content);

            if (response.IsSuccessStatusCode)
            {
                Console.WriteLine($"Zadanie o ID: {workItemId} zostało zaktualizowane.");
                return await response.Content.ReadAsStringAsync();
            }
            else
            {
                
                Console.WriteLine($"Nie udało się zaktualizować zadania o ID: {workItemId}. Status: {response.StatusCode}");
                return null;
            }
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


    }
    public class WorkItem
    {
        public int Id { get; set; }
        public int Rev { get; set; }
        public Dictionary<string, object> Fields { get; set; }
    }
    public class WorkItemResponse
    {
        public int Count { get; set; }
        public List<WorkItem> Value { get; set; }
    }

    public class WorkItemRelation
    {
        public RelationTarget Target { get; set; }
    }

    public class RelationTarget
    {
        public int Id { get; set; }
        public string Url { get; set; }
    }

    public class WorkItemsList
    {
        public List<WorkItemRelation> WorkItemRelations { get; set; }
    }

    public class Iteration
    {
        public string Id { get; set; }
        public Attributes Attributes { get; set; }
        public string Name { get; set; }
    }

    public class Attributes
    {
        public DateTime? FinishDate { get; set; }
    }

    public class IterationsList
    {
        public List<Iteration> Value { get; set; }
    }
}
