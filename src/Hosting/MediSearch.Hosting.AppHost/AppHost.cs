using MediSearch.Shared.Constants;

var builder = DistributedApplication.CreateBuilder(args);

builder.AddAzureContainerAppEnvironment("aca-env");

var databaseServer = builder
    .AddAzurePostgresFlexibleServer(ServiceNames.DatabaseServer)
    .WithPasswordAuthentication()
    .RunAsContainer(container =>
        container
            .WithLifetime(ContainerLifetime.Persistent)
            .WithImageTag("18.3")
            .WithPgAdmin(pgAdmin => pgAdmin.WithImageTag("9.14.0"))
    )
    .AddDatabase(ServiceNames.Database);

var rabbitMq = builder
    .AddRabbitMQ(ServiceNames.RabbitMq)
    .WithManagementPlugin()
    .WithLifetime(ContainerLifetime.Persistent);

var redis = builder
    .AddAzureManagedRedis(ServiceNames.Redis)
    .RunAsContainer(container =>
        container.WithLifetime(ContainerLifetime.Persistent).WithRedisInsight()
    );

var mailpit = builder.AddMailPit(ServiceNames.MailPit);

var web = builder
    .AddProject<Projects.MediSearch_Presentation_WebApi>(ServiceNames.WebApi)
    .WithReference(databaseServer)
    .WithReference(rabbitMq)
    .WithReference(redis)
    .WithReference(mailpit)
    .WaitFor(databaseServer)
    .WaitFor(rabbitMq)
    .WaitFor(redis)
    .WaitFor(mailpit)
    .WithExternalHttpEndpoints()
    .WithAspNetCoreEnvironment()
    .WithUrlForEndpoint(
        "http",
        url =>
        {
            url.DisplayText = "Scalar API Reference";
            url.Url = "/scalar";
        }
    );

builder.Build().Run();
