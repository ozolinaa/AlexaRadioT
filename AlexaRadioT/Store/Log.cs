using System;
using System.Collections.Generic;
using Microsoft.Data.Sqlite;
using System.Linq;
using System.Threading.Tasks;
using System.Data.Common;
using AlexaRadioT.Models;
using Newtonsoft.Json;

namespace AlexaRadioT.Store
{
    public class Log
    {
        public static Guid LogAlexaRequest(string json)
        {
            Guid requestID = Guid.NewGuid();

            using (SqliteCommand cmd = DB.GetConnection().CreateCommand())
            {
                cmd.CommandText = "INSERT INTO [Requests] ([Id],[RequestedDateTimeUtc],[JSON]) VALUES (@Id,@RequestedDateTimeUtc,@JSON)";
                cmd.Parameters.AddWithValue("@Id", requestID.ToString());
                cmd.Parameters.AddWithValue("@RequestedDateTimeUtc", DateTime.UtcNow);
                cmd.Parameters.AddWithValue("@JSON", json);
                cmd.ExecuteNonQuery();
            }
            return requestID;
        }

        public static string[] GetLatestAlexaRequests(int count)
        {
            List<string> result = new List<string>();

            using (SqliteCommand cmd = DB.GetConnection().CreateCommand())
            {
                cmd.CommandText = "SELECT [Id],[RequestedDateTimeUtc],[JSON],[ResponseJSON] FROM [Requests] ORDER BY [RequestedDateTimeUtc] DESC LIMIT @rowCount";
                cmd.Parameters.AddWithValue("@rowCount", count);

                using (DbDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        result.Add((string)reader["JSON"]);
                    }
                }
            }

            return result.ToArray();
        }

        public static void LogAlexaResponse(Guid forRequestID, AlexaResponse response)
        {
            using (SqliteCommand cmd = DB.GetConnection().CreateCommand())
            {
                cmd.CommandText = "UPDATE [Requests] SET [ResponseJSON] = @ResponseJSON WHERE [Id] = @Id";
                cmd.Parameters.AddWithValue("@Id", forRequestID.ToString());
                cmd.Parameters.AddWithValue("@ResponseJSON", response == null ? "" : JsonConvert.SerializeObject(response, Formatting.Indented));
                cmd.ExecuteNonQuery();
            }
        }

        public static void LogException(Exception e)
        {
            using (SqliteCommand cmd = DB.GetConnection().CreateCommand())
            {
                cmd.CommandText = "INSERT INTO [ErrorLog] ([Id],[LoggedDateTimeUtc],[Error]) VALUES (@Id,@LoggedDateTimeUtc,@Error)";
                cmd.Parameters.AddWithValue("@Id", Guid.NewGuid().ToString());
                cmd.Parameters.AddWithValue("@LoggedDateTimeUtc", DateTime.UtcNow);
                cmd.Parameters.AddWithValue("@Error", e.ToString());

                cmd.ExecuteNonQuery();
            }
        }
    }
}
