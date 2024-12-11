namespace Application.DailyArticlesProcessor
{
    public interface IDailyArticlesProcessor
    {
        Task<IEnumerable<Domain.Article>> FetchArticlesAsync(DateOnly date);
    }
}
