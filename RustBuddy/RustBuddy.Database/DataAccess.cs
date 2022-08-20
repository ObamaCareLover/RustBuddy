using Dapper;
using System.Data;
using MySql.Data.MySqlClient;

using RustBuddy.Database.Models;

namespace RustBuddy.Database
{
    public class DataAccess
    {
        private static readonly IDbConnection connection = new MySqlConnection(@"server=localhost;userid=app;password=EuPQt771CDp0O!%sHDXFZHubDHdVCM;database=rustbuddy");

        public static bool OpenConnection()
        {
            try
            {
                connection.Open();
                return true;
            }
            catch (MySqlException ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }
        }

        private static async Task<List<T>> LoadDataAsync<T, U>(string statement, U parameters)
        {
            if (connection.State == ConnectionState.Closed)
                throw new Exception("Connection is closed");

            var rows = await connection.QueryAsync<T>(statement, parameters);

            return rows.ToList();
        }

        private static async Task SaveDataAsync<T>(string statement, T parameters)
        {
            if (connection.State == ConnectionState.Closed)
                throw new Exception("Connection is closed");

            /*return connection.ExecuteAsync(statement, parameters);*/
        }

        public static async Task<List<Item>> GetItems()
        {
            var items = await LoadDataAsync<Item, dynamic>("call GetItems()", new { });
            return items.ToList();
        }

        public static async Task<List<Entity>> GetEntities(ulong guild_id)
        {
            var entities = await LoadDataAsync<Entity, dynamic>("call GetEntities(@GuildId)", new { GuildId = guild_id });
            return entities.ToList();
        }
    }
}
