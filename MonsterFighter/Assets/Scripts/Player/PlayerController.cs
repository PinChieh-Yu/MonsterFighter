﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator), typeof(PlayerInfo), typeof(PhysicsObject))]
public class PlayerController : MonoBehaviour
{
    protected PhysicsObject physics;
    private Animator animator;

    private Vector3 initPosition;
    private Vector3 falloutPosition;
    private Dictionary<string, KeyCode> controlSet;

    public StateType CurrentState { get; set; }
    private bool enableBaseInput;
    private bool enableCombatInput;

    private CombatTrigger combatTrigger;

    private bool isDummy;
    
	private void Awake () {
        physics = GetComponent<PhysicsObject>();
        animator = GetComponent<Animator>();
    }

    protected virtual void Update ()
    {
        HandleBaseOperation();
        HandleCombatOperation();
        UpdateAnimator();
    }

    private void HandleBaseOperation()
    {
        if (!enableBaseInput) return;
        if (isDummy)
        {
            if (physics.IsGrounded) physics.Forward(0f);
            return;
        }

        if (Input.GetKey(controlSet["Right"]))
        {
            physics.IsFaceRight = true;
            physics.Forward(5f);
        }
        else if (Input.GetKey(controlSet["Left"]))
        {
            physics.IsFaceRight = false;
            physics.Forward(5f);
        }
        else if (physics.IsGrounded)
        {
            physics.Forward(0f);
        }

        if ((Input.GetKey(controlSet["Left"]) || Input.GetKey(controlSet["Right"])) && CurrentState == StateType.Base && physics.IsGrounded)
        {
            GetComponent<PlayerInfo>().CurrentManaPoint += Time.deltaTime * 10;
        }

        if (Input.GetKeyDown(controlSet["Up"]) && physics.IsGrounded)
        {
            physics.Jump();
        }
    }

    private void HandleCombatOperation()
    {
        if (isDummy || !enableCombatInput) return;

        if (CurrentState == StateType.Base)
        {
            ResetTriggers();
            combatTrigger = CombatTrigger.None;
        }

        if (Input.GetKeyDown(controlSet["AtkH"]))
        {
            combatTrigger = CombatTrigger.AtkH;
        }
        else if (Input.GetKeyDown(controlSet["AtkL"]))
        {
            combatTrigger = CombatTrigger.AtkL;
        }
        else if (Input.GetKey(controlSet["SklS"]))
        {
            combatTrigger = CombatTrigger.SklS;
        }
        else if (Input.GetKey(controlSet["SklB"]))
        {
            combatTrigger = CombatTrigger.SklB;
        }

        if (CurrentState == StateType.Base)
        {
            TriggerNextCombatState();
        }
    }

    private void ResetTriggers()
    {
        animator.ResetTrigger("AtkL");
        animator.ResetTrigger("AtkH");
        animator.ResetTrigger("SklS");
        animator.ResetTrigger("SklB");
    }

    public void TriggerNextCombatState()
    {
        PlayerInfo plyinf = GetComponent<PlayerInfo>();
        string trigger = Enum.GetName(typeof(CombatTrigger), combatTrigger);
        if (combatTrigger != CombatTrigger.None)
        {
            if (combatTrigger == CombatTrigger.SklS && CurrentState == StateType.Base && physics.IsGrounded && plyinf.AbleToCastSkillS)
            {
                animator.SetTrigger(trigger);
            }
            else if (combatTrigger == CombatTrigger.SklB && CurrentState == StateType.Base && physics.IsGrounded && plyinf.AbleToCastSkillB)
            {
                animator.SetTrigger(trigger);
            }
            else if (combatTrigger != CombatTrigger.SklS && combatTrigger != CombatTrigger.SklB)
            {
                animator.SetTrigger(trigger);
            }
            combatTrigger = CombatTrigger.None;
        }
    }

    private void UpdateAnimator()
    {
        animator.SetBool("IsGrounded", physics.IsGrounded);
        animator.SetFloat("SpeedX", physics.Velocity.x);
        animator.SetFloat("SpeedY", physics.Velocity.y);
    }

    public void SetupController(Dictionary<string, KeyCode> controlset, Vector3 initposition, Vector3 falloutposition)
    {
        controlSet = controlset;
        initPosition = initposition;
        falloutPosition = falloutposition;
        GetComponentInChildren<AttackboxDetector>().Id = gameObject.name;
        isDummy = (controlset.Count == 0);
    }

    public void ResetController()
    {
        if (damageCo != null) StopCoroutine(damageCo);
        animator.ResetTrigger("WakeUp");
        animator.Rebind();
        transform.position = initPosition;
        physics.IsFaceRight = gameObject.name == "0";
        physics.IsGrounded = false;
        SetInvincible(false);
    }

    private IEnumerator damageCo;
    public void Damaged(Vector2 applyVelocity, float stiffTime, bool isKnockDown, float enemyXPosition)
    {
        combatTrigger = CombatTrigger.None;
        physics.IsFaceRight = enemyXPosition > transform.position.x;
        enableBaseInput = false;
        if (damageCo != null) StopCoroutine(damageCo);
        physics.SetPhysicsParam(applyVelocity, Vector2.zero, true);
        damageCo = Stiff(stiffTime, isKnockDown);
        StartCoroutine(damageCo);
    }

    public void Fallout()
    {
        transform.position = falloutPosition;
        physics.Forward(0f);
        if (damageCo != null) StopCoroutine(damageCo);
        damageCo = Stiff(0f, true);
        StartCoroutine(damageCo);
    }

    IEnumerator Stiff(float duration, bool knockDown)
    {
        SetInputActivate(false, false);
        animator.ResetTrigger("WakeUp");
        animator.SetTrigger("Damaged");
        yield return new WaitForSeconds(duration);
        if (knockDown)
        {
            physics.SetPhysicsParam(Vector2.zero, Vector2.zero, true);
            SetInvincible(true);
            animator.SetTrigger("KnockDown");
            yield return new WaitForSeconds(1f);
            WakeUp();
            yield return new WaitForSeconds(1f);
            SetInvincible(false);
        }
        else
        {
            WakeUp();
        }
    }

    public void SetInputActivate(bool enableBase, bool enableCombat)
    {
        enableBaseInput = enableBase;
        enableCombatInput = enableCombat;
    }

    public void WakeUp()
    {
        animator.SetTrigger("WakeUp");
        SetInputActivate(true, true);
    }

    private void SetInvincible(bool yes)
    {
        if (yes)
        {
            GetComponent<SpriteRenderer>().color = new Color(255f, 255f, 255f, 0.4f);
            GetComponent<CombatHandler>().Invincible = true;
        }
        else
        {
            GetComponent<SpriteRenderer>().color = new Color(255f, 255f, 255f, 1f);
            GetComponent<CombatHandler>().Invincible = false;
        }
    }
}
