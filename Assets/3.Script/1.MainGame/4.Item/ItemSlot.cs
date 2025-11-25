using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ItemSlot : MonoBehaviour
{
    public Image icon;
    public TMP_Text levelText;

    public void SetData(Sprite sprite, int level, int maxLevel)
    {
        if (sprite == null)
        {
            Hide();
            return;
        }

        icon.gameObject.SetActive(true);
        icon.sprite = sprite;

        if (maxLevel <= 0)
        {
            levelText.text = "";
        }
        else if (levelText != null && level != maxLevel)
        {
            levelText.text = "" + level;
        }
        else if (level == maxLevel)
        {
            levelText.text = "MAX";
        }

    }

    public void Hide()
    {
        icon.gameObject.SetActive(false);
        if (levelText != null) levelText.text = "";
    }
}