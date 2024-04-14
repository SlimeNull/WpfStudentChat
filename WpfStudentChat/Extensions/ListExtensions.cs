namespace WpfStudentChat.Extensions;

public static class ListExtensions
{
    public static int FindIndex<TItem>(this IList<TItem> list, Predicate<TItem> predicate)
    {
        ArgumentNullException.ThrowIfNull(list, nameof(list));
        ArgumentNullException.ThrowIfNull(predicate, nameof(predicate));

        for (int i = 0; i < list.Count; i++)
        {
            if (predicate.Invoke(list[i]))
                return i;
        }

        return -1;
    }
}
