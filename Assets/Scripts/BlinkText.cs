using System.Collections;
using TMPro;
using UnityEngine;

/**
 * Mthf.PingPong: https://docs.unity3d.com/6000.3/Documentation/ScriptReference/Mathf.PingPong.html
 */

public class BlinkText : MonoBehaviour {
    [Header("Blink Settings")]
    public float speed = 1f;
    public bool isTemporary = false;

    private TextMeshProUGUI textMesh;

    public float minAlpha = 0.2f;
    public float maxAlpha = 1f;

    private bool initialized = false;

    void Start() {
        textMesh = GetComponent<TextMeshProUGUI>();
        
        if (isTemporary) {
            textMesh.alpha = 0f;
        }
    }

    /**
     * Makes the text blink by changing its alpha value over time.
     * If it's marked as temporary, it will deactivate itself after 2 seconds.
     */
    void Update() {
        if (textMesh == null) return;

        if (isTemporary && gameObject.activeSelf && !initialized) {
            initialized = true;
            StartCoroutine(DesactivateText());
        }

        if(!isTemporary || (isTemporary && initialized)) {
            float alpha = Mathf.PingPong(Time.time * speed, maxAlpha);
            textMesh.alpha = alpha + minAlpha;
        }
    }

    private IEnumerator DesactivateText() {
        yield return new WaitForSeconds(2f);
        gameObject.SetActive(false);
    }
}
