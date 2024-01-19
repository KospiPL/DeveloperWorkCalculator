using D.W.C.Lib;
using D.W.C.Lib.D.W.C.Models;
using DevWorkCalc.D.W.C.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Threading.Tasks;

namespace D.W.C.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AzureDevOpsController : ControllerBase
    {
        private readonly AzureDevOpsClient _devOpsClient;

        public AzureDevOpsController(AzureDevOpsClient devOpsClient)
        {
            _devOpsClient = devOpsClient ?? throw new ArgumentNullException(nameof(devOpsClient));
        }

        [HttpGet("iteration/latest")]
        public async Task<IActionResult> GetLatestIteration()
        {
            try
            {
                var (iterationId, iterationName) = await _devOpsClient.GetLatestIterationIdAsync();
                return Ok(new { IterationId = iterationId, IterationName = iterationName });
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet("sprint/workitems/{iterationId}")]
        public async Task<IActionResult> GetWorkItemsFromSprint(string iterationId)
        {
            try
            {
                var workItemsListJson = await _devOpsClient.GetWorkItemsFromSprintAsync(iterationId);
                var workItemsList = JsonConvert.DeserializeObject<WorkItemsList>(workItemsListJson);
                return Ok(workItemsList);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
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
                return StatusCode(500, ex.Message);
            }
        }
        [HttpGet("workitem/extended/{workItemId}")]
        public async Task<IActionResult> GetWorkItemDetailsExtended(int workItemId)
        {
            try
            {
                var workItemDetails = await _devOpsClient.GetWorkItemDetailsExtendedAsync(workItemId);
                return Ok(workItemDetails);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPatch("workitem/update/{workItemId}")]
        public async Task<IActionResult> UpdateWorkItem(int workItemId, [FromBody] string jsonPatchDocument)
        {
            try
            {
                await _devOpsClient.UpdateWorkItemAsync(workItemId, jsonPatchDocument);
                return Ok("Work item updated successfully.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

    }
}






