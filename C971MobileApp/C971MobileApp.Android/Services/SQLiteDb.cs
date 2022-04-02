using C971MobileApp.Droid.Services;
using C971MobileApp.Services;
using SQLite;
using System.IO;
using Xamarin.Forms;

[assembly: Dependency(typeof(SQLiteDb))]
namespace C971MobileApp.Droid.Services
{
    public class SQLiteDb : ISQLiteDb
    {
        public SQLiteAsyncConnection GetConnection()
        {
            // Path for SQL db storage/connection
            var docPath = System.Environment.GetFolderPath(System.Environment.SpecialFolder.MyDocuments);
            var path = Path.Combine(docPath, "C971MobileApp.db3");
            return new SQLiteAsyncConnection(path);
        }
    }
}