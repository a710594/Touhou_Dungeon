using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class AudioSystem : MonoBehaviour
{
    private static readonly string _audioPath = "Audio/Music/";

    public static AudioSystem Instance = null;

    private static bool _exists;

    public AudioSource Source;

    private Dictionary<string, AudioClip> _clipDic = new Dictionary<string, AudioClip>();

    public void Play(string name, bool isTween = false)
    {
        if (!_clipDic.ContainsKey(name))
        {
            _clipDic.Add(name, Resources.Load<AudioClip>(_audioPath + name));
        }


        AudioClip clip;
        if (_clipDic.TryGetValue(name, out clip))
        {
            Source.clip = clip;
            Source.Play();
            if (isTween)
            {
                Source.volume = 0;
                Source.DOFade(1, 0.5f);
            }
            else
            {
                Source.volume = 1;
            }
        }
    }

    public void Stop(bool isTween = false)
    {
        if (isTween)
        {
            Source.DOFade(0, 0.5f);
        }
        else
        {
            Source.Stop();
        }
    }

    void Start()
    {
        if (!_exists)
        {
            _exists = true;
            DontDestroyOnLoad(transform.gameObject);//使物件切換場景時不消失
        }
        else
        {
            Destroy(gameObject); //破壞場景切換後重複產生的物件
            return;
        }
        Instance = this;
    }
}
