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
			var sm = new SettingManager();
			var s = sm.Startup(desktop.Args);
			var mw = new MainWindow();
			mw.Sartup(s);
			desktop.MainWindow = mw;
		}

		base.OnFrameworkInitializationCompleted();
	}
}