namespace Application.TheGuardianClient
{
    public interface ITheGuardianClient
    {
        Task<string> GetStringAsync(string url);
    }
}
