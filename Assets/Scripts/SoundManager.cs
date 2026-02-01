using System.Collections;
using UnityEngine;

/**
 * Background music taken from: https://assetstore.unity.com/packages/audio/music/the-ancient-forest-of-the-fairies-210914
 * Ambience sounds taken from: https://assetstore.unity.com/packages/audio/ambient/nature/sounds-of-australia-natural-ambiences-239337
 * 
 * Land sound taken from: https://pixabay.com/sound-effects/film-special-effects-land2-43790/
 * Smoke sound taken from: https://pixabay.com/sound-effects/nature-fire-crackling-sounds-427410/
 * Fire sound taken from: https://pixabay.com/sound-effects/film-special-effects-084303-hq-flamethrower-87072/
 * Bird sound taken from: https://pixabay.com/sound-effects/nature-macaw-sound-382721/
 * Click sound taken from: https://pixabay.com/sound-effects/film-special-effects-drumsticks-pro-mark-la-special-2bn-hickory-no4-103712/
 */

public enum FxSoundType {
    Click,
    Crash,
    Landing
}

public enum MovementSoundType {
    Fire,
    Smoke
}

public class SoundManager : MonoBehaviour {
    private static SoundManager _instance;

    [Header("Audio Sources")]
    public AudioSource sfxSource;
    public AudioSource musicSource;
    public AudioSource ambienceSource;
    public AudioSource movementSource;

    [Header("Audio Clips")]
    public AudioClip fireSound;
    public AudioClip clickSound;
    public AudioClip crashSound;
    public AudioClip smokeSound;
    public AudioClip landingSound;
    public AudioClip bacgroundMusic;
    public AudioClip ambienceSounds;

    [Header("Movement Sound Settings")]
    public float movementDelay = 0.5f;
    private Coroutine stopMovementCoroutine;

    public static SoundManager Instance {
        get {
            if (_instance == null) {
                GameObject go = new GameObject("SoundManager");
                _instance = go.AddComponent<SoundManager>();
            }
            return _instance;
        }
    }

    private void Awake() {
        _instance = this;
    }

    private void Start() {
        if (bacgroundMusic != null) {
            musicSource.clip = bacgroundMusic;
            musicSource.loop = true;
            musicSource.Play();
        }

        if (ambienceSource != null) {
            ambienceSource.clip = ambienceSounds;
            ambienceSource.loop = true;
            ambienceSource.Play();
        }

        if (fireSound != null) {
            movementSource.clip = fireSound;
            movementSource.loop = true;
        }
    }

    /**
     * Plays the specified sound effect.
     */
    public void PlaySfxSound(FxSoundType sfx) {
        if (sfxSource == null) return;

        switch (sfx) {
            case FxSoundType.Click:
                sfxSource.PlayOneShot(clickSound);
                break;
            case FxSoundType.Crash:
                sfxSource.PlayOneShot(crashSound);
                break;
            case FxSoundType.Landing:
                sfxSource.PlayOneShot(landingSound);
                break;
            default:
                return;
        }
    }

    /**
     * Plays the specified movement sound.
     */
    public void PlayMovementSound(MovementSoundType movement) {
        if (movementSource == null) return;

        switch (movement) {
            case MovementSoundType.Fire:
                if (!movementSource.isPlaying) {
                    stopMovementCoroutine = null;

                    movementSource.clip = fireSound;
                    movementSource.Play();
                }
                break;
            case MovementSoundType.Smoke:
                stopMovementCoroutine = null;

                movementSource.clip = smokeSound;
                movementSource.Play();
                break;
            default:
                return;
        }
    }

    /**
     * Stops the movement sound.
     */
    public void StopMovementSound() {
        if (movementSource == null) return;
        if (!movementSource.isPlaying) return;
        if (stopMovementCoroutine != null) return;

        stopMovementCoroutine = StartCoroutine(StopMovementSoundAfterDelay());
    }

    private IEnumerator StopMovementSoundAfterDelay() {
        yield return new WaitForSeconds(movementDelay);

        movementSource.Stop();
        stopMovementCoroutine = null;
    }
}
