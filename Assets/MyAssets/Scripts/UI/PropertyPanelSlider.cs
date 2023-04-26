using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PropertyPanelSlider : MonoBehaviour
{
    public Slider slider;
    public Text textValue;
    [HideInInspector] public float defaultValue;
    // Start is called before the first frame update
    void Start()
    {
        slider = GetComponent<Slider>();
        defaultValue = slider.value;
        PropertyPanelManager.Instance.sliders.Add(this);
    }
}
