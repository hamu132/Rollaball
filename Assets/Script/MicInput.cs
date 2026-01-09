using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class MicInput : MonoBehaviour
{
    private AudioSource _audioSource;
    private string _micName;

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
            
            // 自分の声がスピーカーから出ないようにミュート（解析には影響しません）
            _audioSource.mute = true;
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
        Debug.Log(maxV);

        // インデックスを実際の周波数(Hz)に変換
        float freq = maxIndex * AudioSettings.outputSampleRate / 2 / spectrum.Length;
        
        if (maxV > 0.01f) // 一定以上の音量がある時だけ
        {
            Debug.Log($"現在の推定周波数: {freq} Hz");
            // ここでfreqの値を使ってゲームの要素（キャラのジャンプ力や色など）を変える
        }
    }
}