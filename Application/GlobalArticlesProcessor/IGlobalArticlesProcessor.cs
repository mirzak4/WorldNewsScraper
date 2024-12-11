namespace Application.GlobalArticlesProcessor
{
    public interface IGlobalArticlesProcessor
    {
        Task StartProcessingAsync(DateOnly startDate, DateOnly endDate);
    }
}
