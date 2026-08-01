public class BasePipelineData
{
    public PipelineState State { get; set; }
}

public enum PipelineState
{
    OK,
    BAD,
}
