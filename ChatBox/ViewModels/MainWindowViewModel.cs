 using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using ChatBox.Models;
using ChatBox.Services;
using DynamicData.Binding;
using MessageBox.Avalonia;
using MessageBox.Avalonia.Exceptions;
using Newtonsoft.Json;
using ReactiveUI;

namespace ChatBox.ViewModels;

public class MainWindowViewModel : ViewModelBase
{
	private List<MessageModel> _messages = new();

	private string _sendedMessage;

	private readonly HttpClient _client = new();

	private Action<int> _trigger;

	private string _selectedModel;

	private string _accessToken;

	private Saver _saver;

	private bool _isChatStarted;

	public MainWindowViewModel(Setting setting, Saver saver)
	{
		_selectedModel = setting.SelectedModel ?? "curie";
		_accessToken = setting.AccessToken;
		_saver = saver;
	}

	public ObservableCollectionExtended<MessageModel> MessageModels { get; } = new();

	public string SendedMessage
	{
		get => _sendedMessage;
		set => this.RaiseAndSetIfChanged(ref _sendedMessage, value);
	}

	public List<string> Models =>
		new()
		{
			"babbage",
			"curie",
			"ada",
			"davinci",
		};

	public string SelectedModel
	{
		get => _selectedModel;
		set => this.RaiseAndSetIfChanged(ref _selectedModel, value);
	}

	public string AccessToken
	{
		get => _accessToken;
		set => this.RaiseAndSetIfChanged(ref _accessToken, value);
	}

	public bool IsChatStarted
	{
		get => _isChatStarted;
		set => this.RaiseAndSetIfChanged(ref _isChatStarted, value);
	}

	public async Task SendMessage()
	{
		IsChatStarted = true;

		if (string.IsNullOrWhiteSpace(SendedMessage) || string.IsNullOrEmpty(SendedMessage))
		{
			return;
		}

		var message = new MessageModel(SendedMessage, "User", false);

		MessageModels.Add(message);
		;

		var sendedMessage = SendedMessage;
		SendedMessage = string.Empty;

		var sendMessage = await SendMessage(sendedMessage);

		var resultModel = new MessageModel(sendMessage, "Bot", true);

		MessageModels.Add(resultModel);
		_trigger?.Invoke(MessageModels.Count);
	}

	public void SetTrigger(Action<int> scrollToTop) => _trigger = scrollToTop;

	public async Task<string> SendMessage(string message)
	{
		var content = new StringContent(
			"{\"model\": \"" + SelectedModel + "\", \"prompt\": \"" + message + "\",\"temperature\": 1,\"max_tokens\": 1000}",
			Encoding.UTF8, "application/json");

		_client.DefaultRequestHeaders.Remove("Authorization");
		_client.DefaultRequestHeaders.Add("Authorization", "Bearer " + AccessToken);
		var response = await _client.PostAsync("https://api.openai.com/v1/completions", content);

		var responseString = await response.Content.ReadAsStringAsync();

		try
		{
			var dyData = JsonConvert.DeserializeObject<Response>(responseString);

			var guess = GuessCommand(dyData!.choices[0]
				.text);

			//Console.ForegroundColor = ConsoleColor.Green;
			//Console.WriteLine($"---> My guess at the command prompt is: {guess}");
			//Console.ResetColor();
			return guess;
		}
		catch (Exception ex)
		{
			MessageBoxManager.GetMessageBoxStandardWindow("Error", ex.Message);

			//Console.WriteLine($"---> Could not deserialize the JSON: {ex.Message}");
			return "Trouble compute";
		}
	}

	public void ReportBug()
	{
		var url = @"https://github.com/CreateLab/ChatBox/issues/new?assignees=&labels=bug&template=bug_report.md&title=";

		OpenUrl(url);
	}

	public void OpenAbout()
	{
		var url = "https://github.com/CreateLab/ChatBox/blob/master/README.md";

		OpenUrl(url);
	}

	private static void OpenUrl(string url)
	{
		if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
		{
			//https://stackoverflow.com/a/2796367/241446
			using var proc = new Process
			{
				StartInfo =
				{
					UseShellExecute = true,
					FileName = url
				}
			};

			proc.Start();

			return;
		}

		if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
		{
			Process.Start("x-www-browser", url);

			return;
		}

		if (!RuntimeInformation.IsOSPlatform(OSPlatform.OSX)) throw new InvalidUrlException("invalid url: " + url);

		Process.Start("open", url);
	}

	public void Exit()
	{
		SaveSettings();
		Environment.Exit(0);
	}

	public void SaveSettings()
	{
		var setting = new Setting
		{
			AccessToken = AccessToken,
			SelectedModel = SelectedModel
		};

		_saver.SaveSettings(setting);
	}

	public Task SaveChat()
	{
		var messageModels = MessageModels.ToList();
		var dialog = JsonConvert.SerializeObject(messageModels);

		return _saver.Save(dialog);
	}

	private static string GuessCommand(string raw) =>

		//Console.WriteLine("---> GPT-3 API Returned Text:");
		// Console.ForegroundColor = ConsoleColor.Yellow;
		//  Console.WriteLine(raw);
		// var lastIndex = raw.LastIndexOf('\n');
		// string guess = raw.Substring(lastIndex);
		// Console.ResetColor();
		//TextCopy.ClipboardService.SetText(guess);
		raw;
}