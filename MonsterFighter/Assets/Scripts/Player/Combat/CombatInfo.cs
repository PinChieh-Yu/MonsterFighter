﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class CombatInfo
{
    public int damage;
    public Vector2 applyVelocity;
    public bool isKnockDown;
    public float stiffTime;
    public AudioClip hitClip;

    public CombatInfo(int damage, Vector2 applyvelocity, bool isknockdown, float stifftime, AudioClip hitclip)
    {
        this.damage = damage;
        applyVelocity = applyvelocity;
        isKnockDown = isknockdown;
        stiffTime = stifftime;
        hitClip = hitclip;
    }
}
