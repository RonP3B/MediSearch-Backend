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

// Keycloak is the external identity provider: it stores the account credentials that
// used to live in the ASP.NET Core Identity tables. The realm, the confidential client
// and its service-account roles are created from Realms/medisearch-realm.json the first
// time the container starts with an empty data volume.
var keycloakAdminUsername = builder.AddParameter("keycloak-admin-username");
var keycloakAdminPassword = builder.AddParameter("keycloak-admin-password", secret: true);

var keycloak = builder
    .AddKeycloak(
        ServiceNames.Keycloak,
        port: 8080,
        adminUsername: keycloakAdminUsername,
        adminPassword: keycloakAdminPassword
    )
    .WithImageTag("26.7.3")
    .WithDataVolume()
    .WithRealmImport("./Realms")
    .WithLifetime(ContainerLifetime.Persistent);

var web = builder
    .AddProject<Projects.MediSearch_Presentation_WebApi>(ServiceNames.WebApi)
    .WithReference(databaseServer)
    .WithReference(rabbitMq)
    .WithReference(redis)
    .WithReference(mailpit)
    .WithReference(keycloak)
    .WaitFor(databaseServer)
    .WaitFor(rabbitMq)
    .WaitFor(redis)
    .WaitFor(mailpit)
    .WaitFor(keycloak)
    .WithEnvironment("Keycloak__Url", keycloak.GetEndpoint("http"))
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
