using System;
using System.IO;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Avalonia.Threading;
using ChatBox.Models;
using ChatBox.Services;
using ChatBox.ViewModels;
using ChatBox.Views;
using Newtonsoft.Json;

namespace ChatBox;

public partial class App : Application
{
	public override void Initialize() => AvaloniaXamlLoader.Load(this);

	public override void OnFrameworkInitializationCompleted()
	{

		if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
		{
			var s = Startup(desktop.Args);
			var mw = new MainWindow();
			mw.Sartup(s);
			desktop.MainWindow = mw;
		}

		base.OnFrameworkInitializationCompleted();
	}

	private Setting Startup(string[] args)
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
			setting = new Setting();
		}

		if(argSp.TryGetValue("-t", out var token))
		{
			setting.AccessToken = token;
		}

		return setting;
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