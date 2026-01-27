using UnityEngine;

/**
 * This script was adapted from the following source:
 * 
 * GIST: https://gist.github.com/RFullum/a1bee89fc289572e0d7bc5d2d533bde8
 * Youtube: https://www.youtube.com/watch?v=faQEKUvc7MY
 * 
 * The damp solution for Rigidbody was used from this source:
 * Youtube: https://www.youtube.com/watch?v=2M8uxUfi6jI
 * 
 * Color palette: https://lospec.com/palette-list/campfire-gb
 */

[RequireComponent(typeof(AudioSource))]
public class SoundInput : MonoBehaviour {
    [Header("Debug Info")]
    public float inputVolume = 0f; // Debug variable
    public static float[] samples = new float[RATE]; // Sample rate

    private Rigidbody rb;
    private AudioSource audioSource;
    private const int RATE = 128;

    [Header("Input Settings")]
    [Tooltip("Selected microphone device from available devices. Usually, the one your system select by default.")]
    public string selectedDevice;
    [Range(0.00001f, 0.001f)]
    [Tooltip("Minimum volume sensitivity to register audio input.")]
    public float sensitivity = 0.0001f;

    [Header("Transform Settings")]
    [Tooltip("Multiplier for upward force applied to the object based on audio input.")]
    public float upForceMultiplier = 2;

    public float downDamping = 2;
    public float upDamping = 10;

    // TODO: Option to select microphone from available devices
    // private string[] microphones;

    void Start() {
        rb = GetComponent<Rigidbody>();
        audioSource = GetComponent<AudioSource>();

        if (Microphone.devices.Length > 0) {
            selectedDevice = Microphone.devices[0]; // Select the first available microphone
            audioSource.clip = Microphone.Start(selectedDevice, true, 1, AudioSettings.outputSampleRate);
            audioSource.loop = true;

            while (!(Microphone.GetPosition(selectedDevice) > 0)) {
                audioSource.Play();
            }
        } else {
            Debug.LogWarning("No microphone devices found.");
        }
    }

    void Update() {
        getOutputData();
        onAudioInput();
    }

    /**
     * Applies upward force to the object based on audio input volume.
     * Changes the air resistance (linear damping), will be higher when going up and lower when falling down.
     * It won't be 0 to avoid an aggressive fall.
     */
    private void onAudioInput() {
        if (inputVolume > 0 && transform.position.y < 14f) {
            rb.linearDamping = upDamping;
            rb.AddForce(0, upForceMultiplier, 0, ForceMode.Impulse);
        } else {
            rb.linearDamping = downDamping;
        }
    }

    /**
     * Checks the audio output data from the microphone and calculates the average volume.
     */
    private void getOutputData() {
        float values = 0f;
        audioSource.GetOutputData(samples, 0);

        for (int i = 0; i < RATE; ++i) {
            values += Mathf.Abs(samples[i]);
        }

        values /= (float)RATE;

        if (values > sensitivity) inputVolume = values;
        else inputVolume = 0;
    }

}
