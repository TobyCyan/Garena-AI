using UnityEngine;

[RequireComponent(typeof(AudioSource))]
[RequireComponent(typeof(SoundEffectLibrary))]
public class SoundEffectManager : MonoBehaviour
{
    private static AudioSource audioSource;
    private static SoundEffectLibrary soundEffectLibrary;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        soundEffectLibrary = GetComponent<SoundEffectLibrary>();
    }

    public static void Play(string name)
    {
        AudioClip audioClip = soundEffectLibrary.GetRandomClip(name);
        if (audioClip != null)
        {
            audioSource.PlayOneShot(audioClip);
        }
    }
}
