using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Condition : MonoBehaviour
{
    public float curValue;
    public float startValue;
    public float maxValue;
    public float passiveValue;
    public Image uiBar;


    void Start()
    {
        curValue = startValue; // 이후 저장된 데이터, 게임을 저장하고 재시작 할 때
    }

    // Update is called once per frame
    void Update()
    {
        uiBar.fillAmount = Getpercentage();
    }

    float Getpercentage()
    {
        return curValue / maxValue;
    }

    public void Add(float value)
    {
        curValue = Mathf.Min(curValue + value, maxValue);
    }

    public void Subtract(float value)
    {
        curValue = Mathf.Max(curValue - value, 0f);
    }
}
