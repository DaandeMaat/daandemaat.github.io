using daandemaat.github.io.Models;

namespace daandemaat.github.io.Services;

public interface ITestService
{
    Task<List<Test>> GetAllAsync();
    Task<Test?> GetByIdAsync(int id);
    Task<Test> CreateAsync(Test dto);
    Task<bool> UpdateAsync(int id, Test dto);
    Task<bool> DeleteAsync(int id);
}