# Business Context And Agent Model

This document explains the business identities that matter in MediSearch and why some features operate on behalf of the company instead of the individual user.

## Main Actors

The codebase currently revolves around:

- client users
- company users
  - company owner
  - company manager
  - company member
- companies
  - pharmacy
  - laboratory
- system administrator

The important modeling detail is that company users are still users, but in some features they act on behalf of their company instead of acting as themselves.

## User vs Agent

This is why the `Agent` value object exists.

An `Agent` is a polymorphic identity with:

- `AgentTypeId`
- `AgentId`

Today there are two agent types:

- `User`
- `Company`

This abstraction lets the application talk about "who is participating" without forcing every feature to care whether the participant is an individual client or a company acting through one of its users.

## How Current User Becomes an Agent

`ICurrentUser` represents the authenticated HTTP user.

Then application helpers convert that user into an `Agent`:

- if the authenticated user has no company, they become a `User` agent
- if the authenticated user belongs to a company, they become a `Company` agent

This is the key rule that makes company-shared behavior work.

In other words:

- company owner, manager, and member are different roles
- but for certain features they collapse into the same company agent

## Features That Use Agent Identity

The features that intentionally operate on agent identity include:

- chat rooms
- chat messages
- favorites
- some realtime push targeting

The result is shared behavior for company users:

- a company chat room belongs to the company, not to the specific employee who started it
- messages sent by company users are sent as the company agent
- favorites created by company users belong to the company agent
- realtime company pushes can fan out to the whole company group

This is why multiple company users can see the same company-level chat/favorite state.

## The Important Exception: Comments

Comments do not use agent identity.

Comments are authored by `UserId`, not by `Agent`.

That means:

- company users comment as themselves
- the UI can still show their company as extra context
- but the author identity is the individual user

This is the deliberate exception to the company-as-agent rule.

## Interaction Rules Enforced in the Current Code

The rules below are the ones that are explicitly encoded today in validators/repositories.

### Chat

Chat is allowed only when there is exactly one company participant and that company is a pharmacy.

That means:

- client <-> pharmacy: allowed
- laboratory <-> pharmacy: allowed
- client <-> laboratory: not allowed
- client <-> client: not allowed
- pharmacy <-> pharmacy: not allowed
- laboratory <-> laboratory: not allowed

This matches the idea that pharmacies are the bridge actor for direct chat interactions.

### Company Favorites

Client users:

- can favorite only pharmacy companies

Company users:

- favorite as their company agent
- can favorite only companies of the opposite company type
- cannot favorite their own company

### Product Favorites

Client users:

- can favorite only pharmacy products

Company users:

- favorite as their company agent
- can favorite only products owned by the opposite company type

In practice:

- pharmacies can favorite laboratory products

## Why This Modeling Choice Matters

Without `Agent`, several features would behave incorrectly for company users:

- every employee would create separate chats instead of sharing the company conversation
- company favorites would fragment per employee
- realtime company notifications would need ad hoc duplication logic everywhere

The `Agent` abstraction keeps those features aligned with the business model instead of the authentication model.

## Roles Still Matter

Even though company users collapse into the same company agent for shared-state features, their roles still matter for authorization.

Examples:

- who can modify company data
- who can add/remove company users
- who can manage products
- who can view the company dashboard

So:

- `Agent` answers "who is represented in this business interaction?"
- roles and permissions answer "what is this authenticated user allowed to do?"

## Practical Guidance

When adding a new feature, decide early whether it should be:

- user-scoped
- company-scoped through `Agent`

Good question to ask:

- if two employees from the same company perform this action, should they share the same state or should they create separate personal state?

If the answer is "shared company state," the feature probably belongs on `Agent`, not directly on `User`.
