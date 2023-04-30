using System.Data;
using System.Data.SqlClient;

namespace GameHub.Blazor.Shared.Services;

[Injectable(InjectableAttribute.InjectionType.Singleton)]
public class SqlService
{
	public static class Tables
	{
		public const string ROOMS = "dbo.GameHub_Rooms"; // Room
		public const string ROOM_PEOPLE = "dbo.GameHub_RoomPeople"; // Room, Name
		public const string ROOM_DECK = "dbo.GameHub_RoomDeck"; // Room, Card, Name nullable, Played default 0
		public const string ROOM_LOG = "dbo.GameHub_RoomLog"; // Room, Log
	}

	string GetConnectionString()
	{
		var csBuilder = new SqlConnectionStringBuilder()
		{
			DataSource = "SQL8004.site4now.net",
			InitialCatalog = "db_a8fb43_projecttracker",
			UserID = "db_a8fb43_projecttracker_admin",
			Password = "ptcrayons4me!",
			TrustServerCertificate = true
		};
		return csBuilder.ConnectionString;
	}

	public async Task<int> Write(string sql)
	{
		using var conn = new SqlConnection(GetConnectionString());
		using var cmd = conn.CreateCommand();
		try
		{
			await cmd.Connection.OpenAsync();
			cmd.CommandText = sql;
			return await cmd.ExecuteNonQueryAsync();
		}
		finally
		{
			await cmd.Connection.CloseAsync();
		}
	}

	public async Task<DataTable> Read(string sql)
	{
		var dt = new DataTable();

		using var conn = new SqlConnection(GetConnectionString());
		using var cmd = conn.CreateCommand();
		using var da = new SqlDataAdapter(cmd);

		try
		{
			await cmd.Connection.OpenAsync();
			cmd.CommandText = sql;
			var reader = await cmd.ExecuteReaderAsync();
			dt.Load(reader);

			return dt;
		}
		finally
		{
			await cmd.Connection.CloseAsync();
		}
	}
}
