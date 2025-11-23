using UnityEngine;
using System.Collections;

public class IntroCamController : MonoBehaviour
{
    [Header("Cameras & Player")]
    public Camera introCam;
    public Camera playerCam;
    public MonoBehaviour playerController;
    public GameObject canvas;

    [Header("Offsets")]
    public Vector3 startOffset = new Vector3(0, 1.8f, 2f);
    public Vector3 endOffset = new Vector3(0, 1.8f, -3f);

    [Header("Times")]
    public float introDuration = 4f;
    public float holdTime = 0.5f;

    private Transform _player;

    void Start()
    {
        _player = playerController.transform;
        playerCam.enabled = false;
        playerController.enabled = false;
        canvas.SetActive(false);

        // Intro
        StartCoroutine(IntroSequence());
    }

    IEnumerator IntroSequence()
    {
        float t = 0f;

        // Fly-around
        while (t < introDuration)
        {
            t += Time.deltaTime;
            float frac = Mathf.Clamp01(t / introDuration);

            // Change cam position
            Vector3 offset = Vector3.Lerp(startOffset, endOffset, frac);
            introCam.transform.position = _player.position + offset;

            // Point camera at player
            introCam.transform.LookAt(_player.position);

            yield return null;
        }

        // Pause
        yield return new WaitForSeconds(holdTime);

        // Switch to player camera
        introCam.enabled = false;
        playerCam.enabled = true;
        playerController.enabled = true;
        canvas.SetActive(true);
    }
}
