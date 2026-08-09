using System.Threading.Tasks;

public interface IPipelineObject
{
    Task<BasePipelineData> DoPipeAsync(BasePipelineData input);
}
