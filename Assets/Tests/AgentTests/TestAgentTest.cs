using NUnit.Framework;

public class TestAgentTest
{
    [Test]
    public void GetPromptResults_ValidPrompt_Success()
    {
        ITextApiService apiService = new MockApiService();
        TestAgent agent = new(apiService);
        
        Assert.NotNull(agent.GetPromptResults());
    }

    [Test]
    public void GetPromptResults_ThrowsError_ReturnsNull()
    {
        ITextApiService apiService = new MockFailingApiService();
        TestAgent agent = new(apiService);

        Assert.Null(agent.GetPromptResults());
    }
}
