namespace GameHub.Blazor.Shared.Models;

public class Player
{
	public string Name { get; set; } = string.Empty;
	public List<int> Hand { get; set; } = new();
	public List<int> Played { get; set; } = new();
	public bool Out { get; set; }
	public bool Protected { get; set; }
}
