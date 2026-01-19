using System.Linq.Expressions;
using UnityEngine;
using UnityEngine.Splines; // 必須

public class SplineController : MonoBehaviour
{
    [SerializeField] private SplineContainer splineContainer;

    /// <summary>
    /// スプラインの特定のポイントを指定したワールド座標に移動させる
    /// </summary>
    /// <param name="index">移動させるポイントの番号（0から開始）</param>
    /// <param name="worldPosition">移動先のワールド座標</param>
    public void SetKnotWorldPosition(int index, Vector3 worldPosition)
    {
        if (splineContainer == null || splineContainer.Spline == null) return;

        // 指定したインデックスがスプラインの点数以内かチェック
        if (index < 0 || index >= splineContainer.Spline.Count) return;

        // 1. ワールド座標を SplineContainer のローカル座標に変換
        Vector3 localPos = splineContainer.transform.InverseTransformPoint(worldPosition);

        // 2. 現在の Knot を取得して座標を書き換え
        var knot = splineContainer.Spline[index];
        knot.Position = localPos;

        // 3. スプラインに反映（構造体なので再代入が必要）
        splineContainer.Spline[index] = knot;
    }
    // public void Start()
    // {
    //     SetKnotWorldPosition(1,new Vector3(1,1,1));
    // }
}