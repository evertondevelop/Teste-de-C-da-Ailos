using MediatR;
using Microsoft.Data.Sqlite;
using Microsoft.OpenApi.Models;
using Questao5.Application.Interfaces;
using Questao5.Application.Interfaces.CommandStore;
using Questao5.Application.Interfaces.QueryStore;
using Questao5.Application.Services;
using Questao5.Infrastructure.Database.CommandStore;
using Questao5.Infrastructure.Database.QueryStore;
using Questao5.Infrastructure.Services.Middlewares;
using Questao5.Infrastructure.Sqlite;
using System.Data;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

builder.Services.AddMediatR(Assembly.GetExecutingAssembly());

// sqlite
builder.Services.AddSingleton(new DatabaseConfig { Name = builder.Configuration.GetValue<string>("DatabaseName", "Data Source=database.sqlite") });
builder.Services.AddSingleton<IDatabaseBootstrap, DatabaseBootstrap>();
builder.Services.AddScoped<IDbConnection, SqliteConnection>((provider) => new SqliteConnection(builder.Configuration.GetValue<string>("DatabaseName", "Data Source=database.sqlite")));

// injeção de dependencia de repositórios
builder.Services.AddScoped<IContaCorrenteQueryStore, ContaCorrenteQueryStore>();
builder.Services.AddScoped<IIdempotenciaQueryStore, IdempotenciaQueryStore>();

builder.Services.AddScoped<IIdempotenciaCommandStore, IdempotenciaCommandStore>();
builder.Services.AddScoped<IMovimentoCommandStore, MovimentoCommandStore>();

// injeção de dependencia de serviços
builder.Services.AddScoped<IInserirMovimentoService, InserirMovimentoService>();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(
    options =>
    {
        options.SwaggerDoc("v1", 
            new OpenApiInfo { 
                Title = "Serviço de Conta Bancária", 
                Description = "O objetivo é desenvolver dois métodos de acordo com suas regras de negocío descritas no documento de auxílio Questão 5", 
                Version = "v1",
                Contact = new OpenApiContact { 
                    Name = "Desenvolvedor", 
                    Email = "wsantos.interview@hotmail.com",
                    Url = new Uri("https://github.com/evertondevelop/Teste-de-C-da-Ailos")
                }
            }
        );

        var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
        var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
        options.IncludeXmlComments(xmlPath);
    }
);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseMiddleware<ExceptionHandlingMiddleware>();

app.UseAuthorization();

app.MapControllers();

// sqlite
#pragma warning disable CS8602 // Dereference of a possibly null reference.
app.Services.GetService<IDatabaseBootstrap>().Setup();
#pragma warning restore CS8602 // Dereference of a possibly null reference.

app.Run();

// Informações úteis:
// Tipos do Sqlite - https://www.sqlite.org/datatype3.html


