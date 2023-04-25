using Microsoft.EntityFrameworkCore;
using Post.Query.Api.Extensions;
using Post.Query.Domain.Repositories;
using Post.Query.Infra.DataAccess;
using Post.Query.Infra.Repositories;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.CreateDatabase(builder.Configuration);

builder.Services.AddControllers();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
