using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioTest : MonoBehaviour
{
    public float volume, pitch;
    public AudioClip[] clips;// Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Space))
            {
            AudioManager.instance.PlaySFX(clips[5],volume,pitch);
        }
    }
}
