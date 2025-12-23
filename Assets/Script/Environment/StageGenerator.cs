using UnityEngine;

public class StageGenerator : MonoBehaviour
{
    public GameObject stagePrefab; // 生成するステージのプレハブ
    public int xCount = 10;        // 横方向（X）に並べる数
    public int zCount = 10;        // 奥行方向（Z）に並べる数
    public float spacing = 15f;    // 基本の間隔

    [Header("ランダム設定")]
    public float posOffsetMax = 5f;    // 位置のズレ（最大）
    public float heightRange = 3f;     // 高さ（Y）の変動幅
    public float minScale = 0.8f;      // サイズの最小値
    public float maxScale = 1.5f;      // サイズの最大値

    void Start()
    {
        GenerateStages();
    }

    void GenerateStages()
    {
        for (int x = 0; x < xCount; x++)
        {
            for (int z = 0; z < zCount; z++)
            {
                if(x-xCount/2 == 0 && z-zCount/2 == 0)
                {
                    // 最初のステージは生成しない（プレイヤーのスタート位置確保）
                    continue;
                }
                // ① 基本位置を計算（中心をずらす）
                float posX = (x - xCount / 2f) * spacing;
                float posZ = (z - zCount / 2f) * spacing;

                // ② ランダムな要素を加える
                posX += Random.Range(-posOffsetMax, posOffsetMax);
                posZ += Random.Range(-posOffsetMax, posOffsetMax);
                float posY = Random.Range(-heightRange, heightRange);

                Vector3 spawnPos = new Vector3(posX, posY, posZ);

                // ③ インスタンス化（生成）
                GameObject newStage = Instantiate(stagePrefab, spawnPos, Quaternion.identity);

                // ④ ランダムなサイズを設定
                float randomScale = Random.Range(minScale, maxScale);
                newStage.transform.localScale = new Vector3(randomScale, randomScale, randomScale);

                // ⑤ 整理のためにこのオブジェクトの子にする
                newStage.transform.SetParent(this.transform);
            }
        }
    }
}