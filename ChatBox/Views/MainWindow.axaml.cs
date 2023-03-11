using System;
using System.ComponentModel;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Input;
using Avalonia.Threading;
using ChatBox.Models;
using ChatBox.Services;
using ChatBox.ViewModels;
using ReactiveUI;

namespace ChatBox.Views;

public partial class MainWindow : Window
{
	private ScrollViewer scroll;

	public MainWindow()
	{
		InitializeComponent();
	}

	public void Sartup(Setting setting)
	{

		scroll = this.FindControl<ScrollViewer>("Scroll");

		var s = new Saver(this);
		var vm = new MainWindowViewModel(setting, s);
		vm.SetTrigger(ScrollToTop);
		DataContext = vm;
	}

	private void ScrollToTop(int count)
	{
		if (count > 0)
		{
			Dispatcher.UIThread.InvokeAsync(() => scroll.ScrollToEnd() );
		}
	}

	private void Window_OnClosing(object? sender, CancelEventArgs e)
	{
		var vm = (MainWindowViewModel) DataContext;
		vm.SaveSettings();
	}
}