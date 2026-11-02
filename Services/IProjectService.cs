using daandemaat.github.io.Models;

namespace daandemaat.github.io.Services;

public interface IProjectService
{
    Task<List<Project>> GetAllAsync();
    Task<Project?> GetByIdAsync(int id);
    Task<Project> CreateAsync(Project dto);
    Task<bool> UpdateAsync(int id, Project dto);
    Task<bool> DeleteAsync(int id);
}