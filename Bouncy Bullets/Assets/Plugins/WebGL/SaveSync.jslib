mergeInto(LibraryManager.library, {
    SyncFiles: function () {
        FS.syncfs(false, function (err) {
            if (err) {
                console.log('Error synchronizing IndexedDB: ' + err);
            }
        });
    }
});