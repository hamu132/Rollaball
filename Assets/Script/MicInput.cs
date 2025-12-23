using UnityEngine;
using TMPro;
[RequireComponent(typeof(AudioSource))]
public class MicInput : MonoBehaviour
{
    private AudioSource _audioSource;
    private string _micName;
    public GameObject obj;
    public TextMeshProUGUI countText;

    void Start()
    {
        _audioSource = GetComponent<AudioSource>();

        // 利用可能なマイクを探す
        if (Microphone.devices.Length > 0)
        {
            _micName = Microphone.devices[0];
            // 録音開始（ループ設定、1秒間のバッファ、マイクのサンプリングレート）
            _audioSource.clip = Microphone.Start(_micName, true, 1, AudioSettings.outputSampleRate);
            _audioSource.loop = true;
            
            // 録音が始まるまで待機
            while (!(Microphone.GetPosition(_micName) > 0)) { }
            _audioSource.Play();
        }
        else
        {
            Debug.Log("マイクが接続されていません");
        }
    }
    void Update()
    {
        float[] spectrum = new float[1024]; // 2の累乗である必要があります
        _audioSource.GetSpectrumData(spectrum, 0, FFTWindow.BlackmanHarris);

        float maxV = 0;
        int maxIndex = 0;

        // 一番強度が強いビン（周波数の箱）を探す
        for (int i = 0; i < spectrum.Length; i++)
        {
            if (spectrum[i] > maxV)
            {
                maxV = spectrum[i];
                maxIndex = i;
            }
        }

        // インデックスを実際の周波数(Hz)に変換
        float freq = maxIndex * AudioSettings.outputSampleRate / 2 / spectrum.Length;
        
        if (maxV < 0.001f) // 一定以上の音量がある時だけ
        {
            freq = 0;
        }
        countText.text = $"Count: {maxV} \nfreq:{freq}";
    }
}