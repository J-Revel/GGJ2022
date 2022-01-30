using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class StateGameButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField]
    private float outScale = 1f;
    [SerializeField]
    private float hoverScale = 1.2f;
    [SerializeField]
    private float scaleVariationDuration = 1.2f;

    private float currentScaleValue = 1f;
    private bool isHoover = false;

    public void OnPointerExit(PointerEventData eventData)
    {
        isHoover = false;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        isHoover = true;
    }

    private void Update()
    {
        if(isHoover && currentScaleValue < hoverScale)
        {
            currentScaleValue = Mathf.Min(currentScaleValue + Time.deltaTime / scaleVariationDuration, hoverScale);
            UpdateScale();
        }
        else if (!isHoover && currentScaleValue > outScale)
        {
            currentScaleValue = Mathf.Max(currentScaleValue - Time.deltaTime / scaleVariationDuration, outScale);
            UpdateScale();
        }

    }

    private void UpdateScale()
    {
        this.transform.localScale = Vector3.one * currentScaleValue;
    }

    public void LoadTutorial()
    {
        SceneManager.LoadScene($"Level0", LoadSceneMode.Single);
    }
}
