using Lectio.Api.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Lectio.Api.Infrastructure.ExternalServices.GoogleBooks;
using Lectio.Api.Infrastructure.ExternalServices.SeedState;
using Lectio.Api.Application.Seeding;
using Lectio.Api.Application.Seeding.UserSeed;
using Lectio.Api.Application.Seeding.LoanSeed;
using Lectio.Api.Application.Seeding.RatingSeed;
using Lectio.Api.Application.Seeding.CategoryTranslation;
using Lectio.Api.Application.Books.TopRatedBooks;
using Lectio.Api.Application.Categories;
using Lectio.Api.Application.Books.BrowseBooks;
using Lectio.Api.Application.Books.BookDetails;
using Lectio.Api.Application.Books.BookRatings;
using Lectio.Api.Application.Loans.UserLoans;
using Lectio.Api.Application.Loans.CreateLoan;
using Lectio.Api.Application.Loans.ReturnLoan;
using Lectio.Api.Application.Ratings.UpsertRating;
using Lectio.Api.Middlewares;
using Lectio.Api.Application.Auth;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.OpenApi;


const string FrontendCorsPolicy = "Frontend";
var builder = WebApplication.CreateBuilder(args);

var jwtSection = builder.Configuration.GetSection(JwtSettings.SectionName);
var jwtSettings = jwtSection.Get<JwtSettings>();

if (jwtSettings is null || jwtSettings.Secret.Length < 32)
{
    throw new InvalidOperationException(
        "Jwt:Secret is missing or shorter than 32 characters. " +
        "Set it with: dotnet user-secrets set \"Jwt:Secret\" \"<random string, 32+ chars>\"");
}

builder.Services.Configure<JwtSettings>(jwtSection);

builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.MapInboundClaims = false;
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = jwtSettings.Issuer,
            ValidateAudience = true,
            ValidAudience = jwtSettings.Audience,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.Secret)),
            ValidateLifetime = true,
            ClockSkew = TimeSpan.FromSeconds(30),
            NameClaimType = "fullName",
            RoleClaimType = "role"
        };
    });

builder.Services.AddAuthorization();

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        Description = "Pega solo el token JWT (sin la palabra 'Bearer')"
    });

    options.AddSecurityRequirement(document => new OpenApiSecurityRequirement
    {
        [new OpenApiSecuritySchemeReference("Bearer", document)] = new List<string>()
    });
});

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddHttpClient<GoogleBooksApiClient>();

builder.Services.AddCors(options =>
{
    options.AddPolicy(FrontendCorsPolicy, policy =>
        policy
            .WithOrigins("http://localhost:5173")
            .AllowAnyHeader()
            .AllowAnyMethod());
});


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
builder.Services.AddSingleton<ITokenService, TokenService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IUserLoansService, UserLoansService>();
builder.Services.AddScoped<ICreateLoanService, CreateLoanService>();
builder.Services.AddScoped<IReturnLoanService, ReturnLoanService>();
builder.Services.AddScoped<IUpsertRatingService, UpsertRatingService>();
builder.Services.AddScoped<ICategoryTranslationService, CategoryTranslationService>();

builder.Services.AddControllers();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors(FrontendCorsPolicy);
app.UseExceptionHandling(); 
app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();