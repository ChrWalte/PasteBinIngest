// Imports:
using PasteBinIngest.CMD;
using PasteBinIngest.Data.Repositories;
using PasteBinIngest.Services;
using PasteBinIngest.Shared;

// Loggger:
var loggger = new Loggger(Constants.LogSaveLocation);
await loggger.Debug(Constants.InitLogger);

// pastebin setup:
var pasteBinRepository = new PasteBinRepository(Constants.DataSaveLocation, loggger);
var pasteBinService = new PasteBinService(Constants.PastebinRawUrl, pasteBinRepository, loggger);
await loggger.Debug(Constants.InitRepoAndService);

await loggger.Debug(Constants.StartedRequestLog);
var request = await pasteBinService.SendPasteBinRequestAsync(Constants.PastebinBaseUrl);
await pasteBinService.SavePasteBinRequestAsync(request);
await loggger.Debug(Constants.FinishedRequestLog);
await loggger.Debug(Constants.ExitedLog);