using CSVProcessor.Share.Interfaces;
using CSVProcessor.Share.Models;
using CSVProcessor.Share.Models.WorkOrders.DTO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace CSVProcessor.Controllers
{
    /// <summary>
    /// CSV controller
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class CSVController : ControllerBase
    {
        private readonly ICSVProcessorRepository _csvProcessorRepository;
        private readonly Configs _configs;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="csvProcessorRepository"></param>
        /// <param name="configs"></param>
        public CSVController(ICSVProcessorRepository csvProcessorRepository, IOptions<Configs> configs)
        {
            _csvProcessorRepository = csvProcessorRepository;
            _configs = configs.Value;
        }

        /// <summary>
        /// Processes CSV file
        /// </summary>
        /// <param name="csvFile"></param>
        /// <returns></returns>
        /// <response code="200">Action completed successfully</response>
        /// <response code="500">Internal server error</response>
        [HttpPost("upload")]
        [ProducesResponseType(statusCode: 200, type: typeof(File))]
        [ProducesResponseType(statusCode: 500, type: typeof(string))]
        public async Task<IActionResult> ProcessCSV([FromForm] WorkOrderCSVFile csvFile)
        {
            try
            {
                var rowCount = _csvProcessorRepository.CheckCSVRowCount(csvFile.CSVFile);

                // If rows count is <= limit that we have determined
                // process the file now
                if (rowCount <= _configs.NrOfRowsLimit)
                {
                    var result = await _csvProcessorRepository.ProcessCSV(csvFile.CSVFile);
                    return File(result, "text/csv", "workorders-report.csv");
                }
                else
                {
                    var jobId = await _csvProcessorRepository.ScheduleCSVProcessTask(csvFile.CSVFile);

                    return Accepted(new { jobId = jobId, message = "File upload scheduled and processing started." });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        /// <summary>
        /// Gets list of scheduled tasks that are going to be processed and their statuses
        /// </summary>
        /// <returns></returns>
        /// <response code="200">Action completed successfully</response>
        /// <response code="500">Internal server error</response>
        [HttpGet("tasks/status")]
        [ProducesResponseType(statusCode: 200, type: typeof(IEnumerable<WorkOrderTasks>))]
        [ProducesResponseType(statusCode: 500, type: typeof(string))]
        public async Task<IActionResult> GetCSVScheduledTasks()
        {
            try
            {
                var tasks = await _csvProcessorRepository.GetScheduledTasks();

                return Ok(tasks);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        /// <summary>
        /// Gets status of a specific task
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        /// <response code="200">Action completed successfully</response>
        /// <response code="500">Internal server error</response>
        [HttpGet("tasks/status/{id:int}")]
        [ProducesResponseType(statusCode: 200, type: typeof(WorkOrderTasks))]
        [ProducesResponseType(statusCode: 500, type: typeof(string))]
        public async Task<IActionResult> GetCSVTaskStatus([FromRoute] int id)
        {
            try
            {
                var task = await _csvProcessorRepository.GetCSVTaskStatus(id);

                return Ok(task);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        /// <summary>
        /// Downloads a CSV file for an task that has been processed
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        /// <response code="200">Action completed successfully</response>
        /// <response code="500">Internal server error</response>
        [HttpGet("tasks/status/{id:int}/download")]
        [ProducesResponseType(statusCode: 200, type: typeof(File))]
        [ProducesResponseType(statusCode: 500, type: typeof(string))]
        public async Task<IActionResult> DownloadCSVFile([FromRoute] int id)
        {
            try
            {
                var csvFile = await _csvProcessorRepository.GetCSVOutputFile(id);

                if (string.IsNullOrWhiteSpace(csvFile))
                    return NotFound("CSV file has not been processed yet!");

                var fileBytes = System.IO.File.ReadAllBytes(csvFile);
                var fileName = Path.GetFileName(csvFile);

                return File(fileBytes, "text/csv", fileName);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
    }
}