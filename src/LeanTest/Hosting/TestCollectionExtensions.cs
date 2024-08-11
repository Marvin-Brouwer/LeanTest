using System.Security.Cryptography;

namespace LeanTest.Hosting;
internal static class TestCollectionExtensions
{
	internal static IReadOnlyList<T> Shuffle<T>(this IReadOnlyList<T> list, CancellationToken cancellationToken)
	{
		if (list.Count <= 1) return list;

		// https://stackoverflow.com/a/1262619/2319865
		var provider = RandomNumberGenerator.Create();
		var shuffledList = list.ToArray();

		int cursor = shuffledList.Length;
		while (cursor > 1)
		{
			if (cancellationToken.IsCancellationRequested) return Array.Empty<T>();

			var box = new byte[1];
			do provider.GetBytes(box);
			while (box[0] >= cursor * (byte.MaxValue / cursor));

			var selector = box[0] % cursor; cursor--;

			var value = shuffledList[selector];
			shuffledList[selector] = shuffledList[cursor];
			shuffledList[cursor] = value;
		}

		return shuffledList;
	}
}
