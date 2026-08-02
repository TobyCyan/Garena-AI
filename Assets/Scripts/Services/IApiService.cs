using System.Threading.Tasks;

public interface IApiService
{
    Task<string> GetResponse(string input);
}
