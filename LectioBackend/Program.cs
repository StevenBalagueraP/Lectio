using Lectio.Api.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Lectio.Api.Infrastructure.ExternalServices.GoogleBooks;
using Lectio.Api.Infrastructure.ExternalServices.SeedState;
using Lectio.Api.Application.Seeding;
using Lectio.Api.Application.Seeding.UserSeed;
using Lectio.Api.Application.Seeding.LoanSeed;

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

builder.Services.AddControllers();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapControllers();

app.Run();