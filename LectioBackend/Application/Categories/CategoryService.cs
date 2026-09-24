using Lectio.Api.Application.Categories.Dtos;
using Lectio.Api.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Lectio.Api.Application.Categories;

public class CategoriesService : ICategoriesService
{
    private readonly AppDbContext _context;

    public CategoriesService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<CategoriesResponseDto> GetAllCategoriesAsync(CancellationToken cancellationToken)
    {
        var categories = await _context.Categories
            .AsNoTracking()
            .Select(category => new CategorySummaryDto
            {
                Id = category.CategoryId,
                Name = category.CategoryName,
            })
            .ToListAsync(cancellationToken);

        return new CategoriesResponseDto { Data = categories };
    }
}