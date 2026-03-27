using HRSystem.Core.Entities;
using HRSystem.Core.Enums;
using HRSystem.Core.Interfaces.Services;
using HRSystem.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace HRSystem.Infrastructure.Services;

public class NotificationService : INotificationService
{
    private readonly HRSystemDbContext _context;
    private readonly IEmailService _emailService;

    public NotificationService(HRSystemDbContext context, IEmailService emailService)
    {
        _context = context;
        _emailService = emailService;
    }

    public async Task SendToUserAsync(int userId, string title, string message, string type = "Info", int? relatedEntityId = null, string? relatedEntity = null)
    {
        if (!Enum.TryParse<NotificationType>(type, out var notificationType))
            notificationType = NotificationType.Info;

        var notification = new Notification
        {
            UserId = userId,
            Title = title,
            Message = message,
            Type = notificationType,
            RelatedEntityId = relatedEntityId,
            RelatedEntity = relatedEntity,
            IsRead = false,
            CreatedAt = DateTime.UtcNow
        };

        _context.Notifications.Add(notification);
        await _context.SaveChangesAsync();
    }

    public async Task SendEmailAsync(string toEmail, string subject, string body)
    {
        await _emailService.SendEmailAsync(toEmail, subject, body);
    }

    public async Task<List<NotificationDto>> GetUserNotificationsAsync(int userId)
    {
        return await _context.Notifications
            .Where(n => n.UserId == userId)
            .OrderByDescending(n => n.CreatedAt)
            .Take(50)
            .Select(n => new NotificationDto(n.Id, n.Title, n.Message, n.Type.ToString(), n.IsRead, n.CreatedAt))
            .ToListAsync();
    }

    public async Task MarkAsReadAsync(int notificationId)
    {
        var notification = await _context.Notifications.FindAsync(notificationId);
        if (notification != null && !notification.IsRead)
        {
            notification.IsRead = true;
            await _context.SaveChangesAsync();
        }
    }

    public async Task MarkAllAsReadAsync(int userId)
    {
        var unread = await _context.Notifications
            .Where(n => n.UserId == userId && !n.IsRead)
            .ToListAsync();

        foreach (var notification in unread)
        {
            notification.IsRead = true;
        }

        await _context.SaveChangesAsync();
    }
}
