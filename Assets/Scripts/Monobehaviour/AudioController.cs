using System;
using UnityEngine;

public class AudioController : MonoBehaviour
{
    [SerializeField] private AudioSource ufoSfxSource;
    [SerializeField] private AudioSource spaceshipThrustSfxSource;
    private bool playSFX;
    [SerializeField] private string sfxToPlay;
    public string SfxToPlay
    {
        get => sfxToPlay;
        set => sfxToPlay = value;
    }
    [Serializable]struct ClipBox
    {
        public string clipName;
        public AudioClip clip;
        public float volume;
    }

    [SerializeField] private ClipBox[] gameClips;

    public void PlayAudioClip(string clipName)
    {
        for (int i = 0; i < gameClips.Length; i++)
        {
            if (gameClips[i].clipName.Equals(clipName))
            {
                AudioSource.PlayClipAtPoint(gameClips[i].clip, Vector3.zero, gameClips[i].volume);
            }
        }
    }

    private void Update()
    {
        if (GameManager.Instance.GameState == GameManager.GameStateEnum.GameOver)
            return;
        
        if (playSFX)
        {
            playSFX = false;
            PlayAudioClip(sfxToPlay);
        }

        if (GameManager.Instance.UfoInAction && ufoSfxSource.volume < 0.3f)
        {
            ufoSfxSource.volume += Time.deltaTime * 0.01f;
        }
        else if(!GameManager.Instance.UfoInAction && ufoSfxSource.volume > 0)
        {
            ufoSfxSource.volume -= Time.deltaTime;
        }
        
        //Spaceship thrust
        float vertical = Input.GetAxis("Vertical");
        vertical = Mathf.Clamp(vertical, 0, 0.5f);
        spaceshipThrustSfxSource.volume = vertical;

    }

    public void PlaySFX()
    {
        playSFX = true;
    }
}
