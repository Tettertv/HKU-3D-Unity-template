using FMOD.Studio;
using FMODUnity;
using UnityEngine;

public class AudioManagerScript : MonoBehaviour
{
    public static AudioManagerScript Instance { get; private set; }

    private Bus masterBus;
    private Bus musicBus;
    private Bus sfxBus;
    private Bus uiBus;
    private Bus ambienceBus;

    private VCA masterVCA;
    private VCA musicVCA;
    private VCA sfxVCA;
    private VCA uiVCA;
    private VCA ambienceVCA;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    //Oneshots (LIKE THE GAME!!!)

    public void PlayOneShot(AudioEventSO audioEvent, Vector3 position)
    {
        if (audioEvent == null)
        {
            Debug.Log("No Audio Passed Though?");
            return;

        }

        if (!audioEvent.eventReference.IsNull)
        {
            FMODBankManager.Instance.EnsureBankLoaded(audioEvent.bankName);

            RuntimeManager.PlayOneShot(audioEvent.eventReference, position);
        }
    }

    //Loops
    public EventInstance CreateInstance(AudioEventSO audioEvent)
    {
        return RuntimeManager.CreateInstance(audioEvent.eventReference);
    }

    //Pausing
    public void PauseGameAudio(bool pauseGameplay = true, bool pauseMusic = false)
    {
        if (pauseGameplay)
        {
            sfxBus.setPaused(true);
            ambienceBus.setPaused(true);
        }

        if (pauseMusic)
        {
            musicBus.setPaused(true);
        }

        //Maybe add the possibility of pausing ui too? not sure if thats needed
    }

    public void ResumeGameAudio()
    {
        sfxBus.setPaused(false);
        musicBus.setPaused(false);
    }

    //Volume
    public void SetMasterVolume(float volume)
    {
        masterVCA.setVolume(volume);
    }

    public void SetMusicVolume(float volume)
    {
        musicVCA.setVolume(volume);
    }

    public void SetSFXVolume(float volume)
    {
        sfxVCA.setVolume(volume);
    }

    public void SetUIVolume(float volume)
    {
        uiVCA.setVolume(volume);
    }

    public void SetAmbienceVolume(float volume)
    {
        ambienceVCA.setVolume(volume);
    }

    //Other shit
    public void ConnectBusVCA()
    {
        masterBus = RuntimeManager.GetBus("bus:/");
        musicBus = RuntimeManager.GetBus("bus:/Music");
        sfxBus = RuntimeManager.GetBus("bus:/SFX");
        uiBus = RuntimeManager.GetBus("bus:/UI");
        ambienceBus = RuntimeManager.GetBus("Bus:/Ambience");

        masterVCA = RuntimeManager.GetVCA("vca:/Master");
        musicVCA = RuntimeManager.GetVCA("vca:/Music");
        sfxVCA = RuntimeManager.GetVCA("vca:/SFX");
        uiVCA = RuntimeManager.GetVCA("vca:/UI");
        ambienceVCA = RuntimeManager.GetVCA("vca:/Ambience");
    }
}
