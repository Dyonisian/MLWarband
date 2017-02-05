﻿using UnityEngine;
using System.Collections;

public class EnemyScript : PlayerScript {
    bool aAttack, aLeft, aRight, aForward, aBack, aJump, aBlock;
    bool InRange;
    float ActionCooldown;
    int RState = 0;
    [SerializeField]

    PlayerScript PScript;
    

	// Use this for initialization
	void Start () {
        PState = State.Idle;
        //m_Animator = GetComponent<Animator>();
        m_Rigidbody = GetComponent<Rigidbody>();
        m_Capsule = GetComponent<CapsuleCollider>();
        m_CapsuleHeight = m_Capsule.height;
        m_CapsuleCenter = m_Capsule.center;
        AnimationControl = GetComponentInChildren<Animation>();
        m_Rigidbody.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY | RigidbodyConstraints.FreezeRotationZ;
        m_OrigGroundCheckDistance = m_GroundCheckDistance;
        StartCoroutine("AttackCombo");
        StartCoroutine("Walk");
        HumanPlayer = false;
    }
	
	// Update is called once per frame
	void Update () {
        
    }
    void FixedUpdate()
    {
        StateText.text = PState.ToString();
        DebugTimer += Time.deltaTime;
        Invincibility -= Time.deltaTime;
        ActionCooldown -= Time.deltaTime;
        if (Hit)
        {
            PState = State.Hit;
            foreach (AnimationState state in AnimationControl)
            {
                state.speed = 0.6f;
            }
            AnimationControl.Play("hit 1");
            StopAllCoroutines();
            StartCoroutine("IsHit");
            h = 0;
            v = 0;
            StartCoroutine("Walk");
            Hit = false;


        }
        if (PState != State.Hit)
        {
            m_Direction = Enemy.transform.position - transform.position;
            transform.rotation = Quaternion.LookRotation(m_Direction);

            if (ActionCooldown <= 0.0f)
            {
                RState = Random.Range(0, 8);
                //Debug.Log("Switching to state" + RState);
                switch (RState)
                {
                    //idle
                    case 0:
                        PState = State.Idle;
                        h = 0;
                        v = 0;
                        AnimationControl.Play("ready");
                        ActionCooldown = 1.0f;
                        break;
                    //walk
                    case 1:
                        PState = State.Walk;
                        h = Random.Range(-1.0f, 1.0f);
                        v = Random.Range(-1.0f, 1.0f);

                        if (h == 0.0f && v >= 0.0f)
                            AnimationControl.Play("run");
                        else if (h < 0.0f)
                            AnimationControl.Play("strafe left");
                        else
                            AnimationControl.Play("strafe right");

                        m_Jump = false;

                        ActionCooldown = 2.0f;
                        break;
                    //jump
                    case 2:
                        PState = State.Jump;
                        h = Random.Range(-1.0f, 1.0f);
                        v = Random.Range(-1.0f, 1.0f);

                        if (h == 0.0f && v >= 0.0f)
                            AnimationControl.Play("run");
                        else if (h < 0.0f)
                            AnimationControl.Play("strafe left");
                        else
                            AnimationControl.Play("strafe right");

                        m_Jump = true;

                        ActionCooldown = 2.0f;
                        break;
                    //attack
                    case 3:
                        h = 0;
                        v = 0;
                        if (PState == State.Idle || PState == State.Walk || PState == State.Jump || PState == State.Block)
                        {
                            AnimationControl.Play("attack 1");
                            SwordAnim.Play("attack 1");
                            if (PState == State.Block)
                                Blocking = false;
                            PState = State.Attack1;


                            m_MoveSpeedMultiplier = 1.0f;
                        }
                        
                        ActionCooldown = 0.6f;
                        break;
                    case 4:
                        h = 0;
                        v = 0;
                        if (PState == State.Idle || PState == State.Walk || PState == State.Jump || PState == State.Block)
                        {
                            AnimationControl.Play("attack 2");
                            SwordAnim.Play("attack 2");
                            if (PState == State.Block)
                                Blocking = false;
                            PState = State.Attack2;


                            m_MoveSpeedMultiplier = 1.0f;
                        }

                        ActionCooldown = 0.6f;
                        break;
                    case 5:
                        h = 0;
                        v = 0;
                        if (PState == State.Idle || PState == State.Walk || PState == State.Jump || PState == State.Block)
                        {
                            AnimationControl.Play("attack 3");
                            SwordAnim.Play("attack 3");
                            if (PState == State.Block)
                                Blocking = false;
                            PState = State.Attack3;


                            m_MoveSpeedMultiplier = 1.0f;
                        }

                        ActionCooldown = 0.6f;
                        break;
                    case 6:
                        h = 0;
                        v = 0;
                        if (PState == State.Idle || PState == State.Walk || PState == State.Jump || PState == State.Block)
                        {
                            AnimationControl.Play("attack 4");
                            SwordAnim.Play("attack 4");
                            if (PState == State.Block)
                                Blocking = false;
                            PState = State.Attack4;


                            m_MoveSpeedMultiplier = 1.0f;
                        }

                        ActionCooldown = 0.6f;
                        break;
                    //Block
                    case 7:
                        ContinueCombo = false;
                        Blocking = true;
                        if (PState == State.Idle || PState == State.Walk)
                        {
                            PState = State.Block;
                            AnimationControl.Play("block");
                            StartCoroutine("PauseAnimation");

                            m_MoveSpeedMultiplier = 1.0f;
                        }
                        PState = State.Block;
                        ActionCooldown = 2.0f;
                        break;
                    default:
                        PState = State.Idle;
                        h = 0;
                        v = 0;
                        AnimationControl.Play("ready");
                        ActionCooldown = 0.2f;
                        break;
                }
                //Debug.Log("Switched to State " + PState + " Velocity " + h + " " + v);





            }
        }
    }
    void OnTriggerEnter(Collider col)
    {
        if (col.tag == "PlayerSword")
        {
            if (Invincibility <= 0.0f && PState != State.Block)
            {

                Hit = true;
            }
            if(PState == State.Block)
            {
                PScript.Hit = true;
            }
           // Destroy(col.gameObject);
        }
    }
    IEnumerator Walk()
    {
        while (true)
        {
            m_PlayerForward = this.transform.forward;//Vector3.Scale(this.transform.forward, new Vector3(1, 0, 1)).normalized;
            m_Move = v * m_PlayerForward + h * this.transform.right;
            // Debug.Log(m_Move);
            Move(m_Move, false, m_Jump);
            m_Jump = false;
            yield return null;

        }



    }
}