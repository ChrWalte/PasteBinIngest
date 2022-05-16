using paste.bin.ingest.api;
using paste.bin.ingest.core.data.Interfaces;
using paste.bin.ingest.core.data.Repositories;
using paste.bin.ingest.core.services;

var builder = WebApplication.CreateBuilder(args);

// get path of executable and make path to logs and data
var rootDirectory = System.IO.Path.GetDirectoryName(args.FirstOrDefault());
var logDirectory = System.IO.Path.Join(rootDirectory, "logs");
var dataDirectory = System.IO.Path.Join(rootDirectory, "data");

// Logger:
var logger = new Logger(logDirectory);
await logger.Debug(Constants.InitLogger);

// paste bin setup:
var pasteBinRepository = new PasteBinRepository(dataDirectory, logger);
var pasteBinService = new PasteBinService(Constants.PasteBinRawUrl, pasteBinRepository, logger);
await logger.Debug(Constants.InitRepoAndService);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddSingleton(logger);
builder.Services.AddSingleton<IPasteBinRepository>(pasteBinRepository);
builder.Services.AddSingleton(pasteBinService);
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

await logger.Debug("starting handling HTTP requests...");
app.Run();

await logger.Debug("finished handling HTTP requests");
await logger.Debug(Constants.ExitedLog);