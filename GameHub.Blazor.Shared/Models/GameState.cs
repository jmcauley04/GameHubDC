namespace GameHub.Blazor.Shared.Models;

public class GameState
{
	public int Room { get; set; }
	public List<int> Deck { get; set; } = new List<int>();
	public List<string> Log { get; set; } = new List<string>();
	public List<Player> Players { get; set; } = new List<Player>();
	public string Turn { get; set; } = string.Empty;
}
