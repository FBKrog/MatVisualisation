using UnityEngine;
using TMPro;

public class TextFiller : MonoBehaviour
{
    [SerializeField] private UnityEngine.UI.Slider slider;

    private TextMeshProUGUI tmpText;
    private string fillin;

    private void Start()
    {
        tmpText = GetComponent<TextMeshProUGUI>();
        fillin = tmpText.text;

        if (slider != null)
            FillText(slider.value);
    }

    public void FillText(float value)
    {
        tmpText.text = string.Format(fillin, value.ToString("0.00"));
    }
}
