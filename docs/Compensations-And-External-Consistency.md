# Compensations And External Consistency

Database transactions do not automatically protect external side effects such as file storage or external identity users. MediSearch uses compensations to close that gap.

The external identity provider is Keycloak, so "creating an external identity user" below means an HTTP call that creates an account in a Keycloak realm, and the matching compensation deletes it again. See [Keycloak Identity Provider](Keycloak-Identity-Provider.md).

## The Problem This Solves

Consider a command that does all of this:

- saves one or more files
- creates an external identity user
- creates or updates domain entities in PostgreSQL

If the database write fails after the external work already succeeded, the system would be left partially applied unless we undo the external side effects.

Compensations are the project's answer to that problem.

## The Core Idea

Commands can execute external side effects through `ICompensationManager`.

For each successful external operation, the manager records how to undo it later.

Then:

- if the command and database save succeed, the compensation stack is cleared
- if anything fails before commit completes, the manager rolls the stack back in reverse order

This gives us an application-level approximation of "all-or-nothing" across the database and external systems.

## How the Manager Is Used

The normal pattern inside a command handler is:

1. Wrap the external operation in an `ICompensableOperation<T>`.
2. Execute it through `ICompensationManager.ExecuteAsync(...)`.
3. Use the returned result in the rest of the workflow.
4. Let `UnitOfWorkBehavior` decide whether to commit or roll back.

The manager stores successful operations in LIFO order, so rollback happens in reverse order of execution.

## Where Commit and Rollback Happen

The compensation manager is not committed manually by handlers.

`UnitOfWorkBehavior` owns the lifecycle:

- commands go through the handler
- `IUnitOfWork.SaveChangesAsync(...)` runs
- if save succeeds, `Commit()` clears the pending compensations
- if anything throws, `RollbackAsync()` runs before the exception is rethrown

This design keeps transaction orchestration in one place instead of scattering it across handlers.

## Why Rollback Publishes Compensation Events

Rollback does not usually call the external service directly.

Instead, compensable operations publish compensation events such as:

- asset deletion
- assets deletion
- external user deletion

Why this is useful:

- rollback work stays consistent with the existing messaging architecture
- compensation handlers get retries and operational visibility through the message bus
- the command path does not need to know every cleanup implementation detail
- cleanup can survive request-scope failure conditions

For transport/outbox details, see [MassTransit Messaging Architecture](MassTransit-Messaging-Architecture.md). This document focuses only on the application concept.

## Two Compensation Patterns in This Codebase

There are really two related patterns in MediSearch.

### 1. Pre-commit external side effects

This is the classic compensation-manager case.

Examples:

- save uploaded files before the aggregate is persisted
- register an external identity user before the domain user is persisted

If the command later fails, rollback publishes the matching compensation events.

### 2. Post-commit cleanup of replaced/removed resources

Some cleanups are not "undo my failed transaction" cases. They are "the transaction succeeded, now remove the obsolete external resource" cases.

Examples:

- old product image keys removed after a successful product image replacement
- old company/user images removed after a successful image change
- external user/image cleanup after successful domain deletions

Those are usually triggered from domain event handlers after the domain state change is accepted.

The common theme is the same: external state changes are modeled explicitly instead of being hidden in ad hoc side effects.

## Idempotency Requirement

Compensation handlers must be naturally idempotent.

In practice this means:

- deleting an already deleted file should be treated as success
- deleting an already deleted external user should be treated as success

If a compensation event is retried, the handler should converge on the desired end state instead of failing because the cleanup already happened.

This is essential because compensations run through asynchronous infrastructure.

## When to Use Compensations

Use a compensable operation when:

- the command must call an external system before the database commit is guaranteed
- a later failure would leave externally visible garbage behind

Do not use compensations when:

- the side effect should happen only after the database transaction succeeds and can simply be triggered from a domain event handler
- the work is purely in-database and already covered by the normal unit of work

## Current Examples in the Codebase

Typical uses today include:

- file upload rollback for user, company, chat, and product media
- external user rollback during user/company-user/company-owner registration
- cleanup of replaced images after successful updates
- cleanup of deleted external artifacts after successful domain removals

## Practical Guidance

When adding a new external dependency to a command:

1. Ask whether it happens before or after DB commit.
2. If it must happen before commit, wrap it in an `ICompensableOperation<T>`.
3. Define the matching compensation event and handler.
4. Make the compensation handler idempotent.
5. Let `UnitOfWorkBehavior` own the commit/rollback boundary.
