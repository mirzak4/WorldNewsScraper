namespace Application.TheGuardianClient
{
    public interface IWebClient
    {
        Task<string> GetStringAsync(string url);
    }
}
