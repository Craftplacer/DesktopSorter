using System;
using System.IO;
using System.Linq;

namespace DesktopSorter
{
	internal static class Program
	{
		public static Tuple<string, string[]>[] Categories = new Tuple<string, string[]>[]
		{
			Tuple.Create("Binaries",            new []{ ".exe" }),
			Tuple.Create("Scripts",             new []{ ".bat", ".vbs", ".js",  ".ps1" }),
			Tuple.Create("Images",              new []{ ".png", ".bmp", ".jpg", ".jpeg", ".gif", ".ico", ".svg", ".webp" }),
			Tuple.Create("Audio",               new []{ ".ogg", ".mid", ".mp3", ".wav",  ".wma" }),
			Tuple.Create("Documents",           new []{ ".txt", ".rtf", ".doc" }),
			Tuple.Create("Videos",              new []{ ".mp4", ".mkv", ".avi", ".wmv", ".webm" }),
			Tuple.Create("Scratch Projects",    new []{ ".sb",  ".sb2", ".sb3" }),
			Tuple.Create("Source Engine Files", new []{ ".vtf", ".vmt" }),
			Tuple.Create("Shortcuts",           new []{ ".lnk", ".url" }),
			Tuple.Create("Archives",            new []{ ".zip", ".tar" }),
			Tuple.Create("Fonts",               new []{ ".fon", ".ttf", ".ttc" })
		};

		private static void Main()
		{
			string[] paths = new[]
			{
				Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory),
				Environment.GetFolderPath(Environment.SpecialFolder.CommonDesktopDirectory)
			};

			foreach (var directoryPath in paths)
			{
				foreach (string filePath in Directory.GetFiles(directoryPath))
				{
					var fileName = Path.GetFileName(filePath);

					// Ignores desktop.ini and system files
					if (new FileInfo(filePath).Attributes.HasFlag(FileAttributes.System) || fileName.Equals("desktop.ini", StringComparison.OrdinalIgnoreCase))
						continue;

					string extension = Path.GetExtension(filePath);
					string category = Categories.FirstOrDefault(c => c.Item2.Contains(extension))?.Item1;

					// Checks if a matching category has been found
					if (string.IsNullOrWhiteSpace(category))
						continue;

					string categoryPath = Path.Combine(directoryPath, category);

					// Creates the missing category directory
					if (!Directory.Exists(categoryPath))
						Directory.CreateDirectory(categoryPath);

					string destinationPath = Path.Combine(categoryPath, fileName);

					// Doesn't move file if it's being blocked by another one
					if (File.Exists(destinationPath))
						continue;

					File.Move(filePath, destinationPath);
				}
			}
		}
	}
}