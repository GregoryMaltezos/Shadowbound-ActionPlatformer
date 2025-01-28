using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HpBar : MonoBehaviour
{
    [SerializeField] private Slider slider;

    public void UpdateHpBar(float currentHP, float maxHP)
    {
        slider.value = currentHP / maxHP;
    }

    // Update is called once per frame
    void Update()
    {

    }
}
