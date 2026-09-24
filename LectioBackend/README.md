```bash
Lectio.Api/
├── Program.cs
├── appsettings.json
│
├── Controllers/
│   ├── AuthController.cs
│   ├── BooksController.cs
│   ├── CategoriesController.cs
│   ├── LoansController.cs
│   ├── RatingsController.cs
│   └── ReadingStatusController.cs  
│
├── Middleware/
│   └── ExceptionMiddleware.cs
│
├── Application/
│   ├── DTOs/
│   │   ├── Books/
│   │   │   ├── BookDto.cs
│   │   │   ├── BookDetailDto.cs
│   │   │   └── BookSummaryDto.cs   
│   │   ├── Loans/
│   │   │   └── LoanDto.cs
│   │   ├── Ratings/
│   │   │   └── RatingDto.cs
│   │   └── Auth/
│   │       ├── RegisterDto.cs
│   │       └── LoginDto.cs
│   ├── Interfaces/
│   │   ├── IBookService.cs
│   │   ├── ILoanService.cs
│   │   ├── IRatingService.cs
│   │   └── IReadingStatusService.cs
│   └── Services/
│       ├── BookService.cs
│       ├── LoanService.cs
│       ├── RatingService.cs
│       └── ReadingStatusService.cs
│
├── Domain/
│   ├── Entities/
│   │   ├── Book.cs
│   │   ├── Author.cs
│   │   ├── BookAuthor.cs
│   │   ├── Category.cs
│   │   ├── BookCategory.cs
│   │   ├── Loan.cs
│   │   ├── Rating.cs
│   │   ├── ReadingStatus.cs
│   │   └── ApplicationUser.cs     
│   ├── Enums/
│   │   └── ReadingStatusType.cs    
│   └── Exceptions/
│       └── BusinessRuleException.cs   
│
└── Infrastructure/
    ├── Data/
    │   ├── AppDbContext.cs
    │   └── Configurations/        
    │       ├── BookConfiguration.cs
    │       ├── LoanConfiguration.cs
    │       ├── RatingConfiguration.cs
    │       └── ...
    ├── Migrations/        
    └── ExternalServices/
        └── GoogleBooksApiClient.cs  
```

## Migrations

```bash
dotnet ef migrations add CreateCategoryTable -o Infrastructure/Migrations
```

```bash
dotnet ef database update
```

## Implemented endpoints

### Books

**`GET /api/books`** — catalog with search, category filter, and real pagination.
- Query params: `search?` (title, ILike), `categoryIds?` (repeatable, e.g. `categoryIds=4&categoryIds=7`, multi-select OR filter), `page=1`, `pageSize=10`
- Only returns books with `isAvailable = true`
- Response:
```json
{
  "data": [
    {
      "id": 1,
      "title": "...",
      "imageUrl": "...",
      "rating": { "stars": 4.3 },
      "authors": [{ "id": 1, "name": "..." }],
      "categories": [{ "id": 1, "name": "..." }]
    }
  ],
  "pagination": { "page": 1, "pageSize": 10, "totalCount": 130, "totalPages": 13 }
}
```

**`GET /api/books/top-rated`** — top 5 highest-rated books, optionally filtered by category.
- Query params: `categoryId?` (single)
- Only returns books with `isAvailable = true`
- Response:
```json
{
  "data": [
    {
      "id": 1,
      "title": "...",
      "description": "...",
      "imageUrl": "...",
      "rating": { "stars": 4.3 },
      "authors": [{ "id": 1, "name": "..." }],
      "categories": [{ "id": 1, "name": "..." }]
    }
  ]
}
```

**`GET /api/books/{id}`** — full book detail (includes unavailable books, so direct links don't break).
- Response:
```json
{
  "idBook": 1,
  "titulo": "Cien años de soledad",
  "imagen": "...",
  "descripcion": "...",
  "numeroPaginas": 496,
  "anoPublicacion": 1967,
  "lenguaje": "Español",
  "autores": [{ "idAutor": 1, "nombreAutor": "Gabriel García Márquez" }],
  "categorias": [{ "idCategoria": 1, "nombreCategoria": "Ficción" }]
}
```

**`GET /api/books/{id}/ratings`** — a book's reviews/ratings, paginated (for the frontend's "Load more" pattern, not page numbers).
- Query params: `page=1`, `pageSize=10`
- `numberOfRatings` is the real count from the `Ratings` table for that book (not `Book`'s denormalized `RatingCount`), so the frontend can reliably decide whether to show "Load more"
- Response:
```json
{
  "averageRating": 4.3,
  "numberOfRatings": 125,
  "users": [
    {
      "idUser": 1,
      "comment": "A very enjoyable book.",
      "rating": 5,
      "updatedAt": "2026-09-20T15:30:00Z",
      "createdAt": "2026-09-18T12:00:00Z"
    }
  ]
}
```

### Categories

**`GET /api/categories`** — all categories (~25), with book count, ordered by `bookCount` descending. Used both for the filter checkboxes/dropdown and for the "top 10 + see more" on the home page.
- Response:
```json
{
  "data": [
    { "id": 1, "name": "Ficción", "bookCount": 4200 }
  ]
}
```
