using System.IO;
using ChatBox.Models;
using Newtonsoft.Json;

namespace ChatBox.Services;

public class SettingManager
{
	public Setting Startup(string[] args)
	{
		Setting setting;

		var argSp = new ArgumentSplitter(args, new string[]
		{
			"-t",
			"-p"
		});

		if (argSp.TryGetValue("-p", out var path))
		{
			setting = TrySetUpSettingFromPath(path, out setting)
				? setting
				: new Setting();
		} else
		{
			if (!TryGetDefaultSetting(out setting))
			{
				setting = new Setting();
			}
		}

		if (argSp.TryGetValue("-t", out var token))
		{
			setting.AccessToken = token;
		}

		return setting;
	}

	private bool TryGetDefaultSetting(out Setting setting)
	{
		setting = null;

		var path = Saver.FilePath;

		if (!File.Exists(path))
		{
			return false;
		}

		setting = JsonConvert.DeserializeObject<Setting>(File.ReadAllText(path));

		return true;
	}

	private bool TrySetUpSettingFromPath(string value, out Setting setting)
	{
		setting = null;

		if (string.IsNullOrEmpty(value))
		{
			return false;
		}

		if (!File.Exists(value))
		{
			return false;
		}

		setting = JsonConvert.DeserializeObject<Setting>(File.ReadAllText(value));

		return true;
	}
}