namespace MediSearch.Core.Application.Shared.Exceptions;

public sealed class CorruptedInvariantException(string message) : Exception(message)
{
    public static CorruptedInvariantException MissingCompanyOwnerContact(Guid companyId) =>
        new(
            $"Company ownership invariant is corrupted."
                + $" Company '{companyId}' has no owner contact information."
        );

    public static CorruptedInvariantException CompanyUserWithoutCompany(string username) =>
        new(
            $"Company user invariant is corrupted."
                + $" User '{username}' has a company role but no associated company name."
        );

    public static CorruptedInvariantException InvalidChatRoomParticipantCount(
        Guid chatRoomId,
        int participantCount
    ) =>
        new(
            $"Chat room participant invariant is corrupted."
                + $" Chat room '{chatRoomId}' contains {participantCount} participants instead of exactly 2."
        );

    public static CorruptedInvariantException DuplicateChatRoomParticipants(Guid chatRoomId) =>
        new(
            $"Chat room participant invariant is corrupted."
                + $" Chat room '{chatRoomId}' does not contain two distinct participants."
        );
}
