namespace GameHub.Blazor.Shared.Extensions;

public static class ListExtensions
{
	public static T GetRandom<T>(this List<T> items)
	{
		var i = Random.Shared.Next(items.Count - 1);
		var item = items[i];
		items.RemoveAt(i);
		return item;
	}
}
