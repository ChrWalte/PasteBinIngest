using PasteBinIngest.API;
using PasteBinIngest.Data.Interfaces;
using PasteBinIngest.Data.Repositories;
using PasteBinIngest.Services;
using PasteBinIngest.Shared;

var builder = WebApplication.CreateBuilder(args);

// Loggger:
var loggger = new Loggger(Constants.LogSaveLocation);
await loggger.Debug(Constants.InitLogger);

// pastebin setup:
var pastebinRepository = new PasteBinRepository(Constants.DataSaveLocation, loggger);
var pastebinService = new PasteBinService(Constants.PastebinRawUrl, pastebinRepository, loggger);
await loggger.Debug(Constants.InitRepoAndService);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddSingleton(loggger);
builder.Services.AddSingleton<IPasteBinRepository>(pastebinRepository);
builder.Services.AddSingleton(pastebinService);
builder.Services
    .AddGraphQLServer()
    .AddQueryType<Query>();

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
app.MapGraphQL();

await loggger.Debug("starting handling HTTP requests...");
app.Run();

await loggger.Debug("finished handling HTTP requests");
await loggger.Debug(Constants.ExitedLog);