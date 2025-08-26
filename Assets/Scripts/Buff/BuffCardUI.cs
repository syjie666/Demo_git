using UnityEngine;
using UnityEngine.UI;

public class BuffCardUI : MonoBehaviour
{
    public Text title;
    public Text desc;

    private BuffData buffData;

    public void SetBuffData(BuffData buff)
    {
        buffData = buff;
        if (title != null)
            title.text = buff.buffName;
        if (desc != null)
            desc.text = buff.description;
    }
}
