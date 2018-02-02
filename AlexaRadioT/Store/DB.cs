using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Threading.Tasks;

namespace AlexaRadioT.Store
{
    public class DB : IDisposable
    {
        private static readonly DB instance = new DB();
        private static readonly SqliteConnection con = new SqliteConnection("" +
            new SqliteConnectionStringBuilder
            {
                DataSource = "RadioTSQLite.db"
            });

        static DB()
        {
        }

        private DB()
        {
        }

        public static DB Instance
        {
            get
            {
                return instance;
            }
        }

        public void Dispose()
        {
            con.Dispose();
        }

        public static SqliteConnection GetConnection()
        {
            if (con.State == ConnectionState.Closed) {
                con.Open();
                SeedDB(con);
            }
                
            return con;
        }


        private static void SeedDB(SqliteConnection con) {
            using (SqliteCommand cmd = con.CreateCommand())
            {
                cmd.CommandText = "CREATE TABLE IF NOT EXISTS Requests([Id] [NVARCHAR(50)] NOT NULL PRIMARY KEY, [RequestedDateTimeUtc] [datetime] NOT NULL, [JSON] [TEXT] NOT NULL, [ResponseJSON] [TEXT] NULL)";
                cmd.ExecuteNonQuery();
            }

            using (SqliteCommand cmd = con.CreateCommand())
            {
                cmd.CommandText = "CREATE TABLE IF NOT EXISTS ErrorLog([Id] [NVARCHAR(50)] NOT NULL PRIMARY KEY, [LoggedDateTimeUtc] [datetime] NOT NULL, [Error] [TEXT] NOT NULL)";
                cmd.ExecuteNonQuery();
            }

            using (SqliteCommand cmd = con.CreateCommand())
            {
                cmd.CommandText = "CREATE TABLE IF NOT EXISTS Users([Id] [NVARCHAR(255)] NOT NULL PRIMARY KEY, [CreatedDateTimeUtc] [datetime] NOT NULL, [LastActiveDateTimeUtc] [datetime] NOT NULL, [ListeningAudioToken] [NVARCHAR(255)] NULL, [OffsetInMilliseconds] [bigint] NULL)";
                cmd.ExecuteNonQuery();
            }
        }
    }
}
