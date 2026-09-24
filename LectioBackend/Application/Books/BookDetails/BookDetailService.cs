using Lectio.Api.Application.Books.BookDetails.Dtos;
using Lectio.Api.Domain.Entities;
using Lectio.Api.Domain.Exceptions;
using Lectio.Api.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Lectio.Api.Application.Books.BookDetails;

public class BookDetailService : IBookDetailService
{
    private readonly AppDbContext _context;

    public BookDetailService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<BookDetailDto> GetBookDetailAsync(int bookId, CancellationToken cancellationToken)
    {
        var book = await _context.Books
            .AsNoTracking()
            .FirstOrDefaultAsync(b => b.BookId == bookId, cancellationToken);

        if (book is null)
        {
            throw new NotFoundException(nameof(Book), bookId);
        }

        var authors = await _context.BookAuthors
            .AsNoTracking()
            .Where(bookAuthor => bookAuthor.BookId == bookId)
            .OrderBy(bookAuthor => bookAuthor.AuthorId)
            .Select(bookAuthor => new BookAuthorDetailDto
            {
                IdAutor = bookAuthor.AuthorId,
                NombreAutor = bookAuthor.Author.AuthorName
            })
            .ToListAsync(cancellationToken);

        var categories = await _context.BookCategories
            .AsNoTracking()
            .Where(bookCategory => bookCategory.BookId == bookId)
            .OrderBy(bookCategory => bookCategory.CategoryId)
            .Select(bookCategory => new BookCategoryDetailDto
            {
                IdCategoria = bookCategory.CategoryId,
                NombreCategoria = bookCategory.Category.CategoryName
            })
            .ToListAsync(cancellationToken);

        return new BookDetailDto
        {
            IdBook = book.BookId,
            Titulo = book.Title,
            Imagen = book.ImageLink,
            Descripcion = book.Description,
            NumeroPaginas = book.PageCount, 
            AnoPublicacion = book.PublishedDate?.Year,
            Lenguaje = book.Language,
            Autores = authors,
            Categorias = categories
        };
    }
}