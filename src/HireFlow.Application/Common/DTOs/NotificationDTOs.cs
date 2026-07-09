namespace HireFlow.Application.Common.DTOs;

public record NotificationDto(
    Guid Id,
    string Title,
    string Body,
    string? Type,
    bool IsRead,
    string? Payload,
    DateTime CreatedAt
);