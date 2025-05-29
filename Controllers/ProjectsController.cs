using Microsoft.AspNetCore.Mvc;
using PosStore.DTOs;
using PosStore.Services.Interfaces;

namespace PosStore.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProjectsController : ControllerBase
    {
        private readonly IProjectService _projectService;

        public ProjectsController(IProjectService projectService)
        {
            _projectService = projectService;
        }

        /// <summary>
        /// Get all projects with pagination and filtering
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<PagedResult<ProjectDto>>> GetProjects([FromQuery] PaginationParameters parameters)
        {
            var result = await _projectService.GetProjectsAsync(parameters);
            return Ok(result);
        }

        /// <summary>
        /// Get all projects without pagination
        /// </summary>
        [HttpGet("all")]
        public async Task<ActionResult<IEnumerable<ProjectDto>>> GetAllProjects()
        {
            var projects = await _projectService.GetAllProjectsAsync();
            return Ok(projects);
        }

        /// <summary>
        /// Get project by ID
        /// </summary>
        [HttpGet("{id}")]
        public async Task<ActionResult<ProjectDto>> GetProject(long id)
        {
            var project = await _projectService.GetProjectByIdAsync(id);

            if (project == null)
            {
                return NotFound($"Project with ID {id} not found.");
            }

            return Ok(project);
        }

        /// <summary>
        /// Create a new project
        /// </summary>
        [HttpPost]
        public async Task<ActionResult<ProjectDto>> CreateProject(CreateProjectDto dto)
        {
            var project = await _projectService.CreateProjectAsync(dto);
            return CreatedAtAction(nameof(GetProject), new { id = project.Id }, project);
        }

        /// <summary>
        /// Update an existing project
        /// </summary>
        [HttpPut("{id}")]
        public async Task<ActionResult<ProjectDto>> UpdateProject(long id, UpdateProjectDto dto)
        {
            var project = await _projectService.UpdateProjectAsync(id, dto);

            if (project == null)
            {
                return NotFound($"Project with ID {id} not found.");
            }

            return Ok(project);
        }

        /// <summary>
        /// Delete a project (soft delete)
        /// </summary>
        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteProject(long id)
        {
            var result = await _projectService.DeleteProjectAsync(id);

            if (!result)
            {
                return NotFound($"Project with ID {id} not found.");
            }

            return NoContent();
        }

        /// <summary>
        /// Search projects by term
        /// </summary>
        [HttpGet("search")]
        public async Task<ActionResult<IEnumerable<ProjectDto>>> SearchProjects([FromQuery] string searchTerm)
        {
            if (string.IsNullOrEmpty(searchTerm))
            {
                return BadRequest("Search term is required.");
            }

            var projects = await _projectService.SearchProjectsAsync(searchTerm);
            return Ok(projects);
        }

        /// <summary>
        /// Get projects by status
        /// </summary>
        [HttpGet("status/{status}")]
        public async Task<ActionResult<IEnumerable<ProjectDto>>> GetProjectsByStatus(string status)
        {
            var projects = await _projectService.GetProjectsByStatusAsync(status);
            return Ok(projects);
        }

        /// <summary>
        /// Get projects by client
        /// </summary>
        [HttpGet("client/{client}")]
        public async Task<ActionResult<IEnumerable<ProjectDto>>> GetProjectsByClient(string client)
        {
            var projects = await _projectService.GetProjectsByClientAsync(client);
            return Ok(projects);
        }

        /// <summary>
        /// Get projects by priority
        /// </summary>
        [HttpGet("priority/{priority}")]
        public async Task<ActionResult<IEnumerable<ProjectDto>>> GetProjectsByPriority(string priority)
        {
            var projects = await _projectService.GetProjectsByPriorityAsync(priority);
            return Ok(projects);
        }

        /// <summary>
        /// Get projects summary
        /// </summary>
        [HttpGet("summary")]
        public async Task<ActionResult<IEnumerable<ProjectSummaryDto>>> GetProjectsSummary()
        {
            var projects = await _projectService.GetProjectsSummaryAsync();
            return Ok(projects);
        }

        /// <summary>
        /// Get overdue projects
        /// </summary>
        [HttpGet("overdue")]
        public async Task<ActionResult<IEnumerable<ProjectDto>>> GetOverdueProjects()
        {
            var projects = await _projectService.GetOverdueProjectsAsync();
            return Ok(projects);
        }
    }
}
