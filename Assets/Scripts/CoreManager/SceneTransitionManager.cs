using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

[DisallowMultipleComponent]
public sealed class SceneTransitionManager : MonoBehaviour
{
    private static int currentSceneIndex;

    public static async void LoadHomeScene()
    {
        currentSceneIndex = 0;
        await LoadSceneAsync(currentSceneIndex);
    }
    
    public static async void LoadNextScene()
    {
        currentSceneIndex++;
        await LoadSceneAsync(currentSceneIndex);
    }

    private static async Task LoadSceneAsync(int sceneIndex)
    {
        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneIndex);

        while (!operation.isDone)
        {
            await Task.Yield();
        }
    }
}
