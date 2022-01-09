
// Imports:
using PasteBinIngest.Data.Repositories;
using PasteBinIngest.Services;

// Loggger:
var loggger = new Loggger("E:\\pastebin\\logs");

// Constants:
const string pastebinBaseUrl = "https://pastebin.com/archive";
const string pastebinRawUrl = "https://pastebin.com/raw";
const string dataSaveLocation = "E:\\pastebin";

// Service Layer:
var pasteBinService = new PasteBinService(pastebinBaseUrl, pastebinRawUrl, loggger);
var request = pasteBinService.GetRequest();

// Data Layer:
var pasteBinRepository = new PastePinRepository(dataSaveLocation, loggger);
pasteBinRepository.SaveRequest(request);
