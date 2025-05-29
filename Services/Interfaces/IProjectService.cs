using PosStore.DTOs;

namespace PosStore.Services.Interfaces
{
    public interface IProjectService
    {
        Task<IEnumerable<ProjectDto>> GetAllProjectsAsync();
        Task<PagedResult<ProjectDto>> GetProjectsAsync(PaginationParameters parameters);
        Task<ProjectDto?> GetProjectByIdAsync(long id);
        Task<ProjectDto> CreateProjectAsync(CreateProjectDto createProjectDto);
        Task<ProjectDto?> UpdateProjectAsync(long id, UpdateProjectDto updateProjectDto);
        Task<bool> DeleteProjectAsync(long id);
        Task<IEnumerable<ProjectDto>> SearchProjectsAsync(string searchTerm);
        Task<IEnumerable<ProjectDto>> GetProjectsByStatusAsync(string status);
        Task<IEnumerable<ProjectDto>> GetProjectsByClientAsync(string client);
        Task<IEnumerable<ProjectSummaryDto>> GetProjectsSummaryAsync();
        Task<IEnumerable<ProjectDto>> GetOverdueProjectsAsync();
        Task<IEnumerable<ProjectDto>> GetProjectsByPriorityAsync(string priority);
    }
}
