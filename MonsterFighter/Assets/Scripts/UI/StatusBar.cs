﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StatusBar : MonoBehaviour {

    private Image icon;
    private Image HPBar;
    private Image MPBar;
    private Image KnockdownBar;
    private Image[] Lights = new Image[2];

    //private Image EPBar;
    //private BattleManager battleManager;

    private float newHpPercentage, newMpPercentage, newKnockdownPercentage;

    void Awake ()
    {
        icon = transform.Find("Icon").GetComponent<Image>();
        HPBar = transform.Find("HpBar").GetComponent<Image>();
        MPBar = transform.Find("MpBar").GetComponent<Image>();
        KnockdownBar = transform.Find("KnockdownBar").GetComponent<Image>();
        Lights[0] = transform.Find("Bubble_0").GetComponent<Image>();
        Lights[1] = transform.Find("Bubble_1").GetComponent<Image>();
    }

    void Start ()
    {
        newHpPercentage = 0f;
        newMpPercentage = 0f;
        newKnockdownPercentage = 0f;
    }
	
	void Update () {
        HPBar.fillAmount = Mathf.Lerp(HPBar.fillAmount, newHpPercentage, 0.1f);
        MPBar.fillAmount = Mathf.Lerp(MPBar.fillAmount, newMpPercentage, 0.1f);
        KnockdownBar.fillAmount = Mathf.Lerp(KnockdownBar.fillAmount, newKnockdownPercentage, 0.1f);
    }

    public void SetIcon(Sprite iconSprite)
    {
        icon.sprite = iconSprite;
    }

    public void OnPlayerHpChange(float percentage)
    {
        newHpPercentage = percentage;
    }

    public void OnPlayerMpChange(float percentage)
    {
        newMpPercentage = percentage;
    }

    public void OnPlayerKnockdownChange(float amount)
    {
        newKnockdownPercentage = amount / 30;
    }

    public void LightBubble(int lightId)
    {
        StartCoroutine("DisplayLight", lightId - 1);
    }

    IEnumerator DisplayLight(int id)
    {
        while(Lights[id].fillAmount < 1f)
        {
            Lights[id].fillAmount = Mathf.Lerp(Lights[id].fillAmount, 1f, 0.05f);
            yield return null;
        }
    }
}
