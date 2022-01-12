using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisableXIfAllYDisabled    : MonoBehaviour
{
    [SerializeField]
    private GameObject x;
    [SerializeField]
    private GameObject[] y;

    private void Update()
    {
        foreach (GameObject obj in y)
        {
            if (obj.gameObject.activeSelf)
            {
                x.gameObject.SetActive(true);
                return;
            }
        }

        x.gameObject.SetActive(false);
    }
}
