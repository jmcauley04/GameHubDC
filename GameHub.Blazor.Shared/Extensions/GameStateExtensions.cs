using GameHub.Blazor.Shared.Models;

namespace GameHub.Blazor.Shared.Extensions;

public static class GameStateExtensions
{
	public static void AddLog(this GameState gameState, string message)
	{
		gameState.Log.Add(message);
		while (gameState.Log.Count > 6)
			gameState.Log.RemoveAt(0);
	}

	public static void Fold(this GameState gameState, Player player)
	{
		// play all cards
		while (player.Hand.Count > 0)
		{
			player.Played.Add(player.Hand[0]);
			player.Hand.RemoveAt(0);
		}

		gameState.AddLog($"{player.Name} is out.");

		if (gameState.Turn == player.Name)
			gameState.Pass(player);

		player.Out = true;
	}

	public static void Pass(this GameState gameState, Player player)
	{
		var inPlayers = gameState.Players.Where(x => !x.Out).ToList();

		// set whose turn it is
		if (inPlayers.Count > 1 && inPlayers.Contains(player))
		{
			var turnIndex = (inPlayers.IndexOf(player) + 1) % inPlayers.Count;
			gameState.Turn = inPlayers[turnIndex].Name;
		}

		gameState.AddLog($"{gameState.Turn}'s turn begins.");
	}

	public static void ShuffleAndDeal(this GameState gameState, Player player)
	{
		List<int> deck = new() { 1, 1, 1, 1, 1, 2, 2, 3, 3, 4, 4, 5, 5, 6, 7, 8 };
		List<int> shuffledDeck = new();

		while (deck.Any())
			shuffledDeck.Add(deck.GetRandom());

		shuffledDeck.Remove(0);

		var turn = Random.Shared.Next(gameState.Players.Count);

		gameState.Turn = gameState.Players[turn].Name;

		foreach (var p in gameState.Players)
		{
			p.Played = new();
			p.Out = false;
			p.Hand.Clear();
			p.Hand.Add(shuffledDeck[0]);
			shuffledDeck.RemoveAt(0);
		}

		gameState.Deck = shuffledDeck;
		gameState.AddLog($"{player.Name} has shuffled, removed one card from the game, and dealt one card to each player.");
	}

	public static void DiscardCard(this GameState gameState, Player player, int card)
	{
		if (player.Hand.Count == 0 || !player.Hand.Contains(card)) return;
		player.Hand.Remove(card);
		player.Played.Add(card);
		Pass(gameState, player);
		// check if deck is empty, if so, resolve the game
		if (gameState.Deck.Count == 0)
		{
			var winner = gameState.Players.MaxBy(x => x.Hand.Count > 0 ? x.Hand.Max() : 0);
			foreach (var p in gameState.Players)
				p.Out = p != winner;
		}
	}

	public static void DrawCard(this GameState gameState, Player player)
	{
		if (gameState.Deck.Count == 0) return;
		gameState.AddLog($"{player.Name} drew a card.");
		player.Hand.Add(gameState.Deck[0]);
		gameState.Deck.RemoveAt(0);
	}
}
