using D.W.C.Lib;
using D.W.C.Lib.D.W.C.Models;
using DevWorkCalc.D.W.C.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using D.W.C.API.D.W.C.Service;

namespace D.W.C.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AzureDevOpsController : ControllerBase
    {
        private readonly AzureDevOpsClient _devOpsClient;
        private readonly MyDatabaseContext _context;

        public AzureDevOpsController(AzureDevOpsClient devOpsClient, MyDatabaseContext context)
        {
            _devOpsClient = devOpsClient ?? throw new ArgumentNullException(nameof(devOpsClient));
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        [HttpGet("Ostatni/sprint")]
        public async Task<IActionResult> GetLatestIteration()
        {
            try
            {
                var (iterationId, iterationName) = await _devOpsClient.GetLatestIterationIdAsync();


                return Ok(new { IterationId = iterationId, IterationName = iterationName });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"B³¹d podczas pobierania najnowszej iteracji: {ex.Message}");
            }
        }

        [HttpGet("sprint/workitems/{iterationId}")]
        public async Task<IActionResult> GetWorkItemsFromSprint(string iterationId)
        {
            if (string.IsNullOrEmpty(iterationId))
            {
                return BadRequest("Identyfikator iteracji nie mo¿e byæ pusty.");
            }

            try
            {
                var workItemsListJson = await _devOpsClient.GetWorkItemsFromSprintAsync(iterationId);
                var workItemsList = JsonConvert.DeserializeObject<WorkItemsList>(workItemsListJson);

                

                return Ok(workItemsList);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"B³¹d podczas pobierania elementów pracy z sprintu: {ex.Message}");
            }
        }

        [HttpGet("workitem/details/{workItemId}")]
        public async Task<IActionResult> GetWorkItemDetails(int workItemId)
        {
            try
            {
                var workItemDetailsJson = await _devOpsClient.GetWorkItemDetailsAsync(workItemId);
                var workItemDetails = JsonConvert.DeserializeObject<WorkItemDetails>(workItemDetailsJson);

            

                return Ok(workItemDetails);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"B³¹d podczas pobierania szczegó³ów elementu pracy: {ex.Message}");
            }
        }

        [HttpGet("workitem/extended/{workItemId}")]
        public async Task<IActionResult> GetWorkItemDetailsExtended(int workItemId)
        {
            try
            {
                var workItemDetails = await _devOpsClient.GetWorkItemDetailsExtendedAsync(workItemId);

                _context.Add(workItemDetails);
                await _context.SaveChangesAsync();

                return Ok(workItemDetails);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"B³¹d podczas pobierania rozszerzonych szczegó³ów elementu pracy: {ex.Message}");
            }
        }

        [HttpPatch("workitem/update/{workItemId}")]
        public async Task<IActionResult> UpdateWorkItem(int workItemId, [FromBody] string jsonPatchDocument)
        {
            if (string.IsNullOrWhiteSpace(jsonPatchDocument))
            {
                return BadRequest("Dokument JSON Patch nie mo¿e byæ pusty.");
            }

            try
            {
                await _devOpsClient.UpdateWorkItemAsync(workItemId, jsonPatchDocument);


                return Ok("Element pracy zosta³ pomyœlnie zaktualizowany.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"B³¹d podczas aktualizacji elementu pracy: {ex.Message}");
            }
        }

        [HttpGet("workitem/history/{workItemId}")]
        public async Task<IActionResult> GetWorkItemHistory(int workItemId)
        {
            try
            {
                var workItemHistory = await _devOpsClient.GetWorkItemHistoryAsync(workItemId);

                _context.WorkItemHistories.Add(workItemHistory);
                await _context.SaveChangesAsync();

                return Ok(workItemHistory);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"B³¹d podczas pobierania historii elementu pracy: {ex.Message}");
            }
        }
    }
}