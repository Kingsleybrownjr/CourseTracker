using SQLite;

namespace C971MobileApp.Services
{
    public interface ISQLiteDb
    {
        SQLiteAsyncConnection GetConnection();
    }
}
