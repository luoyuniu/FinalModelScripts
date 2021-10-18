using UnityEngine;
using UnityEngine.Audio;
using FrameWork;

#region 消息ID定义
public enum AudioEvent
{
    Began = MgrID.Audio,
    PlayOneShot,
    SetMasterVolume,
    SetSoundVolume,
    SetMusicVolume
}
#endregion

[RequireComponent(typeof(AudioSource))]
[MonoSingletonPath("[Audio]/AudioManager")]
public class AudioManager : MgrBase, ISingleton
{
    public AudioMixer audioMixer { get; private set; }
    public AudioSource audioPlayer { get; private set; }

    public override int ManagerId
    {
        get { return MgrID.Audio; }
    }

    public void OnSingletonInit()
    {
        RegisterEvents(
            AudioEvent.PlayOneShot,
            AudioEvent.SetMasterVolume,
            AudioEvent.SetSoundVolume,
            AudioEvent.SetMusicVolume
            );
        audioPlayer = GetComponent<AudioSource>();
    }

    public void PlayOneShot(AudioClip audioClip)
    {
        audioPlayer.PlayOneShot(audioClip);
    }

    public void SetMasterVolume(float volume)
    {
        audioMixer.SetFloat("MasterVolume", volume);
        // MasterVolume为我们暴露出来的Master的参数
    }

    public void SetSoundVolume(float volume)
    {
        audioMixer.SetFloat("SoundVolume", volume);
        // MusicVolume为我们暴露出来的Music的参数
    }

    public void SetMusicVolume(float volume)
    {
        audioMixer.SetFloat("MusicVolume", volume);
    }
}