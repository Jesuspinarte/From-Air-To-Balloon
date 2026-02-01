using TMPro;
using UnityEngine;
using UnityEngine.UI;

using UnityEngine.InputSystem;
using System.Collections;
using UnityEngine.SceneManagement;

/**
 * Font used: https://fonts.google.com/specimen/Knewave?preview.text=from%20Air%20to%20Balloon&categoryFilters=Feeling:%2FExpressive%2FPlayful
 */

public class MenuController : MonoBehaviour {
    private const int RATE = 128;

    [Header("Menu Settings")]
    public GameObject startText;
    public GameObject titleText;
    public GameObject restartText;
    public GameObject mainMenuText;
    public GameObject optionsPanel;

    [Header("OptionsSettings")]
    public GameObject returnText;
    public GameObject continueText;

    [Header("Audio Settings")]
    public Slider micSensitivityValue;
    public Slider blowSensitivityValue;
    public TextMeshProUGUI micTextValue;
    public TextMeshProUGUI blowTextValue;

    [Header("Camear Settings")]
    public Transform menuCameraTransform;
    public Transform gameCameraTransform;
    public float zoomSpeed = 2f;

    private bool isMenuActive = true;
    private bool isOptionsActive = false;
    private bool isPauseActive = false;

    /**
     * Flag to avoid OnValueChange events being triggered during setup.
     * This because the Singleton was overriding the values set in the sliders on the UI.
     */
    private bool setupDone = false;

    void Start() {
        micSensitivityValue.minValue = 0.001f;
        micSensitivityValue.maxValue = 0.1f;
        micSensitivityValue.value = GameManager.Instance.microphoneSensitivity;
        micSensitivityValue.wholeNumbers = false;
        micTextValue.text = micSensitivityValue.value.ToString("F3");

        blowSensitivityValue.minValue = 1;
        blowSensitivityValue.maxValue = RATE;
        blowSensitivityValue.value = GameManager.Instance.blowSensitivity;
        blowSensitivityValue.wholeNumbers = true;
        blowTextValue.text = ((int)blowSensitivityValue.value).ToString();

        GameManager.Instance.gameCamera.position = menuCameraTransform.position;
        GameManager.Instance.gameCamera.rotation = menuCameraTransform.rotation;

        GameManager.Instance.gameStarted = false;
        setupDone = true;
    }

    void Update() {
        if (isMenuActive && !isOptionsActive) {
            if (Keyboard.current.spaceKey.wasPressedThisFrame) {
                SoundManager.Instance.PlaySfxSound(FxSoundType.Click);
                StartGame();
            }

            if (Keyboard.current.oKey.wasPressedThisFrame) {
                SoundManager.Instance.PlaySfxSound(FxSoundType.Click);

                isOptionsActive = true;
                returnText.SetActive(true);
                optionsPanel.SetActive(true);
                continueText.SetActive(false);
                mainMenuText.SetActive(false);
            }
        } else if (isOptionsActive) {
            if (Keyboard.current.spaceKey.wasPressedThisFrame) {
                SoundManager.Instance.PlaySfxSound(FxSoundType.Click);

                isOptionsActive = false;
                returnText.SetActive(false);
                optionsPanel.SetActive(false);
                mainMenuText.SetActive(true);
            }
        } else if (isPauseActive) {
            if (Keyboard.current.oKey.wasPressedThisFrame) {
                SoundManager.Instance.PlaySfxSound(FxSoundType.Click);

                isPauseActive = false;
                restartText.SetActive(true);
                optionsPanel.SetActive(false);
                Time.timeScale = 1f;
            }
        } else {
            if (Keyboard.current.oKey.wasPressedThisFrame) {
                SoundManager.Instance.PlaySfxSound(FxSoundType.Click);

                isPauseActive = true;
                returnText.SetActive(false);
                continueText.SetActive(true);
                optionsPanel.SetActive(true);
                restartText.SetActive(false);
                Time.timeScale = 0f;
            }
        }

        if (Keyboard.current.rKey.wasPressedThisFrame) {
            SoundManager.Instance.PlaySfxSound(FxSoundType.Click);
            RestartGame();
        }
    }

    /**
     * Smoothly zooms the camera from menu position to game position when the game starts.
     */
    private void FixedUpdate() {
        Transform gameCamera = GameManager.Instance.gameCamera;

        if (!isMenuActive && !GameManager.Instance.gameStarted) {
            gameCamera.position = Vector3.Lerp(gameCamera.position, gameCameraTransform.position, Time.deltaTime * zoomSpeed);
            gameCamera.rotation = Quaternion.Lerp(gameCamera.rotation, gameCameraTransform.rotation, Time.deltaTime * zoomSpeed);
        }
    }

    private void StartGame() {
        isMenuActive = false;

        mainMenuText.SetActive(false);
        optionsPanel.SetActive(false);
        titleText.SetActive(false);
        restartText.SetActive(true);

        StartCoroutine(UnlockPlayer());
    }

    private void RestartGame() {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void OnMicSensitivityChange() {
        if (!setupDone) return;

        GameManager.Instance.microphoneSensitivity = micSensitivityValue.value;
        micTextValue.text = micSensitivityValue.value.ToString("F3"); // Format to 3 decimal places
    }

    public void OnBlowSensitivityChange() {
        if (!setupDone) return;

        GameManager.Instance.blowSensitivity = (int)blowSensitivityValue.value;
        blowTextValue.text = ((int)blowSensitivityValue.value).ToString();
    }

    /**
     * Waits for 1 second before unlocking the player controls to avoid accidental input.
     * Otherwise, the camera zoom will have undesired movements with the player movement because
     * the camera is moving with the balloon.
     * It also starts the bird spawning process.
     */
    private IEnumerator UnlockPlayer() {
        yield return new WaitForSeconds(1.5f);
        GameManager.Instance.gameStarted = true;
        BirdGenerator.Instance.StartSpawning();

        if (startText != null) startText.SetActive(true);
    }
}
