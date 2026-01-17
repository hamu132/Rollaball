using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections;

[RequireComponent(typeof(AudioSource))]
public class MicInput : MonoBehaviour
{
    private AudioSource _audioSource;
    private string _micName;


    
    [Header("UI設定")]
    public TextMeshProUGUI countText;
    public Slider slider;
    public StageRoot stageRoot;

    [Header("音量感度設定")]
    [Range(0.01f, 30.0f)] public float sensitivity = 1.0f;
    [Range(0f, 1f)] public float thresholdR = 0.2f;
    [Range(0f, 1f)] public float thresholdG = 0.5f;
    [Range(0f, 1f)] public float thresholdB = 0.8f;
    [SerializeField] private Slider timerSliderR;
    [SerializeField] private Slider timerSliderG;
    [SerializeField] private Slider timerSliderB;
    [Header("溜め時間設定")]
    [Range(0.6f, 10.0f)]public float requiredDuration = 0.6f; // 必要な継続時間
    
    // それぞれの色の溜め時間を計測するカウンター
    private float _timerR = 0f;
    private float _timerG = 0f;
    private float _timerB = 0f;

    private float _currentVolume = 0;
    private const int SAMPLE_COUNT = 1024;
    private Coroutine currentCoroutineR;
    private Coroutine currentCoroutineG;
    private Coroutine currentCoroutineB;
    private bool _micMode;
    private float returnSpeed = 5f;

    void Start()
    {
        micSetup();
    }

    void micSetup()
    {
        _audioSource = GetComponent<AudioSource>();
        if (Microphone.devices.Length > 0)
        {
            _micName = Microphone.devices[0];
            _audioSource.clip = Microphone.Start(_micName, true, 1, AudioSettings.outputSampleRate);
            _audioSource.loop = true;
            while (!(Microphone.GetPosition(_micName) > 0)) { }
            _audioSource.Play();
            _micMode = true;
        }
        else
        {
            _micMode = false;
            Debug.LogError("マイクが接続されていません");
        }
    }

    void Update()
    {
        if (PlayerController.timerf == 0 && _micMode)
        {
            // 1. 波形データの取得と音量(RMS)計算
            float[] samples = new float[SAMPLE_COUNT];
            _audioSource.GetOutputData(samples, 0);
            float sum = 0;
            for (int i = 0; i < samples.Length; i++)
            {
                sum += samples[i] * samples[i];
            }
            float rms = Mathf.Sqrt(sum / SAMPLE_COUNT);

            // 2. 滑らかな音量の更新
            float targetVolume = rms * sensitivity;
            _currentVolume = Mathf.Lerp(_currentVolume, targetVolume, Time.deltaTime * 15f);
        }
        else
        {
            float targetVolume = PlayerController.timerf;
            _currentVolume = Mathf.Lerp(_currentVolume, targetVolume, Time.deltaTime * 15f);
        }

        // 3. UI更新
        countText.text = $"Volume: {_currentVolume:F4}\nTimerR: {_timerR:F1}\nTimerG: {_timerG:F1}\nTimerB: {_timerB:F1}";
        slider.value = Mathf.Clamp01(_currentVolume);

        // 4. 床の出現判定（溜め時間のロジック）
        
        // --- 青色の判定（大声） ---
        if (currentCoroutineB == null){
            if (_currentVolume > thresholdB)
            {
                _timerB += Time.deltaTime;
                if (_timerB >= requiredDuration)
                {
                    stageRoot.enableGround("BlueGround");
                    currentCoroutineB = StartCoroutine(WaitCoroutineB());
                }
            }
            else { _timerB = 0f; } // しきい値を下回ったら即座にリセット
        }
        if (timerSliderB != null) timerSliderB.value = Mathf.Clamp01(_timerB / requiredDuration);


        // --- 緑色の判定（中声） ---
        if (currentCoroutineG == null){
            if (thresholdB >= _currentVolume && _currentVolume > thresholdG)
            {
                _timerG += Time.deltaTime;
                if (_timerG >= requiredDuration)
                {
                    stageRoot.enableGround("GreenGround");
                    currentCoroutineG = StartCoroutine(WaitCoroutineG());
                }
            }
            else { _timerG = 0f; }
        }
        if (timerSliderG != null) timerSliderG.value = Mathf.Clamp01(_timerG / requiredDuration);

        // --- 赤色の判定（小声） ---
        if (currentCoroutineR == null)
        {
            if (thresholdG >= _currentVolume && _currentVolume > thresholdR)
            {
                _timerR += Time.deltaTime;
                if (_timerR >= requiredDuration)
                {
                    stageRoot.enableGround("RedGround");
                    currentCoroutineR = StartCoroutine(WaitCoroutineR());
                }
            }
            else { _timerR = 0f; }
        }
        if (timerSliderR != null) timerSliderR.value = Mathf.Clamp01(_timerR / requiredDuration);
    }
    IEnumerator WaitCoroutineR()
    {
        float elapsed = 0;
        while (elapsed < stageRoot.GetComponent<StageRoot>().mainDuration)
        {
            if (!StageRoot.isTimePaused)
            {
                elapsed += Time.deltaTime;
            }
            yield return null;
        }
        while (_timerR > 0)
        {
            _timerR -= returnSpeed * Time.deltaTime;
            yield return null;
        }
        currentCoroutineR = null;
        _timerR = 0;
    }
    IEnumerator WaitCoroutineG()
    {
        float elapsed = 0;
        while (elapsed < stageRoot.GetComponent<StageRoot>().mainDuration)
        {
            if (!StageRoot.isTimePaused)
            {
                elapsed += Time.deltaTime;
            }
            yield return null;
        }
        while (_timerG > 0)
        {
            _timerG -= returnSpeed * Time.deltaTime;
            yield return null;
        }
        currentCoroutineG = null;
        _timerG = 0;
    }
    IEnumerator WaitCoroutineB()
    {
        float elapsed = 0;
        while (elapsed < stageRoot.GetComponent<StageRoot>().mainDuration)
        {
            if (!StageRoot.isTimePaused)
            {
                elapsed += Time.deltaTime;
            }
            yield return null;
        }
        
        while (_timerB > 0)
        {
            _timerB -= returnSpeed * Time.deltaTime;
            yield return null;
        }
        currentCoroutineB = null;
        _timerB = 0;
    }
}