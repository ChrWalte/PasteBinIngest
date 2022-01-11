// Imports:
using PasteBinIngest.Data.Interfaces;
using PasteBinIngest.Data.Repositories;
using PasteBinIngest.Services;

// load configuration
// handle arguments
// you just have them:
// args

// WILL REPLACE:
// Constants:
const string pastebinBaseUrl = "https://pastebin.com/archive";
const string pastebinRawUrl = "https://pastebin.com/raw";
const string dataSaveLocation = "E:\\pastebin";
const string logSaveLocation = "E:\\pastebin\\logs";

// Loggger:
var loggger = new Loggger(logSaveLocation);

// Service Setup:
var pasteBinService = new PasteBinService(pastebinRawUrl, loggger);
var request = pasteBinService.GetRequest(pastebinBaseUrl);

// Data Setup:
// MOVE INTO SERVICE
IPasteBinRepository pasteBinRepository = new PasteBinRepository(dataSaveLocation, loggger);
pasteBinRepository.SaveRequest(request);
