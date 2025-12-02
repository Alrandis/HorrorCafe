using UnityEngine;
using UnityEngine.UI;

public class FPSDisplay : MonoBehaviour
{
    public Text fpsText; // drag UI Text here
    public float updateInterval = 0.25f;

    private float _accum = 0f;
    private int _frames = 0;
    private float _timeLeft;

    private void Start()
    {
        if (fpsText == null) Debug.LogWarning("FPSDisplay: assign UI Text");
        _timeLeft = updateInterval;
    }

    private void Update()
    {
        _timeLeft -= Time.deltaTime;
        _accum += Time.timeScale / Time.deltaTime;
        _frames++;

        if (_timeLeft <= 0f)
        {
            float fps = _accum / _frames;
            if (fpsText != null) fpsText.text = Mathf.RoundToInt(fps).ToString() + " FPS";
            _timeLeft = updateInterval;
            _accum = 0f;
            _frames = 0;
        }
    }
}
