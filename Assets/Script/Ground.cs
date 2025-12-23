using UnityEngine;
public class Ground : MonoBehaviour
{
    public void enableGround()
    {
        // 0～個数-1までの子を順番に配列に格納
        for (var i = 0; i < transform.childCount; i++)
        {
            Transform child = transform.GetChild(i);
            if (child.CompareTag("RedGround"))
            {
                
            }
        }
    }
}