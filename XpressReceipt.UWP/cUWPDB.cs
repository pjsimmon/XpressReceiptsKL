using SQLite;
using Xamarin.Forms;
using LocalDataAccess.UWP;
using Windows.Storage;
using System.IO;
[assembly: Dependency(typeof(cUWPDB))]
namespace XPressReceipt.UWP
{
	public class cUWPDB : IDatabaseConnection
	{
        public SQLiteConnection cUWPDB()
        {
			var dbName = "CustomersDb.db3";
			var path = Path.Combine(ApplicationData.
			  Current.LocalFolder.Path, dbName);
			return new SQLiteConnection(path);
		}
	}
}