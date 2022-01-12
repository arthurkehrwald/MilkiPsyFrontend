using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LayoutSwapper : MonoBehaviour
{
    public enum Layout { None, Horizontal, Vertical };

    [Tooltip("Swap objects will be parented to the horizontal group of the" +
        "aspect ratio of the groups parent is above this value")]
    [SerializeField]
    private float swapThreshold = 1f;
    [SerializeField]
    private RectTransform groupsParent;
    [SerializeField]
    private HorizontalLayoutGroup horizontalGroup;
    [SerializeField]
    private VerticalLayoutGroup verticalGroup;
    [SerializeField]
    private RectTransform[] swapObjects;

    private RectTransform horizontalRect;
    private RectTransform verticalRect;
    private Layout currentLayout;
    private Layout CurrentLayout
    {
        get => currentLayout;
        set
        {
            if (value == currentLayout)
            {
                return;
            }

            currentLayout = value;

            foreach(RectTransform obj in swapObjects)
            {
                obj.parent = currentLayout switch
                {
                    Layout.Horizontal => horizontalGroup.transform,
                    Layout.Vertical => verticalGroup.transform
                };
            }
        }
    }

    private void Update()
    {
        float aspect = GetAspect();

        if (aspect > swapThreshold)
        {
            CurrentLayout = Layout.Horizontal;
        }
        else
        {
            CurrentLayout = Layout.Vertical;
        }
    }

    private float GetAspect()
    {
        return groupsParent.rect.width / groupsParent.rect.height;
    }
}
