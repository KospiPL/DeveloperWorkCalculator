using D.W.C.Lib;
using D.W.C.Lib.D.W.C.Models;
using DevWorkCalc.D.W.C.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using D.W.C.API.D.W.C.Service;
using AutoMapper;

namespace D.W.C.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AzureDevOpsController : ControllerBase
    {
        private readonly AzureDevOpsClient _devOpsClient;
        private readonly IMapper _mapper;
        private readonly MyDatabaseContext _context;

        public AzureDevOpsController(IMapper mapper, AzureDevOpsClient devOpsClient, MyDatabaseContext context)
        {
            _mapper = mapper;
            _devOpsClient = devOpsClient ?? throw new ArgumentNullException(nameof(devOpsClient));
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        [HttpGet("Ostatni/sprint")]
        public async Task<IActionResult> GetLatestIteration()
        {
            try
            {
                IterationsListdto iterationListDto = await _devOpsClient.GetLatestIterationIdAsync();

                if (iterationListDto == null || iterationListDto.Value == null || !iterationListDto.Value.Any())
                {
                    return NotFound("Nie znaleziono iteracji.");
                }

                var iterations = _mapper.Map<List<Iteration>>(iterationListDto.Value);

                foreach (var iteration in iterations)
                {
                    var existingIteration = await _context.Iterations
                        .FirstOrDefaultAsync(i => i.ApiId == iteration.ApiId);

                    if (existingIteration != null)
                    {
                        _mapper.Map(iteration, existingIteration);
                    }
                    else
                    {
                        await _context.Iterations.AddAsync(iteration);
                    }
                }

                
                await _context.SaveChangesAsync();

                return Ok(iterations);
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
                WorkDetailsDto workItemDetailsDto = await _devOpsClient.GetWorkItemDetailsExtendedAsync(workItemId);
                var item = workItemDetailsDto.Value[0];
                var workItemDetailsEntity = _mapper.Map<WorkItemDetails>(item);

                _context.Add(workItemDetailsEntity);
                await _context.SaveChangesAsync();

                
                var workItemDetailsToReturn = _mapper.Map<WorkItemDetailsDto>(workItemDetailsEntity);

                return Ok(workItemDetailsToReturn);
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
                WorkItemHistoryListDto workItemHistoryListDto = await _devOpsClient.GetWorkItemHistoryAsync(workItemId);

              
                if (workItemHistoryListDto.Value == null || !workItemHistoryListDto.Value.Any())
                {
                    return NotFound("Nie znaleziono historii dla podanego ID elementu pracy.");
                }

                
                var workItemHistories = _mapper.Map<List<WorkItemHistory>>(workItemHistoryListDto.Value);

               
                foreach (var workItemHistory in workItemHistories)
                {
                    _context.WorkItemsHistory.Add(workItemHistory);
                }
                await _context.SaveChangesAsync();

                var workItemHistoriesDtoToReturn = _mapper.Map<List<WorkItemHistorydto>>(workItemHistories);

                return Ok(workItemHistoriesDtoToReturn);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"B³¹d podczas pobierania historii elementu pracy: {ex.Message}");
            }
        }
    }
}