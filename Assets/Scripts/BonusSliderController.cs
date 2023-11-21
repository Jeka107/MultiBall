using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class BonusSliderController : MonoBehaviour
{
    [SerializeField] private float sliderSpeed;
    private Slider slider;
    private int progress = 0;

    private void Awake()
    {
        slider=GetComponent<Slider>();

        GameManager.onSliderprogress += UpdateProgress;
        GameManager.onSliderReset += RemoveProgress;

    }
    private void OnDestroy()
    {
        GameManager.onSliderprogress -= UpdateProgress;
        GameManager.onSliderReset -= RemoveProgress;
    }
    private void UpdateProgress()
    {
        progress++;
        StartCoroutine(FillAll());
    }
    IEnumerator FillAll()
    {
        int i = 0;

        while (i != slider.maxValue)
        {
            slider.value++;
            i++;
            yield return new WaitForFixedUpdate();
        }
        StartCoroutine(FillProgress());
    }
    IEnumerator FillProgress()
    {
        while (progress != slider.value)
        {
            slider.value--;
            yield return new WaitForFixedUpdate();
        }
    }
    private void RemoveProgress()
    {
        progress = 0;
        slider.value = progress;
    }
}
