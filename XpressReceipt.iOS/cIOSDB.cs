using System;
using LocalDataAccess.iOS;
using SQLite;
using System.IO;
[assembly: Xamarin.Forms.Dependency(typeof(DatabaseConnection_iOS))]

namespace XpressReceipt.iOS
{
    public class cIOSDB
    {
        public SQLiteConnection cIOSDB()
        {
            var dbName = "XPressReceiptDb.db3";
            string personalFolder = System.Environment.
                GetFolderPath(Environment.SpecialFolder.Personal);
            String libraryFolder =
                Path.Combine(personalFolder, "..", "Library");
            var path = Path.Combine(libraryFolder, dbName);
            return new SQLiteConnection(path);
        }
    }
}

