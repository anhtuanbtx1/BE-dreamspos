using AutoMapper;
using Microsoft.EntityFrameworkCore;
using PosStore.Data;
using PosStore.DTOs;
using PosStore.Models;
using PosStore.Services.Interfaces;

namespace PosStore.Services.Service
{
    public class ProjectService : IProjectService
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;

        public ProjectService(ApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<IEnumerable<ProjectDto>> GetAllProjectsAsync()
        {
            var projects = await _context.Projects
                .Include(p => p.Category)
                .Include(p => p.CreatedByUser)
                // .Where(p => p.DeletedAt == null)  // Temporarily disabled until DB is updated
                .OrderBy(p => p.ProjectName)
                .ToListAsync();

            return _mapper.Map<IEnumerable<ProjectDto>>(projects);
        }

        public async Task<PagedResult<ProjectDto>> GetProjectsAsync(PaginationParameters parameters)
        {
            IQueryable<Project> query = _context.Projects
                .Include(p => p.Category)
                .Include(p => p.CreatedByUser);
                // .Where(p => p.DeletedAt == null);  // Temporarily disabled

            // Apply search filter
            if (!string.IsNullOrEmpty(parameters.SearchTerm))
            {
                query = query.Where(p =>
                    p.ProjectName.Contains(parameters.SearchTerm) ||
                    p.Description.Contains(parameters.SearchTerm) ||
                    p.ClientName.Contains(parameters.SearchTerm) ||
                    (p.Category != null && p.Category.Name.Contains(parameters.SearchTerm)));
            }

            // Apply category filter (using Category field for status filtering)
            if (!string.IsNullOrEmpty(parameters.Category))
            {
                query = query.Where(p => p.Status == parameters.Category);
            }

            // Apply sorting
            if (!string.IsNullOrEmpty(parameters.SortBy))
            {
                switch (parameters.SortBy.ToLower())
                {
                    case "name":
                    case "projectname":
                        query = parameters.SortOrder?.ToLower() == "desc"
                            ? query.OrderByDescending(p => p.ProjectName)
                            : query.OrderBy(p => p.ProjectName);
                        break;
                    case "startdate":
                        query = parameters.SortOrder?.ToLower() == "desc"
                            ? query.OrderByDescending(p => p.StartDate)
                            : query.OrderBy(p => p.StartDate);
                        break;
                    case "budget":
                        query = parameters.SortOrder?.ToLower() == "desc"
                            ? query.OrderByDescending(p => p.Budget)
                            : query.OrderBy(p => p.Budget);
                        break;
                    case "progress":
                    case "progresspercentage":
                        query = parameters.SortOrder?.ToLower() == "desc"
                            ? query.OrderByDescending(p => p.ProgressPercentage)
                            : query.OrderBy(p => p.ProgressPercentage);
                        break;
                    case "priority":
                        query = parameters.SortOrder?.ToLower() == "desc"
                            ? query.OrderByDescending(p => p.Priority)
                            : query.OrderBy(p => p.Priority);
                        break;
                    default:
                        query = query.OrderBy(p => p.ProjectName);
                        break;
                }
            }
            else
            {
                query = query.OrderBy(p => p.ProjectName);
            }

            // Get total count
            var totalCount = await query.CountAsync();

            // Apply pagination
            var projects = await query
                .Skip((parameters.Page - 1) * parameters.PageSize)
                .Take(parameters.PageSize)
                .ToListAsync();

            var projectDtos = _mapper.Map<IEnumerable<ProjectDto>>(projects);

            // Calculate pagination info
            var totalPages = (int)Math.Ceiling((double)totalCount / parameters.PageSize);

            return new PagedResult<ProjectDto>
            {
                Data = projectDtos,
                Pagination = new PaginationInfo
                {
                    CurrentPage = parameters.Page,
                    PageSize = parameters.PageSize,
                    TotalCount = totalCount,
                    TotalPages = totalPages,
                    HasPrevious = parameters.Page > 1,
                    HasNext = parameters.Page < totalPages
                }
            };
        }

        public async Task<ProjectDto?> GetProjectByIdAsync(long id)
        {
            var project = await _context.Projects
                .Include(p => p.Category)
                .Include(p => p.CreatedByUser)
                .FirstOrDefaultAsync(p => p.Id == id); // && p.DeletedAt == null);  // Temporarily disabled

            return project == null ? null : _mapper.Map<ProjectDto>(project);
        }

        public async Task<ProjectDto> CreateProjectAsync(CreateProjectDto createProjectDto)
        {
            try {
                var project = _mapper.Map<Project>(createProjectDto);
                project.CreatedAt = DateTime.UtcNow;
                project.UpdatedAt = DateTime.UtcNow;

                // Validate and fix Priority
                var validPriorities = new[] { "low", "medium", "high" };
                if (string.IsNullOrEmpty(project.Priority) || !validPriorities.Contains(project.Priority.ToLower()))
                {
                    project.Priority = "medium"; // Default to medium
                }

                // Validate CategoryId
                var categoryExists = await _context.ProjectCategories.AnyAsync(c => c.Id == project.CategoryId);
                if (!categoryExists)
                {
                    // Set to first available category or create default
                    var firstCategory = await _context.ProjectCategories.FirstOrDefaultAsync();
                    if (firstCategory != null)
                    {
                        project.CategoryId = firstCategory.Id;
                    }
                    else
                    {
                        // Create default category if none exists
                        var defaultCategory = new ProjectCategory
                        {
                            Name = "General",
                            Description = "Default category"
                        };
                        _context.ProjectCategories.Add(defaultCategory);
                        await _context.SaveChangesAsync();
                        project.CategoryId = defaultCategory.Id;
                    }
                }

                // Validate CreatedBy if provided
                if (createProjectDto.CreatedBy.HasValue)
                {
                    var userExists = await _context.Users.AnyAsync(u => u.Id == createProjectDto.CreatedBy.Value);
                    if (!userExists)
                    {
                        // Set to first available user or default to 1
                        var firstUser = await _context.Users.FirstOrDefaultAsync();
                        project.CreatedBy = firstUser?.Id ?? 1; // Default to user ID 1 if no users exist
                    }
                }
                else
                {
                    // If CreatedBy not provided, set to first available user or default to 1
                    var firstUser = await _context.Users.FirstOrDefaultAsync();
                    project.CreatedBy = firstUser?.Id ?? 1; // Default to user ID 1 if no users exist
                }

                _context.Projects.Add(project);
                await _context.SaveChangesAsync();
                // Reload with includes for proper mapping
                var createdProject = await _context.Projects
                    .Include(p => p.Category)
                    // .Include(p => p.CreatedByUser)
                    .FirstOrDefaultAsync(p => p.Id == project.Id);
                return _mapper.Map<ProjectDto>(createdProject);
            }
            catch (Exception ex)
            {
                var a = ex.Message;
                throw; // Re-throw to see the actual error
            }

            return null;
        }

        public async Task<ProjectDto?> UpdateProjectAsync(long id, UpdateProjectDto updateProjectDto)
        {
            var project = await _context.Projects.FindAsync(id);
            if (project == null) // || project.DeletedAt != null)  // Temporarily disabled
                return null;

            _mapper.Map(updateProjectDto, project);
            project.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            // Reload with includes for proper mapping
            var updatedProject = await _context.Projects
                .Include(p => p.Category)
                // .Include(p => p.CreatedByUser)
                .FirstOrDefaultAsync(p => p.Id == id);

            return _mapper.Map<ProjectDto>(updatedProject);
        }

        public async Task<bool> DeleteProjectAsync(long id)
        {
            var project = await _context.Projects.FindAsync(id);
            if (project == null)
                return false;

            // Hard delete for now (until DeletedAt column is added to DB)
            _context.Projects.Remove(project);
            // Soft delete (when DB is updated):
            // project.DeletedAt = DateTime.UtcNow;
            // project.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<ProjectDto>> SearchProjectsAsync(string searchTerm)
        {
            var projects = await _context.Projects
                .Include(p => p.Category)
                // .Include(p => p.CreatedByUser)
                .Where(p => p.ProjectName.Contains(searchTerm) ||
                     p.Description.Contains(searchTerm) ||
                     p.ClientName.Contains(searchTerm))
                .OrderBy(p => p.ProjectName)
                .ToListAsync();

            return _mapper.Map<IEnumerable<ProjectDto>>(projects);
        }

        public async Task<IEnumerable<ProjectDto>> GetProjectsByStatusAsync(string status)
        {
            var projects = await _context.Projects
                .Include(p => p.Category)
                // .Include(p => p.CreatedByUser)
                .Where(p => p.Status == status)
                .OrderBy(p => p.ProjectName)
                .ToListAsync();

            return _mapper.Map<IEnumerable<ProjectDto>>(projects);
        }

        public async Task<IEnumerable<ProjectDto>> GetProjectsByClientAsync(string client)
        {
            var projects = await _context.Projects
                .Include(p => p.Category)
                // .Include(p => p.CreatedByUser)
                .Where(p => p.ClientName == client)
                .OrderBy(p => p.ProjectName)
                .ToListAsync();

            return _mapper.Map<IEnumerable<ProjectDto>>(projects);
        }

        public async Task<IEnumerable<ProjectSummaryDto>> GetProjectsSummaryAsync()
        {
            var projects = await _context.Projects
                .Include(p => p.Category)
                .OrderBy(p => p.ProjectName)
                .ToListAsync();

            return _mapper.Map<IEnumerable<ProjectSummaryDto>>(projects);
        }

        public async Task<IEnumerable<ProjectDto>> GetOverdueProjectsAsync()
        {
            var today = DateTime.Today;
            var projects = await _context.Projects
                .Include(p => p.Category)
                // .Include(p => p.CreatedByUser)
                .Where(p => p.EndDate < today && p.Status != "completed")
                .OrderBy(p => p.EndDate)
                .ToListAsync();

            return _mapper.Map<IEnumerable<ProjectDto>>(projects);
        }

        public async Task<IEnumerable<ProjectDto>> GetProjectsByPriorityAsync(string priority)
        {
            var projects = await _context.Projects
                .Include(p => p.Category)
                // .Include(p => p.CreatedByUser)
                .Where(p => p.Priority == priority)
                .OrderBy(p => p.ProjectName)
                .ToListAsync();

            return _mapper.Map<IEnumerable<ProjectDto>>(projects);
        }
    }
}
