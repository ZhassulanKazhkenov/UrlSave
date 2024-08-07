﻿using System.Net;
using System.Net.Mail;
using Hangfire;
using Microsoft.EntityFrameworkCore;
using UrlSave.Contexts;
using UrlSave.Entities;

namespace UrlSave.Jobs;

public class SendMailJob
{
    private readonly ILogger<SendMailJob> _logger;
    private readonly LinkContext _context;
    private readonly IConfiguration _configuration;

    public SendMailJob(ILogger<SendMailJob> logger, LinkContext context, IConfiguration configuration)
    {
        _logger = logger;
        _context = context;
        _configuration = configuration;
    }

    [JobDisplayName("SendMailJob")]
    public async Task Execute()
    {
        try
        {
            var notifications = await _context.Notifications
                .Where(n => !n.IsSend)
                .ToListAsync();
            _logger.LogInformation($"Notifications to Send: {notifications.Count}");

            foreach (var notification in notifications)
            {
                var isSend = await SendEmail(notification);
                if (isSend)
                {
                    notification.IsSend = true;
                    _context.Notifications.Update(notification);
                    await _context.SaveChangesAsync();
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error in SendMailJob: {ex.Message}");
        }
    }

    private async Task<bool> SendEmail(Notification notification)
    {
        var recipientEmail = notification.Recipient;
        try
        {
            _logger.LogInformation($"Try to send: {recipientEmail}");
            var smtpUser = _configuration["Email:SmtpUser"];
            var smtpPass = _configuration["Email:SmtpPass"];
            var senderEmail = _configuration["Email:SenderEmail"];
            using var smtpClient = new SmtpClient("in-v3.mailjet.com")
            {
                Port = 587,
                Credentials = new NetworkCredential(smtpUser, smtpPass),
                EnableSsl = true
            };

            var mailMessage = new MailMessage
            {
                From = new MailAddress(senderEmail),
                Subject = notification.Title,
                Body = notification.Body,
                IsBodyHtml = true,
            };

            mailMessage.To.Add(recipientEmail);

            await smtpClient.SendMailAsync(mailMessage);
            _logger.LogInformation($"Email is sent to {recipientEmail}: {notification.Title}");
            return true;
        }
        catch (SmtpException smtpEx)
        {
            _logger.LogError(smtpEx, $"SMTP failed: {smtpEx.Message}");
            return false;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Unexpected error when send email: {ex.Message}");
            return false;
        }
    }
}