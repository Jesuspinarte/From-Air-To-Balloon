using UnityEngine;

public class LandArea : MonoBehaviour {
    private void OnCollisionEnter(Collision coll) {
        if (coll.transform.tag == "Player") {
            GameManager.Instance.OnPlayerWin();
        }
    }
}
