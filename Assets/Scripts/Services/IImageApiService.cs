using System.Threading.Tasks;

public interface IImageApiService
{
    Task<byte[]> GetImage(string prompt);
}
