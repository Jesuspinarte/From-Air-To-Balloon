using UnityEngine;

/**
 * This script was adapted from the following source:
 * 
 * GIST: https://gist.github.com/RFullum/a1bee89fc289572e0d7bc5d2d533bde8
 * Youtube: https://www.youtube.com/watch?v=faQEKUvc7MY
 * 
 * To understand rigidbody movement I watched:
 * YouTube: https://www.youtube.com/watch?v=tNtOcDryKv4
 * 
 * The damp solution for Rigidbody was used from this source:
 * Youtube: https://www.youtube.com/watch?v=2M8uxUfi6jI
 * 
 * Color palette: https://lospec.com/palette-list/campfire-gb
 * 
 * Tested with the following microphone: Microphone (F Series Stereo Track Usb Audio) boom microphone.
 * 
 * A mixer was added to the audio source to prevent the microphone audio from being played on the speakers.
 * 
 * FOG:
 * Window -> Rendering -> Lighting -> Environment -> Fog
 * 
 * ASSETS:
 * Air balloon taken from: https://assetstore.unity.com/packages/3d/vehicles/air/underpoly-free-hot-air-balloons-257931
 * Environment taken from: https://www.fab.com/listings/ea7f5167-a797-45fe-96d4-efb9f3aecc20
 * VFX taken from: https://assetstore.unity.com/packages/vfx/particles/fire-explosions/vfx-urp-fire-package-305098
 */

[RequireComponent(typeof(AudioSource))]
public class SoundInput : MonoBehaviour {
    public static bool isGoingUp = false;

    private AudioSource audioSource;
    private const int RATE = 128;
    private float[] samples = new float[RATE]; // Sample rate

    [Header("Debug Info")]
    public int _zeroCrossings = 0; // Debug variable
    public float _inputVolume = 0f; // Debug variable
    [Tooltip("Selected microphone device from available devices. Usually, the one your system select by default.")]
    public string selectedDevice;

    void Start() {
        audioSource = GetComponent<AudioSource>();

        if (Microphone.devices.Length > 0) {
            selectedDevice = Microphone.devices[0]; // Select the first available microphone
            audioSource.clip = Microphone.Start(selectedDevice, true, 10, 44100);
            audioSource.loop = true;

            while (!(Microphone.GetPosition(selectedDevice) > 0)) { }

            audioSource.Play();
        } else {
            Debug.LogWarning("No microphone devices found.");
        }
    }

    void Update() {
        if (!GameManager.Instance.gameStarted) return;
        if (GameManager.Instance.gameFinished) return;

        getAirInput();
    }

    /**
     * Analyzes the audio input to determine volume and zero crossings.
     * Sets _inputVolume and _zeroCrossings based on sensitivity thresholds.
     */
    private void getAirInput() {
        audioSource.GetOutputData(samples, 0);

        float volume = 0f;
        int zeroCrossings = 0;

        for (int i = 0; i < RATE - 1; ++i) {
            float currentPos = samples[i];
            float nextPos = samples[i + 1];

            volume += Mathf.Abs(samples[i]);

            if ((currentPos > 0 && nextPos <= 0) || (currentPos < 0 && nextPos > 0)) {
                ++zeroCrossings;
            }
        }

        float averageVolume = volume / (float)RATE;

        if (averageVolume > GameManager.Instance.microphoneSensitivity && zeroCrossings > GameManager.Instance.blowSensitivity) {
            _inputVolume = averageVolume;
            _zeroCrossings = zeroCrossings;
            isGoingUp = true;
        } else {
            _inputVolume = 0f;
            _zeroCrossings = 0;
            isGoingUp = false;
        }
    }
}
