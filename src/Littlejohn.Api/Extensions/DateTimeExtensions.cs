namespace Littlejohn.Api.Extensions;

public static class DateTimeExtensions
{
    public static DateOnly ToDateOnly(this DateTime self) => DateOnly.FromDateTime(self);
}
