using Lectio.Api.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Lectio.Api.Infrastructure.ExternalServices.GoogleBooks;
using Lectio.Api.Infrastructure.ExternalServices.SeedState;
using Lectio.Api.Application.Seeding;
using Lectio.Api.Application.Seeding.UserSeed;
using Lectio.Api.Application.Seeding.LoanSeed;
using Lectio.Api.Application.Seeding.RatingSeed;
using Lectio.Api.Application.Books.TopRatedBooks;
using Lectio.Api.Application.Categories;
using Lectio.Api.Application.Books.BrowseBooks;
using Lectio.Api.Application.Books.BookDetails;
using Lectio.Api.Application.Books.BookRatings;
using Lectio.Api.Middlewares;


var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddHttpClient<GoogleBooksApiClient>();


builder.Services.AddScoped<SeedProgressStore>();
builder.Services.AddScoped<GoogleBooksSeederService>();
builder.Services.AddScoped<UserDataGenerator>();
builder.Services.AddScoped<UserSeederService>();
builder.Services.AddScoped<LoanDataGenerator>();
builder.Services.AddScoped<LoanSeederService>();
builder.Services.AddScoped<RatingDataGenerator>();
builder.Services.AddScoped<RatingSeederService>();
builder.Services.AddScoped<ITopRatedBooksService, TopRatedBooksService>();
builder.Services.AddScoped<ICategoriesService, CategoriesService>();
builder.Services.AddScoped<IBrowseBooksService, BrowseBooksService>();
builder.Services.AddScoped<IBookDetailService, BookDetailService>();
builder.Services.AddScoped<IBookRatingsService, BookRatingsService>();

builder.Services.AddControllers();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseExceptionHandling(); 
app.UseHttpsRedirection();

app.MapControllers();

app.Run();