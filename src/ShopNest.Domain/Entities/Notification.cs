using System;
using ShopNest.Domain.Entities.Common;
using ShopNest.Domain.Enums;

namespace ShopNest.Domain.Entities;

public class Notification : BaseEntity
{
	public Guid UserId { get; set; }

	public string Title { get; set; } = string.Empty;

	public string Message { get; set; } = string.Empty;

	public NotificationType Type { get; set; }

	public bool IsRead { get; set; } = false;

	public DateTime? ReadAt { get; set; }

	public string? Data { get; set; }

	public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

	public void MarkRead()
	{
		if (!IsRead)
		{
			IsRead = true;
			ReadAt = DateTime.UtcNow;
		}
	}

	public static Notification Create(Guid userId, string title, string message, NotificationType type, string? data = null)
	{
		return new Notification
		{
			UserId = userId,
			Title = title,
			Message = message,
			Type = type,
			Data = data
		};
	}
}
