using paste.bin.ingest.core.data.Repositories;
using paste.bin.ingest.core.services;

// this is a script project used to run various scripts throughout the project lifetime
// these scripts are not runnable at anytime, they were created at a certain point in the project lifetime and will only work with that data
// update any scripts before running
// comment out scripts after finished

// set up:
// might want to change the data location to save to another folder to prevent data loss
var loggger = new Logger("E:\\pastebin\\scripts.logs");
var pasteBinRepository = new PasteBinRepository("E:\\pastebin\\", loggger);

// run script:
// comment out after

// var fileAdjustmentScripts = new FileAdjustments(pasteBinRepository, loggger);
// await fileAdjustmentScripts.RenameEntryFolderNamesFromUrlToIds();

// var duplicateDataScripts = new DuplicateData(pasteBinRepository, loggger);
// await duplicateDataScripts.CheckAndRemoveDuplicateDataAsync();

await loggger.Info("exited.\n");