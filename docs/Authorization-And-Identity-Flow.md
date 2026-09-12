# Authorization And Identity Flow

MediSearch handles authorization primarily in the application layer, not in Web API endpoint declarations.

## The Main Idea

HTTP endpoints are intentionally thin.

Their job is usually just:

- receive transport input
- map it into a command or query
- send it through MediatR

Authentication and authorization decisions are then enforced by application-level request metadata and MediatR behaviors.

## Authentication Stack

The current authentication flow is:

- ASP.NET Core JWT bearer authentication validates the access token
- `CurrentUser` reads the claims from `HttpContext.User`
- application code consumes `ICurrentUser`

The access and refresh tokens are issued by the application itself. Accounts and passwords, however, live in Keycloak rather than in the database: the application asks Keycloak to verify a password, then builds its own token from claims it reads out of PostgreSQL. See [Keycloak Identity Provider](Keycloak-Identity-Provider.md) for the whole picture.

Important current-user data includes:

- authenticated user id
- external identity user id
- company id when the user belongs to a company
- roles

## Where Authorization Is Declared

Commands and queries can be decorated with the application's custom `[Authorize]` attribute.

That attribute supports:

- `Roles`
- `Permission`

This is application metadata, not the ASP.NET Core MVC/endpoint auth attribute.

## Where Authorization Is Enforced

`AuthorizationBehavior` is the enforcement point.

For every MediatR request it:

1. reads any application `[Authorize]` attributes on the request type
2. rejects anonymous callers when authorization is required
3. checks required roles
4. checks required permissions through `IPermissionService`

If authorization fails, it throws:

- `UnauthorizedException` for missing authentication
- `ForbiddenAccessException` for insufficient access

## Why This Design Is Useful

This keeps access rules close to the use case rather than close to the HTTP transport.

Benefits:

- the same command/query stays protected even if the caller changes
- security rules are visible on the request type itself
- the endpoint layer stays simple
- authorization becomes testable as part of application behavior, not only HTTP routing behavior

## Roles vs Permissions

Roles are coarse-grained identity categories:

- `SystemAdmin`
- `CompanyOwner`
- `CompanyManager`
- `CompanyMember`
- `Client`

Permissions are finer-grained capabilities seeded in persistence and checked through `IPermissionService`.

Examples include:

- modify company
- add/remove company user
- get company dashboard
- add/modify/remove product
- create/update/delete product classifications and categories

In practice:

- some requests require only authentication
- some require a role
- some require a permission
- some additionally do resource-level ownership checks inside the handler

## Resource-Level Checks Still Exist

Application-level authorization attributes do not replace all handler checks.

Handlers still perform request-specific access validation such as:

- "does this product belong to the caller's company?"
- "is this company the same company as the authenticated company user?"
- "is the caller allowed to edit this specific comment?"

Those checks usually throw `ForbiddenAccessException` when the caller is authenticated but out of scope.

## Where Identity Data Lives

It helps to keep three stores apart:

- **Keycloak** owns the credential: username, email, whether the email is verified, and the password hash.
- **PostgreSQL** owns everything the business cares about: the domain user, its company, its roles and the permissions behind them.
- **The access token** carries a snapshot of the second one, plus `ExternalUserId`, which is the Keycloak account id.

Roles and permissions were never part of the identity store and still are not. Swapping ASP.NET Core Identity for Keycloak changed nothing in this section of the system.

## Identity and Company Representation

For shared-state features, `ICurrentUser` is often converted into an `Agent`.

That means:

- client users become user agents
- company users become company agents

This is part of business behavior, but it also matters for authorization-adjacent flows because it decides which identity a feature operates under. See [Business Context And Agent Model](Business-Context-And-Agent-Model.md).

## The SignalR Exception

SignalR is the main place where ASP.NET Core endpoint-level authorization is used directly.

The application-layer MediatR authorization model applies to commands and queries. The hub itself still uses ASP.NET Core auth because it is not a MediatR request.

## Practical Guidance

When adding a new secured use case:

1. Put the `[Authorize]` requirement on the command or query.
2. Use roles only when the rule is truly role-based.
3. Prefer permission checks for business capabilities.
4. Keep resource-specific scope checks inside the handler.
5. Throw `ForbiddenAccessException` for authenticated-but-not-allowed cases.

When adding a new endpoint:

- do not move the main authorization logic into the endpoint just because it is HTTP-facing
- keep the endpoint thin and let the request pipeline enforce the rule
