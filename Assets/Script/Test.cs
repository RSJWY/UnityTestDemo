using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class Test : MonoBehaviour
{
    [SerializeField] public Slider slider;

    [Range(1,10)]
    public int time;

    public TextMeshProUGUI text;
    public float count;
    public float bujin;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //StartCoroutine(Test1());
        slider.value = 0f;
        DOTween.To(
            ()=>slider.value,
            value=>slider.value=value,
            1,time)
            .OnUpdate(() =>
            {
                text.text=Mathf.RoundToInt(slider.value*100).ToString()+"%";
            })
            .SetEase(Ease.Linear);
    }

    IEnumerator Test1()
    {
        slider.value = 0f;
        float elapsedTime = 0f;
    
        while (elapsedTime < time)
        {
            elapsedTime += Time.deltaTime;
            slider.value = Mathf.Clamp01(elapsedTime / time);
            text.text=$"{Mathf.RoundToInt(slider.value*100)}%";
            yield return null; // 每帧更新一次
        }
    
        slider.value = 1f; // 确保最终为100%
    }
        
}
