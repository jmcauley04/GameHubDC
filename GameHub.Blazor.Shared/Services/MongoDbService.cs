﻿using Microsoft.Extensions.Configuration;
using RestSharp;

namespace GameHub.Blazor.Shared.Services;

[Injectable(InjectableAttribute.InjectionType.Singleton)]
public class MongoDbService
{
	static string? APIKEY;
	const string ENDPOINT = "https://us-east-2.aws.data.mongodb-api.com/app/data-giljf/endpoint/data/v1/action";

	public static void SetAPIKey(string apiKey)
	{
		APIKEY = apiKey;
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

	class Body
	{
		public string Collection { get; set; } = "Rooms";
		public string Database { get; set; } = "GameHub";
		public string DataSource { get; set; } = "GameHub0";
	}

	RestRequest GetRequest()
	{
		var request = new RestRequest();
		request.AddHeader("Content-Type", "application/json");
		request.AddHeader("Access-Control-Request-Headers", "*");
		request.AddHeader("api-key", APIKEY);
		return request;
	}

	RestRequest GetRequest(string body)
	{
		var request = GetRequest();
		request.AddStringBody(body, DataFormat.Json);
		return request;
	}

	public async Task<string?> UpsertOne(string filter, string update)
	{
		var client = new RestClient(Path.Combine(ENDPOINT, Action.UPDATEONE));
		var request = GetRequest($@"{{
	""collection"":""Rooms"",
	""database"":""GameHub"",
	""dataSource"":""GameHub0"",
	""upsert"": true,
	""filter"": {{ {filter} }},
	""update"": {{ 
		""$set"": {{
			{update} 
		}}
	}}
}}");
		RestResponse response = await client.PostAsync(request);
		return response.Content;
	}

	public async Task<string?> FindOne(string filter)
	{
		var client = new RestClient(Path.Combine(ENDPOINT, Action.FINDONE));
		var request = GetRequest($@"
			{{
			    ""collection"":""Rooms"",
			    ""database"":""GameHub"",
			    ""dataSource"":""GameHub0"",
			    ""filter"": {{ {filter} }}
			}}");
		RestResponse response = await client.PostAsync(request);
		return response.Content;
	}

	public async Task<string?> DeleteOne(string filter)
	{
		var client = new RestClient(Path.Combine(ENDPOINT, Action.DELETEONE));
		var request = GetRequest($@"
			{{
			    ""collection"":""Rooms"",
			    ""database"":""GameHub"",
			    ""dataSource"":""GameHub0"",
			    ""filter"": {{ {filter} }}
			}}");
		RestResponse response = await client.PostAsync(request);
		return response.Content;
	}

	public async Task Test()
	{
		var document = @"
			""Players"": {
				""Michael"": ""8_72"",
				""Thomas"": ""4_12"",
				""Locke"": ""7_34"",
				""Jennifer"": ""4_256""
			},
			""Deck"": ""1231"",
			""Turn"": ""Michael""
			";
		var filter = @"
			""Room"": 1
			";

		await UpsertOne(filter, document);
		await FindOne(filter);
		await DeleteOne(filter);
	}
}