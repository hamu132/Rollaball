using UnityEngine;

public class TextureScroller : MonoBehaviour
{
    // スクロール速度（X方向とY方向）
    public Vector2 scrollSpeed = new Vector2(0.1f, 0.05f);
    private Renderer ren;

    void Start()
    {
        ren = GetComponent<Renderer>();
    }

    void Update()
    {
        // 時間経過に合わせてオフセット（ずらし）を計算
        Vector2 offset = Time.time * scrollSpeed;
        // マテリアルのメインテクスチャのオフセットを変更
        ren.material.mainTextureOffset = offset;
    }
}