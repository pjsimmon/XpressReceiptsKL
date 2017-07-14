using SQLite;

namespace XpressReceipt
{
    public interface ISQLiteDB
    {
      SQLiteConnection DbConnection();
    }
}
