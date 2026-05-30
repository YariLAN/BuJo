using BuJo.Contracts.V1.Accounting;
using BuJo.Domain.Accounting;

namespace BuJo.Application.Accounting;

public static class UserMapper
{
    public static UserResponse ToResponse(this User user) => UserResponse.Create(
        user.Id,
        user.TelegramId,
        user.Name,
        user.ReminderMorningTime,
        user.ReminderEveningTime);
}