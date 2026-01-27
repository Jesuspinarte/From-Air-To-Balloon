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
 */

[RequireComponent(typeof(AudioSource))]
public class SoundInput : MonoBehaviour {
    private Rigidbody rb;
    private AudioSource audioSource;
    private const int RATE = 128;
    private float[] samples = new float[RATE]; // Sample rate

    [Header("Debug Info")]
    public int _zeroCrossings = 0; // Debug variable
    public float _inputVolume = 0f; // Debug variable
    [Tooltip("Selected microphone device from available devices. Usually, the one your system select by default.")]
    public string selectedDevice;

    [Header("Input Settings")]
    [Tooltip("Number of zero crossings to consider a valid blow.")]
    public int blowSensitivity = 9;
    [Range(0.01f, 0.1f)]
    [Tooltip("Minimum volume sensitivity to register audio input.")]
    public float microphoneSensitivity = 0.03f;

    [Header("Force Settings")]
    [Tooltip("Damping applied when the object is going up.")]
    public float upDamping = 2;
    [Tooltip("Damping applied when the object is falling down.")]
    public float downDamping = 2;
    [Tooltip("Multiplier for upward force applied to the object based on audio input.")]
    public float upForceMultiplier = 10;
    [Tooltip("Constant horizontal force applied to the object.")]
    public float horizontalForce = 150f;
    [Tooltip("Maximum height limit for the balloon.")]
    public float topLimit = 14f;
    [Tooltip("Minimum height limit for the balloon.")]
    public float bottomLimit = 2f;

    void Start() {
        rb = GetComponent<Rigidbody>();
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
        getAirInput();
    }

    private void FixedUpdate() {
        onAudioInput();
        // Fixed horizontal movement
        if (transform.position.y > bottomLimit) {
            rb.linearVelocity = new Vector3(horizontalForce * Time.fixedDeltaTime, rb.linearVelocity.y, 0);
        }
    }

    /**
     * Applies upward force to the object based on audio input volume.
     * Changes the air resistance (linear damping), will be higher when going up and lower when falling down.
     * It won't be 0 to avoid an aggressive fall.
     */
    private void onAudioInput() {
        if (_inputVolume > 0 && transform.position.y < topLimit) {
            rb.linearDamping = upDamping;
            rb.AddForce(0, upForceMultiplier, 0, ForceMode.Impulse);
        } else {
            rb.linearDamping = downDamping;
        }
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

        if (averageVolume > microphoneSensitivity && zeroCrossings > blowSensitivity) {
            _inputVolume = averageVolume;
            _zeroCrossings = zeroCrossings;
        } else {
            _inputVolume = 0f;
            _zeroCrossings = 0;
        }
    }
}
