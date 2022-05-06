// Imports:
using paste.bin.ingest.cmd;
using paste.bin.ingest.core.data.Repositories;
using paste.bin.ingest.core.services;

// Logger:
var logger = new Logger(Constants.LogSaveLocation);
await logger.Debug(Constants.InitLogger);

// paste bin setup:
var pasteBinRepository = new PasteBinRepository(Constants.DataSaveLocation, logger);
var pasteBinService = new PasteBinService(Constants.PasteBinRawUrl, pasteBinRepository, logger);
await logger.Debug(Constants.InitRepoAndService);

// send and save the paste bin request
await logger.Debug(Constants.StartedRequestLog);
var request = await pasteBinService.SendPasteBinRequestAsync(Constants.PasteBinBaseUrl);
await pasteBinService.SavePasteBinRequestAsync(request);
await logger.Debug(Constants.FinishedRequestLog);
await logger.Debug(Constants.ExitedLog);