using GameHub.Blazor.Shared.Models;

namespace GameHub.Blazor.Shared.Services;

[Injectable(InjectableAttribute.InjectionType.Singleton)]
public class DataService
{
	private static bool _mock = true;
	private readonly SqlService _sqlService;
	private readonly MongoDbService _mongoDbService;

	public DataService(SqlService sqlService, MongoDbService mongoDbService)
	{
		_sqlService = sqlService;
		_mongoDbService = mongoDbService;
	}

	public async Task<bool> TryMakeRoom(int roomId)
	{
		var sql = $"IF NOT EXISTS(SELECT * FROM {SqlService.Tables.ROOMS} WHERE Room = {roomId}) INSERT INTO {SqlService.Tables.ROOMS} VALUES ({roomId});";

		return await _sqlService.Write(sql) > 0;
	}

	public async Task JoinRoom(int roomId, string name)
	{
		await TryMakeRoom(roomId);

		var sql = $"INSERT INTO {SqlService.Tables.ROOM_PEOPLE} VALUES ({roomId}, '{name}');";

		if (await _sqlService.Write(sql) == 0)
			throw new Exception("Something went wrong - data didn't write!");
	}

	public async Task LeaveRoom(int roomId, string name)
	{
		var sql = $"DELETE {SqlService.Tables.ROOM_PEOPLE} WHERE Room = {roomId} AND Name = '{name}';";

		if (await _sqlService.Write(sql) == 0)
			throw new Exception("Something went wrong - data didn't write!");
	}

	public async Task<GameState> GetRoom(int roomId)
	{
		var filter = $"\"Room\": {roomId}";
		var stateString = _mock ? mockRoom : await _mongoDbService.FindOne(filter);
		if (stateString is null) return new();
		var document = System.Text.Json.JsonSerializer.Deserialize<Document>(stateString);
		if (document?.document is null) return new();
		return document.document;
	}

	public async Task Save(GameState gameState)
	{
		if (_mock)
		{
			await Task.Delay(500);
			var doc = new Document()
			{
				document = gameState
			};
			mockRoom = System.Text.Json.JsonSerializer.Serialize(doc);
			Console.WriteLine(mockRoom);
		}
	}
	string mockRoom = @"
		{""document"": {
			""_id"":""644cef8de43033961f0121b1"",
			""Room"":1,
			""Deck"":[1,2,3,1],
			""Log"":[
				""Michael drew a card"",
				""Jennifer played the Auditor"",
				""Jennifer drew a card"",
				""Thomas is out"",
				""Locke played the Project Manager [Thomas]""
			],
			""Players"":[
				{	""Name"":""Michael"",
					""Hand"":[8,5],
					""Played"":[7,2],""Out"":false
				},{	""Name"":""Jennifer"",
					""Hand"":[4],
					""Played"":[2,5,6],
					""Out"":false
				},{	""Name"":""Locke"",
					""Hand"":[7],
					""Played"":[3,4],
					""Out"":false
				},{	""Name"":""Thomas"",
					""Hand"":[],
					""Played"":[4,1,2],
					""Out"":true
				}],
			""Turn"":""Michael""
		}}";
}
