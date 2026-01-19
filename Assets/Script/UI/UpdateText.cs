using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class UpdateText : MonoBehaviour
{
    public static UpdateText instance;
    void Awake() { instance = this; }
    public TextMeshProUGUI itemCountText;
    public TextMeshProUGUI micVolText;
    public void SetItemCount(int count)
    {
        itemCountText.text = "Count: " + count.ToString() + "/" + GameDirector.instance.itemNum;
    }
    public void SetMicVol(float vol)
    {
        micVolText.text = "Mic Vol: " + vol.ToString("F2");
    }
}

