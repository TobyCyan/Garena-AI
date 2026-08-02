using System.Threading.Tasks;

public interface ITextApiService
{
    Task<string> GetResponse(string prompt);
}
