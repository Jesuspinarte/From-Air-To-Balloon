using UnityEngine;

public class CameraMovement : MonoBehaviour {
    [SerializeField]
    private GameObject balloon = null;
    [SerializeField]
    private float offsetX = 15.0f;

    void Start() {

    }

    void Update() {
        if (GameManager.Instance.gameStarted == false) return;

        if (balloon != null) {
            Vector3 newPosition = transform.position;

            newPosition.x = balloon.transform.position.x + offsetX;
            transform.position = newPosition;
        }
    }
}
