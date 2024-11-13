namespace Event_system.Auth.Model;

public class EventRoles
{
    public const string Admin = nameof(Admin);
    public const string EventUser = nameof(EventUser);

    public static readonly IReadOnlyCollection<string> All = new[] { Admin, EventUser };
}