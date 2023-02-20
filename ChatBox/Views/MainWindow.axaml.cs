using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Threading;
using ChatBox.Models;
using ChatBox.ViewModels;
using ReactiveUI;

namespace ChatBox.Views;

public partial class MainWindow : Window
{
	private ListBox list;

	public MainWindow()
	{
		InitializeComponent();
	}

	public void Sartup(Setting setting)
	{
		list = this.FindControl<ListBox>("MessageListBox");

		var vm = new MainWindowViewModel(setting);
		vm.SetTrigger(ScrollToTop);
		DataContext = vm;
	}

	private void ScrollToTop(int count)
	{
		if (count > 0)
		{
			Dispatcher.UIThread.InvokeAsync(() => list.ScrollIntoView(count));
		}
	}
}