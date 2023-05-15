using Microsoft.Extensions.Configuration;
using RestSharp;

namespace GameHub.Blazor.Shared.Services;

[Injectable(InjectableAttribute.InjectionType.Singleton)]
public class MongoDbService
{
	static string? APIKEY;
	const string ENDPOINT = "https://us-east-2.aws.data.mongodb-api.com/app/data-giljf/endpoint/data/v1/action";
	const string GETENDPOINT = "https://us-east-2.aws.data.mongodb-api.com/app/data-giljf/endpoint/get";
	const string UPSERTENDPOINT = "https://us-east-2.aws.data.mongodb-api.com/app/data-giljf/endpoint/Upsert";
	const string GETROOMS = "https://us-east-2.aws.data.mongodb-api.com/app/data-giljf/endpoint/rooms";


	public static void SetAPIKey(string apiKey)
	{
		APIKEY = apiKey;// 81P3LLtkEplK6hLYSI3K3zWYLARHxi9Nsw3E3DvovpNIY1lZjCXVuSdy3ELWtaQ3
	}

	public MongoDbService(IConfiguration configuration)
	{
		APIKEY ??= configuration["APIKEY"]!;
	}

	static class Action
	{
		public const string FINDONE = "findOne";
		public const string INSERTONE = "insertOne";
		public const string UPDATEONE = "updateOne";
		public const string DELETEONE = "deleteOne";
	}

	static class DbConfig
	{
		public const string Collection = "Rooms";
		public const string Database = "GameHub";
		public const string DataSource = "GameHub0";
	}

	RestRequest GetRequest()
	{
		var request = new RestRequest();
		return request;
	}
	public async Task<string> TestGet()
	{
		var client = new RestClient(GETENDPOINT);
		var request = GetRequest();
		RestResponse response = await client.GetAsync(request);
		return response.Content ?? "No data";
	}

	public async Task<string> TestPost()
	{
		var client = new RestClient(UPSERTENDPOINT);
		var request = GetRequest("{ \"upsert\": true }");
		RestResponse response = await client.PostAsync(request);
		return response.Content ?? "No data";
	}

	RestRequest GetRequest(string body)
	{
		var request = GetRequest();
		//request.AddHeader("api-key", APIKEY!);
		//request.AddHeader("Content-Type", "application/json");
		//request.AddHeader("Access-Control-Request-Headers", "*");
		request.AddStringBody(body, DataFormat.Json);
		return request;
	}

	public async Task<string?> UpsertOne(int roomId, string update)
	{
		var client = new RestClient(UPSERTENDPOINT);
		client.AddDefaultQueryParameter("roomId", roomId.ToString());
		var request = GetRequest(update);
		RestResponse response = await client.PostAsync(request);
		return response.Content;
	}

	public async Task<string?> FindOne(int roomId)
	{
		var client = new RestClient(GETENDPOINT);
		client.AddDefaultQueryParameter("roomId", roomId.ToString());
		var request = GetRequest();
		RestResponse response = await client.GetAsync(request);
		return response.Content;
	}

	public async Task<string?> DeleteOne(string filter)
	{
		var client = new RestClient(Path.Combine(ENDPOINT, Action.DELETEONE));
		var request = GetRequest($@"
			{{
				""collection"":""{DbConfig.Collection}"",
				""database"":""{DbConfig.Database}"",
				""dataSource"":""{DbConfig.DataSource}"",
			    ""filter"": {{ {filter} }}
			}}");
		RestResponse response = await client.PostAsync(request);
		return response.Content;
	}

	public async Task<string?> GetRooms()
	{
		var client = new RestClient(GETROOMS);
		var request = GetRequest();
		RestResponse response = await client.GetAsync(request);
		return response.Content;
	}
}
