using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SFxManager : MonoBehaviour
{

    public AudioSource Audio;
    public AudioClip Click;
    public static SFxManager sfxInstance;


    private void Awake()
    {
        if (sfxInstance != null && sfxInstance != this)
        {
            Destroy(this.gameObject);
            return;
        }

        sfxInstance = this;
        DontDestroyOnLoad(this);
    }
}
