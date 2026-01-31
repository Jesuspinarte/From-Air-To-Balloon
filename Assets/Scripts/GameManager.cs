using UnityEngine;

public class GameManager : MonoBehaviour {
    private static GameManager _instance;

    [Header("Game Settings")]
    public bool gameStarted = false;
    public bool gameFinished = false;

    [Header("References")]
    public Transform gameCamera;

    /**
     * Setting the values here to be called from different managers and being used
     * accross the game. In this case we are only using it in the SoundInput script but
     * this help us to make a separation of concerns and use set these values in the MenuController script.
     */
    [Header("Microphone Input Settings")]
    [Tooltip("Number of zero crossings to consider a valid blow.")]
    public int blowSensitivity = 15;
    [Range(0.0001f, 0.1f)]
    [Tooltip("Minimum volume sensitivity to register audio input.")]
    public float microphoneSensitivity = 0.03f;

    public static GameManager Instance {
        get {
            if (_instance == null) {
                Debug.Log("Creating new GameManager instance.");
                GameObject go = new GameObject("GameManager");
                _instance = go.AddComponent<GameManager>();
            }
            return _instance;
        }
    }

    private void Awake() {
        _instance = this;
    }

    public void OnPlayerWin() {
        Debug.Log("Player Wins!");
    }

    public void OnPlayerLose() {
        Debug.Log("Player Loses!");
    }
}
