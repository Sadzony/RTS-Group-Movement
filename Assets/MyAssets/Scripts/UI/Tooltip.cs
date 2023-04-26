using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static System.Net.Mime.MediaTypeNames;

public class Tooltip : MonoBehaviour
{
    //Singleton Template
    private static Tooltip instance;
    public static Tooltip Instance
    {
        get { return instance; }
        private set { instance = value; }
    }
    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            instance = this;
        }
        Cursor.lockState = CursorLockMode.Confined;
        canvasRect = transform.parent.GetComponent<RectTransform>();
        tooltipText = transform.Find("Text").GetComponent<TextMeshProUGUI>();
        tooltipRectTransform = transform.Find("Background").GetComponent<RectTransform>();
        parentRectTransform = transform.GetComponent<RectTransform>();
        Hide();
    }
    bool enabled = false;
    RectTransform canvasRect;
    public float textPaddingSize;
    private TextMeshProUGUI tooltipText;
    private RectTransform parentRectTransform;
    private RectTransform tooltipRectTransform;

    string currentText = "";
    public void Show(string text)
    {
        gameObject.SetActive(true);
        currentText = text;
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }
    public void UpdateText()
    {
        tooltipText.SetText(currentText);
        tooltipText.ForceMeshUpdate();

        Vector2 backgroundSize = tooltipText.GetRenderedValues(false);
        Vector2 paddingSize = new Vector2(textPaddingSize * 2, textPaddingSize * 2);

        tooltipRectTransform.sizeDelta = backgroundSize + paddingSize;
    }

    public void UpdateTooltipPosition()
    {
        Vector2 anchoredPosition = new Vector2(Input.mousePosition.x / canvasRect.localScale.x, Input.mousePosition.y / canvasRect.localScale.y);
        anchoredPosition -= new Vector2(tooltipRectTransform.rect.width / 2, -10);
        
        //Render tooltip below cursor
        if(anchoredPosition.y > (canvasRect.rect.height/2) - 100)
        {
            anchoredPosition -= new Vector2(0, tooltipRectTransform.rect.height + 58);
        }

        if(anchoredPosition.x + tooltipRectTransform.rect.width > canvasRect.rect.width)
        { 
            anchoredPosition.x = canvasRect.rect.width - tooltipRectTransform.rect.width;
        }
        if (anchoredPosition.y + tooltipRectTransform.rect.height > canvasRect.rect.height)
        {
            anchoredPosition.y = canvasRect.rect.height - tooltipRectTransform.rect.height;
        }
        if(anchoredPosition.x < 0)
        {
            anchoredPosition.x = 0;
        }
        if(anchoredPosition.y < 0)
            anchoredPosition.y = 0;


        parentRectTransform.anchoredPosition = anchoredPosition;

    }
}
