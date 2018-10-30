using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Database;
using Android.Database.Sqlite;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Java.IO;
using Java.Nio.Channels;

namespace Clipcoin.Phone.Database.Sqlite
{
    public class DBHelper : SQLiteOpenHelper
    {
        private const string DataBaseName = "AppDatabase";
        //private const string WifiRssiTableName = "WifiRssi";
        private Context context;

        public DBHelper(Context context) : base(context, DataBaseName, null, 1)
        {
            this.context = context;
        }

        public void DeleteId(string tablename, string id)
        {
            WritableDatabase.Delete(tablename, $"id={id}", null);
        }



        public override void OnCreate(SQLiteDatabase db)
        {
            //db.ExecSQL("create table " + "test" + " ("
            //    + "id integer primary key autoincrement,"
            //    + "ssid text,"
            //    + "bssid text,"
            //    + "rssi int"
            //    + ");");
        }

        public void CreateNewTable(string tablename, IDictionary<string, string> headers)
        {

            string sql = "create table " + tablename + " (";
            sql += "id integer primary key autoincrement,";

            foreach (var key in headers.Keys)
            {
                sql += key + " " + headers[key] + ",";
            }
            sql = sql.Substring(0, sql.Length - 1);
            sql += ");";

            this.WritableDatabase.ExecSQL(sql);
        }

        public void DropTable(string tableName)
        {
            string sql = "drop table if exists " + tableName;

            WritableDatabase.ExecSQL(sql);
        }

        public void ClearTable(string tableName)
        {
            if (TableExist(tableName))
            {
                this.WritableDatabase.Delete(tableName, null, null);

            }

            var result = TableExist(tableName);


        }

        public bool TableExist(string TableName)
        {
            bool result = false;

            if (TableName != null)
            {
                try
                {
                    var cursor = ReadableDatabase.RawQuery("select count(*) from sqlite_master where type = ? and name = ?", new string[] { "table", TableName });
                    if (cursor != null)
                    {
                        if (cursor.MoveToFirst())
                        {
                            int count = cursor.GetInt(0);
                            cursor.Close();

                            result = count > 0;
                        }
                    }
                }
                catch (SQLException e)
                {

                }
            }

            return result;
        }

        public void CopyDatabase(string path)
        {
            string prefix = "";
            DateTime time = DateTime.Now;
            prefix += $"_{time.Year}_{time.Month}_{time.Day}_{time.TimeOfDay.Hours}_{time.TimeOfDay.Minutes}_{time.TimeOfDay.Seconds}";
            Java.IO.File dsFile = new Java.IO.File(path, DatabaseName + prefix + ".sqlite");
            Java.IO.File dbFile = new Java.IO.File(this.ReadableDatabase.Path.Substring(0, this.ReadableDatabase.Path.Length - DatabaseName.Length - 1), DatabaseName);

            FileChannel inChannel = new FileInputStream(dbFile).Channel;
            FileChannel outChannel = new FileOutputStream(dsFile).Channel;

            try
            {
                inChannel?.TransferTo(0, inChannel.Size(), outChannel);
            }
            finally
            {
                inChannel?.Close();
                outChannel?.Close();
            }
        }

        public override void OnUpgrade(SQLiteDatabase db, int oldVersion, int newVersion)
        {

        }
    }
}