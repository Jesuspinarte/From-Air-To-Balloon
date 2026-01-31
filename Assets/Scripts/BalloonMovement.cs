using UnityEngine;

public class BalloonMovement : MonoBehaviour {
    [SerializeField]
    [Tooltip("List of VFX Particle for Fire to control")]
    private ParticleSystem[] vfxList;

    private Rigidbody rb;

    [Header("Damping Constraints")]
    [Tooltip("Damping applied when the object is going up.")]
    public float upDamping = 6;
    [Tooltip("Damping applied when the object is falling down.")]
    public float downDamping = 2;

    [Header("Force Settings")]
    [Tooltip("Multiplier for upward force applied to the object based on audio input.")]
    public float upForceMultiplier = 10;
    [Tooltip("Constant horizontal force applied to the object.")]
    public float horizontalForce = 150f;

    [Header("Position Constraints")]
    [Tooltip("Minimum height limit for the balloon.")]
    public float bottomLimit = 2f;
    [Tooltip("Maximum height limit for the balloon.")]
    public float topLimit = 14f;

    void Start() {
        rb = GetComponent<Rigidbody>();

        foreach (ParticleSystem vfx in vfxList) {
            ParticleSystem.EmissionModule emission = vfx.emission;
            emission.enabled = false;
        }
    }

    private void FixedUpdate() {
        if (GameManager.Instance.gameStarted == false) return;

        OnAudioInput();

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
    private void OnAudioInput() {
        if (SoundInput.isGoingUp && transform.position.y < topLimit) {
            rb.linearDamping = upDamping;
            rb.AddForce(0, upForceMultiplier, 0, ForceMode.Impulse);
        } else {
            rb.linearDamping = downDamping;
        }

        // Enable fire VFX when going up
        foreach (ParticleSystem vfx in vfxList) {
            ParticleSystem.EmissionModule emission = vfx.emission;
            emission.enabled = SoundInput.isGoingUp;
        }
    }
}
