using AlexaRadioT.Models;
using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Threading.Tasks;

namespace AlexaRadioT.Store
{
    public class User
    {
        public static AlexaUserModel GetById(string id)
        {

            AlexaUserModel result = null;
            result = _selectByID(id);
            if (result == null)
                result = _createNew(id);

            return result;
        }

        private static AlexaUserModel _selectByID(string id)
        {
            AlexaUserModel result = null;

            using (SqliteCommand cmd = DB.GetConnection().CreateCommand())
            {
                cmd.CommandText = "SELECT [Id], [CreatedDateTimeUtc], [LastActiveDateTimeUtc], [ListeningAudioToken], [OffsetInMilliseconds] FROM [Users] WHERE [Id] = @Id";
                cmd.Parameters.AddWithValue("@Id", id);

                using (DbDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        result = new AlexaUserModel()
                        {
                            Id = (string)reader["Id"],
                            ListeningAudioToken = reader["ListeningAudioToken"] == DBNull.Value ? null : (string)reader["ListeningAudioToken"],
                            OffsetInMilliseconds = reader["OffsetInMilliseconds"] == DBNull.Value ? null : (long?)reader["OffsetInMilliseconds"]
                        };
                    }
                }
            }

            return result;
        }

        private static AlexaUserModel _createNew(string id)
        {
            using (SqliteCommand cmd = DB.GetConnection().CreateCommand())
            {

                cmd.CommandText = "INSERT INTO [Users] ([Id], [CreatedDateTimeUtc], [LastActiveDateTimeUtc]) VALUES (@Id, @CurrentDateTime, @CurrentDateTime)";
                cmd.Parameters.AddWithValue("@Id", id);
                cmd.Parameters.AddWithValue("@CurrentDateTime", DateTime.UtcNow);
                cmd.ExecuteNonQuery();
            }
            return _selectByID(id);
        }

        public static void SaveListenPosition(string userId, string audioToken, long offsetInMilliseconds)
        {
            using (SqliteCommand cmd = DB.GetConnection().CreateCommand())
            {
                cmd.CommandText = "UPDATE [Users] SET [LastActiveDateTimeUtc] = @CurrentDateTime, [ListeningAudioToken] = @audioToken, [OffsetInMilliseconds] = @offsetInMilliseconds WHERE [Id] = @Id";
                cmd.Parameters.AddWithValue("@CurrentDateTime", DateTime.UtcNow);
                cmd.Parameters.AddWithValue("@Id", userId);
                cmd.Parameters.AddWithValue("@audioToken", audioToken);
                cmd.Parameters.AddWithValue("@offsetInMilliseconds", offsetInMilliseconds);
                cmd.ExecuteNonQuery();
            }
        }

        public static void ClearListenPosition(string userId)
        {
            using (SqliteCommand cmd = DB.GetConnection().CreateCommand())
            {
                cmd.CommandText = "UPDATE [dbo].[Users] SET [LastActiveDateTimeUtc] = @ActiveDateTime, [ListeningAudioToken] = NULL, [OffsetInMilliseconds] = NULL WHERE [Id] = @Id";
                cmd.Parameters.AddWithValue("@ActiveDateTime", DateTime.UtcNow);
                cmd.Parameters.AddWithValue("@Id", userId);
                cmd.ExecuteNonQuery();
            }
        }
    }
}
