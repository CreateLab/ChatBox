using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using ChatBox.Models;
using DynamicData.Binding;
using MessageBox.Avalonia;
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

	public MainWindowViewModel(Setting setting)
	{
		_selectedModel = setting.SelectedModel ?? "text-davinci-003";
		_accessToken = setting.AccessToken;
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
			"text-davinci-003",
			"text-davinci-002",
			"code-davinci-002"
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

	public async Task SendMessage()
	{
		var message = new MessageModel
		{
			Message = SendedMessage,
			Username = "User"
		};

		MessageModels.Add(message);
		var sendMessage = await SendMessage(SendedMessage);

		var resultModel = new MessageModel
		{
			Message = sendMessage,
			Username = "Bot"
		};

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
			return "обработка не удалась";
		}
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