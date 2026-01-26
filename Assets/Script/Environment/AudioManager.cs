using UnityEngine;
//AudioManagerオブジェクトにアタッチ
public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;
    private AudioSource audioSource;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            audioSource = GetComponent<AudioSource>();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // 外部から音を指定して鳴らすメソッド
    public void PlaySE(AudioClip clip, float volume)
    {
        audioSource.volume = volume;
        audioSource.PlayOneShot(clip);
    }
}