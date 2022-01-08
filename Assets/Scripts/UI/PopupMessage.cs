using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PopupMessage : MonoBehaviour
{
    public string Text
    {
        set
        {
            text.text = Truncate(value, maxTextLength);
        }
    }

    [SerializeField]
    private int maxTextLength;
    [SerializeField]
    private float lifetime;
    [SerializeField]
    private Button closeButton;
    [SerializeField]
    private TextMeshProUGUI text;
    private DebugMessageType type;

    private void Awake()
    {
        closeButton.onClick.AddListener(CloseButtonClickedHandler);
        if (lifetime > 0f)
        {
            Destroy(gameObject, lifetime);
        }
    }

    private void CloseButtonClickedHandler()
    {
        Destroy(gameObject);
    }

    private string Truncate(string s, int maxLength)
    {
        if (maxLength <= 0)
        {
            return s;
        }

        if (string.IsNullOrEmpty(s))
        {
            return s;
        }

        if (s.Length <= maxLength)
        {
            return s;
        }

        return s.Substring(0, maxLength) + "..";
    }
}
