using MediSearch.Core.Application.Shared.Ports;
using MediSearch.Infrastructure.Communication.Email.APIs;
using MediSearch.Infrastructure.Communication.Email.Smtp;
using MediSearch.Shared.Constants;
using Microsoft.Extensions.Configuration;
using Resend;

namespace Microsoft.Extensions.DependencyInjection;

public static partial class DependencyInjection
{
    private const string EmailServiceSectionName = "EmailService";

    private static void AddEmailServices(this IHostApplicationBuilder builder)
    {
        if (builder.Environment.IsDevelopment())
        {
            string connectionString = Guard.Against.NullOrWhiteSpace(
                builder.Configuration.GetConnectionString(ServiceNames.MailPit),
                $"Connection string '{ServiceNames.MailPit}' is missing or empty."
            );

            // "Endpoint=smtp://localhost:xxx" to "smtp://localhost:xxx"
            var endpoint = connectionString.Replace("Endpoint=", "");

            var smtpUri = new Uri(endpoint);

            builder.Services.Configure<SmtpOptions>(opts =>
            {
                opts.Host = smtpUri.Host;
                opts.Port = smtpUri.Port;
            });

            builder.Services.AddScoped<IEmailService, SmtpEmailService>();
        }
        else
        {
            builder.Services.Configure<EmailServiceOptions>(options =>
                Guard.Against.Null(
                    builder.Configuration.GetSection(EmailServiceSectionName),
                    $"Configuration key '{EmailServiceSectionName}' is missing or empty."
                )
            );

            // Pick only one of these to register.
            builder.AddResendEmailService();
            //builder.AddSendGridEmailService();
        }
    }

    // Resend — free tier (3,000/month, 100/day, requires a domain), native bulk support via EmailBatchAsync
    private static void AddResendEmailService(this IHostApplicationBuilder builder)
    {
        builder.Services.AddHttpClient<ResendClient>();

        EmailServiceOptions emailOptions = Guard.Against.Null(
            builder.Configuration.GetSection(EmailServiceSectionName).Get<EmailServiceOptions>(),
            $"Configuration key '{EmailServiceSectionName}' is missing or empty."
        );

        builder.Services.Configure<ResendClientOptions>(o => o.ApiToken = emailOptions.ApiKey);

        builder.Services.AddTransient<IResend, ResendClient>();

        builder.Services.AddScoped<IEmailService, ResendEmailService>();
    }

    // SendGrid — industry standard for bulk email, requires paid plan for production volumes
    private static void AddSendGridEmailService(this IHostApplicationBuilder builder)
    {
        builder.Services.AddScoped<IEmailService, SendGridEmailService>();
    }
}
