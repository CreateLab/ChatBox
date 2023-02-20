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
	public string Message { get; set; }

	/// <summary>
	/// Имя пользователя
	/// </summary>
	public string Username { get; set; }

	/// <summary>
	/// Обновляет модель
	/// </summary>
	/// <param name="model">Модель</param>
	public void Update(MessageModel model)
	{
		Message = model.Message;
		Username = model.Username;
	}
}