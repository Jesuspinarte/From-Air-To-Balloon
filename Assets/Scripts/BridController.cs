using UnityEngine;

public class BridController : MonoBehaviour {
    public float horizontalForce = 50f;

    private Rigidbody rb;
    private bool isDead = false;

    void Start() {
        rb = GetComponent<Rigidbody>();
    }

    private void FixedUpdate() {
        if (isDead) return;

        // Fixed horizontal movement
        rb.linearVelocity = new Vector3(-horizontalForce * Time.fixedDeltaTime, 0, 0);

        if (transform.position.x < GameManager.Instance.gameCamera.transform.position.x - 30f) {
            Destroy(gameObject);
        }
    }

    private void OnCollisionEnter(Collision coll) {
        if (coll.gameObject.CompareTag("Player")) {
            isDead = true;

            GameManager.Instance.OnPlayerLose();
            SoundManager.Instance.PlaySfxSound(FxSoundType.Crash);

            coll.transform.GetComponent<BalloonMovement>().PlayerLoses();

            Destroy(gameObject);
        }
    }
}
