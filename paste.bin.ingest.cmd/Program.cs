
// Imports:
using paste.bin.ingest.cmd;
using paste.bin.ingest.core.data.Repositories;
using paste.bin.ingest.core.services;
using System.Security.Cryptography;

// get path of executable and make path to logs and data
var rootDirectory = Path.GetDirectoryName(Environment.GetCommandLineArgs().FirstOrDefault()) ?? "./";

var logDirectory = Path.Join(rootDirectory, "logs");
var dataDirectory = Path.Join(rootDirectory, "data");

// Logger:
var logger = new Logger(logDirectory);
await logger.Debug(Constants.InitLogger);

// paste bin setup:
var pasteBinRepository = new PasteBinRepository(dataDirectory, logger);
var pasteBinService = new PasteBinService(Constants.PasteBinRawUrl, pasteBinRepository, logger);
await logger.Debug(Constants.InitRepoAndService);

// sleep a random amount of time, to throw off paste bin servers...
var randomNumber = RandomNumberGenerator.GetInt32(3600000);
await logger.Info($"sleeping for {(1080000 + randomNumber) / 1000} seconds...");
Thread.Sleep(millisecondsTimeout: 1080000 + randomNumber);

// send and save the paste bin request
await logger.Debug(Constants.StartedRequestLog);
var request = await pasteBinService.SendPasteBinRequestAsync(Constants.PasteBinBaseUrl);
await pasteBinService.SavePasteBinRequestAsync(request);
await logger.Debug(Constants.FinishedRequestLog);
await logger.Debug(Constants.ExitedLog);