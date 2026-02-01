using UnityEngine;

public class LoseBarrier : MonoBehaviour {
    private void OnTriggerEnter(Collider coll) {
        if (coll.CompareTag("Player")) {
            GameManager.Instance.OnPlayerLose();
        }
    }
}
