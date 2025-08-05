using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class ImageSlot : MonoBehaviour
{
    [SerializeField] Image bgImage;

    [SerializeField] Color originalColor;

    public void Init()
    {
        
    }

    public void Clear()
    {
        bgImage.color = originalColor;
    }

    public void OnSelectionChanged(bool selected)
    {
        bgImage.color = (selected) ? GlobalSettings.i.BgHighlightedColor : originalColor;
    }
}
