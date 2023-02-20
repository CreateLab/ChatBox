using System.Security.Cryptography;
using System.Text;

namespace ChatBox.Models;

/// <summary>
/// Модель сообщения
/// </summary>
public class MessageModel
{
	/// <summary>
	/// Сообщение
	/// </summary>
	public string Message { get; private set; }

	/// <summary>
	/// Имя пользователя
	/// </summary>
	public string Username { get; private set; }

	public bool Bot { get; private set; }
	public bool User { get;  private set;}

	/// <summary>
	/// Обновляет модель
	/// </summary>
	/// <param name="model">Модель</param>
	public void Update(MessageModel model)
	{
		Message = model.Message;
		Username = model.Username;
	}

	public MessageModel(string message, string username, bool isBot)
	{
		Message = message;
		Username = username;
		Bot = isBot;
		User = !isBot;
	}

}