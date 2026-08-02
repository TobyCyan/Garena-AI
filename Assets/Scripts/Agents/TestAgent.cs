using UnityEngine;

public class TestAgent : BaseAgent
{
    private const string PROMPT = "Test prompt, simply return an okay";

    public TestAgent(IApiService service) : base(service) {}

    public override string GetPromptResults()
    {
        return Prompt(PROMPT).Result;
    }
}
