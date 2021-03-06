﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class CombatSetting
{
    public CombatType combatType;
    public float triggerFrame;
    public bool isKnockDown;
    public float stiffTime;
    public bool isCrit;
    public AudioClip startClip;
    public AudioClip hitClip;

    [Space(20)]
    public int damage;
    public Vector2 applyVelocity;
    [Space(20)]
    public GameObject projectile;
    public Vector2 launchPoint;

    public void Execute(CombatHandler handler, StateType statetype)
    {
        if (startClip != null)
        {
            handler.GetComponent<AudioSource>().clip = startClip;
            handler.GetComponent<AudioSource>().Play();
        }

        CombatInfo combatInfo = new CombatInfo(statetype, damage, applyVelocity, isKnockDown, stiffTime, isCrit, hitClip);
        if(combatType == CombatType.Attack)
        {
            handler.PrepareAttack(combatInfo);
        }
        else if (combatType == CombatType.Project)
        {
            Vector3 point = new Vector3(launchPoint.x, launchPoint.y, 0f);
            int flip = handler.transform.rotation.eulerAngles.y > 90f  ? -1 : 1;
            point.x *= flip;
            Projectile instance = Object.Instantiate(projectile, handler.transform.position + point, handler.transform.rotation).GetComponent<Projectile>();
            instance.ownerId = handler.gameObject.name;
            instance.combatInfo = combatInfo;
        }
    }
}
