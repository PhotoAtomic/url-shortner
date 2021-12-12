

using api.Configurations;
using api.Persistence;
using api.Persistence.Cosmos;
using api.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

ConfigurationManager configurationManager = builder.Configuration;
var configuration = configurationManager.Get<Configuration>();

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(
                      builder =>
                      {
                          foreach (var origin in configuration.ClientOrigins)
                          {
                              builder
                                .AllowAnyMethod()
                                .AllowAnyHeader()
                                .AllowCredentials()
                                .WithOrigins(origin);
                          }
                      });
});



builder.Services.Configure<UrlShorteningServiceConfiguration>(builder.Configuration.GetSection(nameof(UrlShorteningServiceConfiguration)));
builder.Services.Configure<PersistenceConfiguration>(builder.Configuration.GetSection(nameof(PersistenceConfiguration)));

builder.Services.AddSingleton<IUnitOfWorkFactory, CosmosUnitOfWorkFactory>();
builder.Services.AddSingleton<UrlShorteningService>();


var app = builder.Build();

app.UseCors();

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
