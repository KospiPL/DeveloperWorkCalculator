<<<<<<< HEAD
using D.W.C.Lib;
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
=======
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace D.W.C.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WorkItemsController : ControllerBase
    {
        private readonly AzureDevOpsClient _devOpsClient;

        public WorkItemsController(AzureDevOpsClient devOpsClient)
>>>>>>> 215f3fdc5446b037aed93af09a585e23f793fd4e
        {
            _devOpsClient = devOpsClient;
        }

<<<<<<< HEAD
        [HttpGet("iteration/latest")]
        public async Task<IActionResult> GetLatestIteration()
=======
        [HttpGet("Ostatni_Sprint")]
        public async Task<IActionResult> GetLatestSprintWorkItems()
>>>>>>> 215f3fdc5446b037aed93af09a585e23f793fd4e
        {
            try
            {
                var (iterationId, iterationName) = await _devOpsClient.GetLatestIterationIdAsync();
<<<<<<< HEAD
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
    }
}
=======
                var workItemsListJson = await _devOpsClient.GetWorkItemsFromSprintAsync(iterationId);


                return Ok(new { IterationId = iterationId, IterationName = iterationName, WorkItems = workItemsListJson });
            }
            catch (System.Exception ex)
            {

                return StatusCode(500, "Wyst¹pi³ b³¹d: " + ex.Message);
            }
        }
    }
}
>>>>>>> 215f3fdc5446b037aed93af09a585e23f793fd4e
