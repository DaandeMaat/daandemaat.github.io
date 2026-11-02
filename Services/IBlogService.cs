using daandemaat.github.io.Models;

namespace daandemaat.github.io.Services;

public interface IBlogService
{
    Task<List<Blog>> GetAllAsync();
    Task<Blog?> GetByIdAsync(int id);
    Task<Blog> CreateAsync(Blog dto);
    Task<bool> UpdateAsync(int id, Blog dto);
    Task<bool> DeleteAsync(int id);
}