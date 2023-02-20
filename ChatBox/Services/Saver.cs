using System;
using System.IO;
using System.Threading.Tasks;
using Avalonia.Controls;
using ChatBox.Models;
using Newtonsoft.Json;

namespace ChatBox.Services;

public class Saver
{
	public static string FilePath => Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "ChatBox", "settings.json");
	private Window _window;

	public Saver(Window window)
	{
		_window = window;
	}

	public async Task Save(string dialog)
	{
		var sd = new SaveFileDialog();

		sd.Filters.Add(new FileDialogFilter()
		{
			Name = "TXT",
			Extensions =
			{
				"txt"
			}
		});

		var showAsync = await sd.ShowAsync(_window);

		if (showAsync != null)
		{
			await File.WriteAllTextAsync(showAsync, dialog);
		}
	}

	public void SaveSettings(Setting setting)
	{
		var path = FilePath;

		if (!Directory.Exists(Path.GetDirectoryName(path)))
		{
			Directory.CreateDirectory(Path.GetDirectoryName(path));
		}

		File.WriteAllText(path, JsonConvert.SerializeObject(setting));
	}
}