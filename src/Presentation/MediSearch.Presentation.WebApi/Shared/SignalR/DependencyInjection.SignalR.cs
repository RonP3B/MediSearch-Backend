using MediSearch.Core.Application.Shared.Ports;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.SignalR;

namespace Microsoft.Extensions.DependencyInjection;

internal static partial class DependencyInjection
{
    private static void AddSignalRServices(this IHostApplicationBuilder builder)
    {
        builder.Services.AddSingleton<IRealtimeActionPusher, SignalRRealtimeActionPusher>();

        builder.Services.AddSingleton<IUserIdProvider, SignalRUserIdProvider>();

        builder.Services.AddSignalR();

        builder.Services.PostConfigure<JwtBearerOptions>(
            JwtBearerDefaults.AuthenticationScheme,
            options =>
            {
                var existingHandler = options.Events.OnMessageReceived;

                options.Events.OnMessageReceived = async context =>
                {
                    if (existingHandler is not null)
                    {
                        await existingHandler(context);
                    }

                    if (!string.IsNullOrWhiteSpace(context.Token))
                    {
                        return;
                    }

                    string? accessToken = context.Request.Query["access_token"];

                    if (ShouldUseAccessToken(context, accessToken))
                    {
                        context.Token = accessToken;
                    }
                };
            }
        );
    }

    private static bool ShouldUseAccessToken(MessageReceivedContext context, string? accessToken)
    {
        return !string.IsNullOrWhiteSpace(accessToken)
            && context.HttpContext.Request.Path.StartsWithSegments(AppHub.Route);
    }
}
