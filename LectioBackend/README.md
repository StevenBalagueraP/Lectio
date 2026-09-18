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