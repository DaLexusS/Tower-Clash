using TMPro;
using UnityEngine.Events;
using UnityEngine;
using UnityEngine.UI;

public class SummonButtonHandler : MonoBehaviour
{
    public TMP_Text Price;
    public Image Icon;

    private UnityAction _onClick;

    public void Init(Sprite iconSprite, int price, UnityAction onClick)
    {
        Price.text = UiUtils.FormatWithCommas(price).ToString();
        Icon.sprite = iconSprite;
        _onClick = onClick;
    }

    public void HandleClick()
    {
        _onClick?.Invoke();
    }
}