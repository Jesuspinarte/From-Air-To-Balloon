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
 */

[RequireComponent(typeof(AudioSource))]
public class SoundInput : MonoBehaviour {
    private Rigidbody rb;
    private AudioSource audioSource;
    private const int RATE = 128;
    private float[] samples = new float[RATE]; // Sample rate

    [Header("Debug Info")]
    public float inputVolume = 0f; // Debug variable
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

    void Start() {
        rb = GetComponent<Rigidbody>();
        audioSource = GetComponent<AudioSource>();

        if (Microphone.devices.Length > 0) {
            selectedDevice = Microphone.devices[0]; // Select the first available microphone
            //audioSource.clip = Microphone.Start(selectedDevice, true, 1, AudioSettings.outputSampleRate);
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
        // Fixed horizontal movement
        onAudioInput();
        if (transform.position.y > 2) {
            rb.linearVelocity = new Vector3(horizontalForce * Time.fixedDeltaTime, rb.linearVelocity.y, 0);
        }
    }

    /**
     * Applies upward force to the object based on audio input volume.
     * Changes the air resistance (linear damping), will be higher when going up and lower when falling down.
     * It won't be 0 to avoid an aggressive fall.
     */
    private void onAudioInput() {
        if (inputVolume > 0 && transform.position.y < 14f) { // Top limit
            rb.linearDamping = upDamping;
            rb.AddForce(0, upForceMultiplier, 0, ForceMode.Impulse);
        } else {
            rb.linearDamping = downDamping;
        }
    }

    private void getAirInput() {
        float volume = 0f;
        int ceroCrossings = 0;
        audioSource.GetOutputData(samples, 0);

        for (int i = 0; i < RATE - 1; ++i) {
            volume += Mathf.Abs(samples[i]);
            if ((samples[i] > 0 && samples[i + 1] <= 0) || (samples[i] < 0 && samples[i + 1] > 0)) {
                ++ceroCrossings;
            }
        }

        float averageVolume = volume / (float)RATE;

        if (averageVolume > microphoneSensitivity && ceroCrossings > blowSensitivity) {
            inputVolume = averageVolume;
        } else {
            inputVolume = 0f;
        }
    }

}
