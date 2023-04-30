using GameHub.Blazor.Shared.Models;

namespace GameHub.Blazor.Shared.Services;

[Injectable(InjectableAttribute.InjectionType.Singleton)]
public class DataService
{
	private static bool _mock = false;
	private readonly MongoDbService _mongoDbService;

	string RoomFilter(int roomId) => $"\"Room\": {roomId}";

	public DataService(MongoDbService mongoDbService)
	{
		_mongoDbService = mongoDbService;
	}

	public async Task<GameState> TryMakeRoom(int roomId)
	{
		var gameState = await GetRoom(roomId);

		if (gameState is null)
		{
			gameState = new GameState()
			{
				Room = roomId
			};

			await Save(gameState);
		}
		return gameState;
	}

	public async Task JoinRoom(int roomId, string name)
	{
		var gameState = await TryMakeRoom(roomId);

		if (!gameState.Players.Any(x => x.Name == name))
		{
			gameState.Players.Add(new Player()
			{
				Name = name,
				Out = true
			});

			await Save(gameState);
		}
	}

	public async Task LeaveRoom(int roomId, string name)
	{
	}

	public async Task<GameState?> GetRoom(int roomId)
	{
		var stateString = _mock ?
			(roomId == 1 ? mockRoom : null) :
			await _mongoDbService.FindOne(RoomFilter(roomId));

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
		else
			await _mongoDbService.UpsertOne(
					RoomFilter(gameState.Room),
					System.Text.Json.JsonSerializer.Serialize(gameState)
				);

	}
	//string mockRoom = @"
	//	{""document"": {
	//		""_id"":""644cef8de43033961f0121b1"",
	//		""Room"":1,
	//		""Deck"":[1],
	//		""Log"":[],
	//		""Players"":[],
	//		""Turn"":""""
	//	}}";
	string mockRoom = @"
		{""document"": null}";
}
