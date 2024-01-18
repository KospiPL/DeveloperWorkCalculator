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
        {
            _devOpsClient = devOpsClient;
        }

        [HttpGet("Ostatni_Sprint")]
        public async Task<IActionResult> GetLatestSprintWorkItems()
        {
            try
            {
                var (iterationId, iterationName) = await _devOpsClient.GetLatestIterationIdAsync();
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