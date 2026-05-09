# Templating And Application Models

This document explains how MediSearch handles template rendering today, why the application layer owns the template data, and how the infrastructure side is shaped to scale beyond HTML later if needed.

## The Main Idea

In this codebase, the application layer owns:

- when content should be generated
- what data the content needs
- which use case or notification the content belongs to

Infrastructure owns:

- how that content is rendered
- how localization text is resolved inside templates
- how links and asset URLs are built
- how the final rendered body is delivered through email providers

That split is intentional.

## Why The Application Owns The Template Data

The application layer is where the use case lives, so it is also where the content contract should live.

That is why you will see records such as:

- `CompanyRenamedModel`
- `ProductCreatedModel`
- `CommentPostedModel`
- `ChatRoomStartedModel`
- `AccountEmailConfirmationRequestedModel`

These records are not infrastructure details. They are application-owned descriptions of the data a rendered piece of content needs.

This has a few benefits:

- handlers stay explicit about what they are sending
- infrastructure does not need to know how to query business data on its own
- the same model can be reused by different renderers in the future
- changing renderer technology does not force the content contract to move out of the application layer

## The `*Model` Naming Convention

The `Model` suffix is a convention with real meaning here.

When you see a record named `SomethingModel`, it usually means:

- it is the data contract for a rendered template
- the HTML template component is expected to be named `Something.razor`

Example:

- `CompanyRenamedModel` maps to `CompanyRenamed.razor`

The infrastructure HTML renderer resolves templates from the model type name by stripping the `Model` suffix.

## End-To-End Flow

The normal flow looks like this:

1. A notification handler or similar application use case decides that content must be sent.
2. The handler gathers the required business data, often from query services.
3. The handler creates a feature-specific `*Model` record in the application layer.
4. The handler calls `ITemplateRenderingService.RenderHtmlAsync(model)`.
5. Infrastructure resolves the matching Razor component and renders it to HTML.
6. The handler wraps that HTML in an `EmailMessage` together with a subject code.
7. `IEmailService` localizes the subject and sends the message through the active provider.

This keeps the orchestration in the application layer and the rendering mechanics in infrastructure.

## The Port Boundary

The application only depends on this port:

- `ITemplateRenderingService`

That is important because the application does not know:

- Blazor
- Razor component discovery
- HTML renderer lifecycle
- provider-specific email implementation details

It only knows that a model can be rendered into HTML.

## How Infrastructure Renders HTML Today

Today the infrastructure implementation is:

- `TemplateRenderingService`
  - the application-facing implementation of `ITemplateRenderingService`
- `IHtmlTemplateRenderer`
  - an internal infrastructure abstraction for HTML rendering
- `BlazorHtmlTemplateRenderer`
  - the current concrete HTML renderer

The actual rendering is done with Blazor's `HtmlRenderer`.

## Template Discovery Convention

`BlazorHtmlTemplateRenderer` builds an index of available templates at startup time and resolves them by convention.

A component is considered an HTML template when:

- it is a component type
- its namespace ends with `.Templates.Html`
- it has a `Model` property

Then the renderer maps:

- `SomethingModel` -> `Something`

and looks for a component with that exact name.

This means template resolution is convention-based, not manually registered per template.

It also means duplicate component names across HTML template namespaces are rejected, which is a good safety check.

## What Lives Inside The Templates

The Razor templates themselves usually contain:

- localized text via `ITextLocalizer`
- shared layout through `EmailLayout`
- links and URL construction via `AppUrlBuilder`
- feature-specific data through the typed `Model`

So a template is not just a string file. It is a typed component with localization and helper services available.

## Shared Layout And Feature Templates

`EmailLayout.razor` gives the emails:

- shared HTML shell
- common styling
- footer/disclaimer
- consistent look across features

Feature templates then fill in only the meaningful body content.

That keeps the rendering consistent while still letting each feature own its actual message.

## Localization Split: Body vs Subject

Template body text and email subject text are handled differently on purpose.

Body text:

- lives in Razor templates
- uses `ITextLocalizer`
- references infrastructure template text keys

Email subject:

- is passed as a subject code in `EmailMessage`
- is localized inside the email service before sending

So the application chooses the subject code, but infrastructure resolves the localized subject string.

This keeps the body rendering flow and the email transport flow nicely separated.

## URL And Asset Building

Templates do not hardcode frontend URLs or action links.

Instead they use `AppUrlBuilder`, which is fed by the `AppUrls` configuration section.

That covers things like:

- login URL
- email confirmation URL
- password reset URL
- email-confirmation result URL
- asset URLs

This is one of the reasons the rendering logic belongs in infrastructure: it needs environment-aware URL composition.

## Why This Is Built To Scale Beyond HTML

Right now the application port exposes `RenderHtmlAsync(...)`, and the infrastructure implementation delegates to an HTML renderer.

So yes, today's concrete implementation is HTML-focused.

But the shape already keeps the important parts separated:

- the application-owned data model
- the application-facing rendering port
- the format-specific renderer inside infrastructure

That means future expansion can happen without moving the content contract out of the application layer.

Possible future directions could include:

- plain-text email rendering
- PDF rendering
- SMS/text-oriented renderers
- push-notification body rendering

The important thing is that the data model can stay the same while infrastructure adds new rendering mechanisms.

## How To Add A New Template

The usual steps are:

1. Add or reuse the notification/use case that needs rendered content.
2. Create an application-layer record named `SomethingModel` with exactly the data the template needs.
3. In the handler, build that model from application/query data.
4. Add `Something.razor` under the correct `Templates/Html` feature folder.
5. Give the component a typed `Model` parameter.
6. Add any template text keys/constants and localization entries needed by the Razor component.
7. Render it through `ITemplateRenderingService.RenderHtmlAsync(...)`.
8. Send it through `IEmailService` with the proper localized subject code.

## What Not To Do

Avoid these patterns:

- querying business data directly from infrastructure templates
- pushing raw anonymous objects into the renderer
- building long HTML strings inside notification handlers
- treating the model record as if it belonged to the renderer instead of the use case

Those shortcuts make the rendering flow harder to understand and much harder to evolve.

## Rule Of Thumb

If the question is:

- "what data should this content carry?"

that belongs to the application layer.

If the question is:

- "how do we turn that data into HTML and send it?"

that belongs to infrastructure.
