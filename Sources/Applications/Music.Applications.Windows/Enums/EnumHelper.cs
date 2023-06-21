namespace Music.Applications.Windows.Enums;

public static class EnumHelper
{
    public static T Next<T>(this T e) where T : struct
    {
        if (!typeof(T).IsEnum) throw new ArgumentException($"Argument {typeof(T).FullName} is not an Enum");

        var arr = (T[])Enum.GetValues(e.GetType());
        var j = Array.IndexOf(arr, e) + 1;
        return arr.Length == j ? arr[0] : arr[j];
    }

    public static T Previous<T>(this T e) where T : struct
    {
        if (!typeof(T).IsEnum) throw new ArgumentException($"Argument {typeof(T).FullName} is not an Enum");

        var arr = (T[])Enum.GetValues(e.GetType());
        var j = Array.IndexOf(arr, e) - 1;
        return j == -1 ? arr[^1] : arr[j];
    }
}