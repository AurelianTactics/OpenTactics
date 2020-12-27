using UnityEngine;
using System.Collections;

public class SoundManager : Singleton<SoundManager>
{
    [SerializeField]
    private AudioSource soundSource;
    [SerializeField]
    private AudioClip missSound;
    [SerializeField]
    private AudioClip hitSound;
    [SerializeField]
    private AudioClip uiSound;
    [SerializeField]
    private AudioClip deathSound;

    protected SoundManager()
    { // guarantee this will be always a singleton only - can't use the constructor!

    }

    //private int testSound = 0;

    public void PlaySoundClip(int type = 0)
    {
        //testSound += 1;
        //testSound = testSound % 4;
        //type = testSound;

        if (uiSound == null)
            return;

        if (type == 0)
        {
            soundSource.PlayOneShot(uiSound);
        }
        else if (type == 1)
        {
            soundSource.PlayOneShot(hitSound);
        }
        else if (type == 2)
        {
            soundSource.PlayOneShot(missSound);
        }
        else if (type == 3)
        {
            soundSource.PlayOneShot(deathSound);
        }
        else
        {
            soundSource.PlayOneShot(uiSound);
        }
    }
}
