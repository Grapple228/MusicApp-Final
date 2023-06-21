namespace Music.Shared.Common;

public record PageResult<T>(
        long TotalPagesCount,
        long CurrentPageNumber,
        long TotalItemsCount,
        IReadOnlyCollection<T> ItemsOnPage
    ) where T : IModel;