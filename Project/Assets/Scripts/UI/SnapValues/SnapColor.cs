using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SnapColor : MonoBehaviour
{
    public Color[] colors;
    public SnapScrollbox snapScrollbox;
    private Image image;
    void Start()
    {
        image = GetComponent<Image>();
    }

    void Update()
    {
        int leftColor = Mathf.Clamp(Mathf.FloorToInt(snapScrollbox.value), 0, colors.Length - 1);
        int rightColor = Mathf.Clamp(Mathf.CeilToInt(snapScrollbox.value), 0, colors.Length - 1);
        image.color = Color.Lerp(colors[leftColor], colors[rightColor], snapScrollbox.value % 1);
    }
}
