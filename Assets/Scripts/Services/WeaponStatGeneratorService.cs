public class WeaponStatGeneratorService
{
    public static string GenerateWeaponStats(string prompt)
    {
        string weaponResult = ChatApiService.Instance.GetResponse(prompt)
                    .GetAwaiter()
                    .GetResult();
        return weaponResult;
    }
}
