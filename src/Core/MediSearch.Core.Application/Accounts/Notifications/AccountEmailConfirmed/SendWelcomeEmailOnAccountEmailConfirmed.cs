using MediSearch.Core.Application.Accounts.Constants;
using MediSearch.Core.Application.Accounts.DTOs;
using MediSearch.Core.Application.Accounts.Models;
using MediSearch.Core.Application.Accounts.Ports;

namespace MediSearch.Core.Application.Accounts.Notifications.AccountEmailConfirmed;

public sealed class SendWelcomeEmailOnAccountEmailConfirmed(
    IAccountQueryService accountQueryService,
    IAccountPasswordManager accountPasswordManager,
    ITemplateRenderingService templateRenderingService,
    IEmailService emailService
) : NotificationHandler<AccountEmailConfirmedNotification>
{
    private readonly IAccountQueryService _accountQueryService = accountQueryService;
    private readonly IAccountPasswordManager _accountPasswordManager = accountPasswordManager;
    private readonly ITemplateRenderingService _templateRenderingService = templateRenderingService;
    private readonly IEmailService _emailService = emailService;

    public override async Task Handle(
        AccountEmailConfirmedNotification notification,
        CancellationToken cancellationToken
    )
    {
        var user = await _accountQueryService.GetUserOnboardingDataAsync(
            notification.ExternalUserId,
            cancellationToken
        );

        Role primaryRole = ResolvePrimaryRole(user.RoleIds);

        string body = primaryRole switch
        {
            _ when primaryRole == Role.Client => await RenderClientAsync(
                _templateRenderingService,
                user,
                cancellationToken
            ),
            _ when primaryRole == Role.CompanyOwner => await RenderCompanyOwnerAsync(
                _templateRenderingService,
                user,
                cancellationToken
            ),
            _ => await RenderCompanyUserAsync(
                _templateRenderingService,
                _accountPasswordManager,
                user,
                notification.ExternalUserId,
                cancellationToken
            ),
        };

        await _emailService.SendAsync(
            new EmailMessage(
                user.Email,
                AccountEmailSubjectCodes.EmailConfirmedSubject,
                body,
                user.DisplayName
            ),
            notification.IdempotencyKey,
            cancellationToken
        );
    }

    private static async Task<string> RenderClientAsync(
        ITemplateRenderingService templateRenderingService,
        UserOnboardingDto user,
        CancellationToken cancellationToken
    )
    {
        var body = await templateRenderingService.RenderHtmlAsync(
            new ClientUserWelcomeModel(FullName: user.FullName),
            cancellationToken
        );

        return body;
    }

    private static async Task<string> RenderCompanyOwnerAsync(
        ITemplateRenderingService templateRenderingService,
        UserOnboardingDto user,
        CancellationToken cancellationToken
    )
    {
        if (string.IsNullOrWhiteSpace(user.CompanyName))
        {
            throw CorruptedInvariantException.CompanyUserWithoutCompany(user.Username);
        }

        var body = await templateRenderingService.RenderHtmlAsync(
            new CompanyOwnerWelcomeModel(FullName: user.FullName, CompanyName: user.CompanyName),
            cancellationToken
        );

        return body;
    }

    private static async Task<string> RenderCompanyUserAsync(
        ITemplateRenderingService templateRenderingService,
        IAccountPasswordManager accountPasswordManager,
        UserOnboardingDto user,
        string externalUserId,
        CancellationToken cancellationToken
    )
    {
        if (string.IsNullOrWhiteSpace(user.CompanyName))
        {
            throw CorruptedInvariantException.CompanyUserWithoutCompany(user.Username);
        }

        var tokenDto = await accountPasswordManager.GetPasswordResetTokenAsync(
            externalUserId,
            cancellationToken
        );

        var body = await templateRenderingService.RenderHtmlAsync(
            new CompanyUserWelcomeModel(
                FullName: user.FullName,
                CompanyName: user.CompanyName,
                Username: user.Username,
                ExternalUserId: externalUserId,
                PasswordResetToken: tokenDto.ResetToken
            ),
            cancellationToken
        );

        return body;
    }

    private static Role ResolvePrimaryRole(int[] roleIds)
    {
        int[] priority =
        {
            Role.CompanyOwner.Id,
            Role.CompanyManager.Id,
            Role.CompanyMember.Id,
            Role.Client.Id,
        };

        return Role.GetById(priority.First(roleIds.Contains));
    }
}
