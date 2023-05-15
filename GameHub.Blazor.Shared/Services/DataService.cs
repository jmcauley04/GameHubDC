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

	public async Task<string> Test()
	{
		await _mongoDbService.TestGet();
		await _mongoDbService.TestPost();
		return "";
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

	public async Task<GameState[]?> GetRooms()
	{
		var stateString = await _mongoDbService.GetRooms();

		if (stateString is null) return Enumerable.Empty<GameState>().ToArray();
		return System.Text.Json.JsonSerializer.Deserialize<GameState[]>(stateString);
	}

	public async Task LeaveRoom(int roomId, string name)
	{
	}

	public async Task<GameState?> GetRoom(int roomId)
	{
		var stateString = _mock ?
			(roomId == 1 ? mockRoom : null) :
			await _mongoDbService.FindOne(roomId);

		if (stateString is null) return new();
		return System.Text.Json.JsonSerializer.Deserialize<GameState>(stateString);
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
		}
		else
			await _mongoDbService.UpsertOne(
					gameState.Room,
					System.Text.Json.JsonSerializer.Serialize(gameState)
				);

	}
	string mockRoom = @"
		{""document"": null}";
}
