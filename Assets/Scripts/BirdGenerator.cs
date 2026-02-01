using System.Collections;
using UnityEngine;

public class BirdGenerator : MonoBehaviour {
    private static BirdGenerator _instance;

    [Header("Bird Settings")]
    public GameObject birdPrefab;
    public float spawnInterval = 3f;

    [Header("Spawn Area")]
    public float minY = 4f;
    public float maxY = 14f;
    public float spawnZ = -1.16f;
    public float spawnXCameraOfsset = 35f;

    [Header("Bird speed")]
    public float minForce = 50f;
    public float maxForce = 400f;

    public static BirdGenerator Instance {
        get {
            if (_instance == null) {
                GameObject go = new GameObject("BirdGenerator");
                _instance = go.AddComponent<BirdGenerator>();
            }
            return _instance;
        }
    }

    private void Awake() {
        _instance = this;
    }

    public void StartSpawning() {
        StartCoroutine(SpawnBirdRutine());
    }

    /**
     * Coroutine that spawns birds at regular intervals within the defined vertical range.
     * The Z position is fixed, and the X position is offset from the camera's current position.
     */
    private IEnumerator SpawnBirdRutine() {
        while (!GameManager.Instance.gameFinished) {
            yield return new WaitForSeconds(spawnInterval);

            float spawnY = Random.Range(minY, maxY);

            Vector3 spawnPosition = new Vector3(
                GameManager.Instance.gameCamera.transform.position.x + spawnXCameraOfsset,
                spawnY,
                spawnZ
            );

            GameObject bird = Instantiate(birdPrefab, spawnPosition, birdPrefab.transform.rotation);
            bird.transform.GetComponent<BridController>().horizontalForce = Random.Range(minForce, maxForce);
        }
    }
}
