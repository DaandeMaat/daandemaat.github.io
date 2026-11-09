using daandemaat.github.io.Data;
using daandemaat.github.io.Models;
using Microsoft.EntityFrameworkCore;

namespace daandemaat.github.io.Services;

public class TestService : ITestService
{
    private readonly AppDbContext _context;

    public TestService(AppDbContext context) => _context = context;

    public async Task<List<Test>> GetAllAsync()
    {
        return await _context.Tests.ToListAsync();
    }

    public async Task<Test?> GetByIdAsync(int id)
    {
        return await _context.Tests.FindAsync(id);
    }

    public async Task<Test> CreateAsync(Test dto)
    {
        _context.Tests.Add(dto);
        await _context.SaveChangesAsync();
        return dto;
    }

    public async Task<bool> UpdateAsync(int id, Test dto)
    {
        var existingTest = await _context.Tests.FindAsync(id);
        if (existingTest == null)
            return false;

        existingTest.Onderdeel = dto.Onderdeel;
        existingTest.Beschrijving = dto.Beschrijving;
        existingTest.Ezelsbrug = dto.Ezelsbrug;

        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var test = await _context.Tests.FindAsync(id);
        if (test == null)
            return false;

        _context.Tests.Remove(test);
        await _context.SaveChangesAsync();
        return true;
    }
}