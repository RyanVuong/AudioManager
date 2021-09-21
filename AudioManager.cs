using UnityEngine;
using System.Collections;

[System.Serializable]
public class Sound
{
    public string name;
    public AudioClip clip;

    [Range(0f, 3f)]
    public float volume = 0.7f;
    [Range(0.5f, 1.5f)]
    public float pitch = 1f;

    [Range(0f, 0.5f)]
    public float randomVolume = 0.1f;
    [Range(0f, 0.5f)]
    public float randomPitch = 0.1f;

    [Range(0f, 1f)]
    public float spatialBlend = 0f;

    public AudioSource source;

    public void SetSource(AudioSource _source)
    {
        source = _source;
        source.clip = clip;
    }
    public void Play()
    {
        source.volume = volume * (1 + Random.Range(-randomVolume / 2f, randomVolume / 2f));
        source.pitch = pitch * (1 + Random.Range(-randomPitch / 2f, randomPitch / 2f));
        source.spatialBlend = spatialBlend;
        source.rolloffMode = AudioRolloffMode.Linear;
        source.Play();
    }
}

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;

    [SerializeField]
    Sound[] sounds;

    void Awake()
    {
        if (instance != null)
        {
            Debug.LogError("more than one AudioManger in the scene.");
        }
        else
        {
            instance = this;
        }
    }
    void Start()
    {
        for (int i = 0; i < sounds.Length; i++)
        {
            GameObject _go = new GameObject("Sound_" + i + "_" + sounds[i].name);
            _go.transform.SetParent(this.transform);
            sounds[i].SetSource(_go.AddComponent<AudioSource>());
        }
    }

    public bool IsThisSoundPlaying(int newSound)
    //Check what sound is playing so we only stop it if the sound changes
    {
        Sound s = sounds[newSound];
        if (s.source.isPlaying)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public void PlayStep(string _name)
    {
        for (int i = 0; i < sounds.Length; i++)
        {
            if (sounds[i].name == _name && IsThisSoundPlaying(i) == false)
            {
                sounds[i].Play();
                return;
            }
        }
        // no sound with _name
        Debug.LogWarning("AudioManager: Sound not found in list " + _name);
    }

    public void PlaySoundAtLocation(string _name, Vector3 location)
    {
        for (int i = 0; i < sounds.Length; i++)
        {
            if (sounds[i].name == _name)
            {
                AudioSource.PlayClipAtPoint(sounds[i].source.clip, location);
                return;
            }
        }
        // no sound with _name
        Debug.LogWarning("AudioManager: Sound not found in list " + _name);
    }

    public void PlaySound(string _name)
    {
        for (int i = 0; i < sounds.Length; i++)
        {
            if (sounds[i].name == _name)
            {
                sounds[i].Play();
                return;
            }
        }
        // no sound with _name
        Debug.LogWarning("AudioManager: Sound not found in list " + _name);
    }

    public void StopSound(string _name)
    {
        for (int i = 0; i < sounds.Length; i++)
        {
            if (sounds[i].name == _name)
            {
                sounds[i].source.Stop();
                return;
            }
        }
        // no sound with _name
        Debug.LogWarning("AudioManager: Sound not found in list " + _name);
    }

    public void SetZero(string _name)
    {
        for (int i = 0; i < sounds.Length; i++)
        {
            if (sounds[i].name == _name)
            {
                sounds[i].source.volume = 0;
                return;
            }
        }
        // no sound with _name
        Debug.LogWarning("AudioManager: Sound not found in list " + _name);
    }

    public void PlayAfterDelay(string _name, float delay)
    {
        StartCoroutine(Wait(_name, delay));
        /*
        Invoke("PlaySound(_name)", delay);
        
        if(Time.time > delay)
        {
            for (int i = 0; i < sounds.Length; i++)
            {
                if (sounds[i].name == _name)
                {
                    sounds[i].Play();
                    return;
                }
            }
        }
        Debug.LogWarning("AudioManager: Sound not found in list " + _name);
        */
    }

    IEnumerator Wait(string _name, float delay)
    {
        yield return new WaitForSeconds(delay);
        PlaySound(_name);
    }

}
