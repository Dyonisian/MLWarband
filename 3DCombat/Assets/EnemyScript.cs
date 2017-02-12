using UnityEngine;
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
        
        m_CapsuleHeight = m_Capsule.height;
        m_CapsuleCenter = m_Capsule.center;
        AnimationControl = GetComponent<Animator>();
        //m_Rigidbody.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY | RigidbodyConstraints.FreezeRotationZ;
        m_OrigGroundCheckDistance = m_GroundCheckDistance;
        //StartCoroutine("AttackCombo");
        StartCoroutine("Walk");
        HumanPlayer = false;
        BlockLeft.SetActive(false);
        BlockRight.SetActive(false);
        BlockUp.SetActive(false);
        BlockDown.SetActive(false);
        PDirection = Direction.Up;
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

        if (AnimationControl.GetCurrentAnimatorStateInfo(0).IsName("Attack4"))
        {
            PState = State.Attack4;
            AnimationControl.SetBool("Attack4", false);
        }
        if (AnimationControl.GetCurrentAnimatorStateInfo(0).IsName("Attack3"))
        {
            PState = State.Attack3;
            AnimationControl.SetBool("Attack3", false);
        }
        if (AnimationControl.GetCurrentAnimatorStateInfo(0).IsName("Attack2"))
        {
            PState = State.Attack2;
            AnimationControl.SetBool("Attack2", false);
        }
        if (AnimationControl.GetCurrentAnimatorStateInfo(0).IsName("Attack1"))
        {
            PState = State.Attack1;
            AnimationControl.SetBool("Attack1", false);
        }
        if (AnimationControl.GetCurrentAnimatorStateInfo(0).IsName("Idle") && AnimationControl.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.01f && !AnimationControl.IsInTransition(0))
        {
            PState = State.Idle;
        }

        if (Hit)
        {
            PState = State.Hit;
            m_MoveSpeedMultiplier = m_MoveNormal;
            BlockUp.SetActive(false);
            BlockDown.SetActive(false);
            BlockLeft.SetActive(false);
            BlockRight.SetActive(false);
            if (gameObject.CompareTag("Enemy"))
            {
                BlockUp.SetActive(false);
                BlockDown.SetActive(false);
                BlockLeft.SetActive(false);
                BlockRight.SetActive(false);
            }
           
            AnimationControl.Play("Hit");
            AnimationControl.SetBool("Hit",true);


            StopAllCoroutines();
            //StartCoroutine("IsHit");
            h = 0;
            v = 0;
            StartCoroutine("Walk");
            ActionCooldown = 3.0f;
            Hit = false;


        }
        if (PState != State.Hit)
        {
            m_Direction = Enemy.transform.position - transform.position;
            transform.rotation = Quaternion.LookRotation(m_Direction);

            if (ActionCooldown <= 0.0f)
            {
                AnimationControl.SetBool("Block", false);
                m_MoveSpeedMultiplier = m_MoveNormal;
                BlockUp.SetActive(false);
                BlockDown.SetActive(false);
                BlockLeft.SetActive(false);
                BlockRight.SetActive(false);
                RState = Random.Range(0, 8);
                //Debug only!
               // if (RState == 4)
                //    RState--;
                //Debug.Log("Switching to state" + RState);
                switch (RState)
                {
                    //idle
                    case 0:
                        PState = State.Idle;
                        h = 0;
                        v = 0;
                        AnimationControl.Play("Idle");
                        ActionCooldown = 1.0f;
                        break;
                    //walk
                    case 1:
                        PState = State.Walk;
                        h = Random.Range(-1.0f, 1.0f);
                        v = Random.Range(-1.0f, 1.0f);


                        if (Mathf.Abs(v) > Mathf.Abs(h))
                        {
                            if (v > 0)
                                AnimationControl.Play("Run");
                            else
                                AnimationControl.Play("BackPedal");
                        }
                        else
                        {
                            if (h > 0)
                                AnimationControl.Play("StrafeRight");
                            else
                                AnimationControl.Play("StrafeLeft");
                        }

                        m_Jump = false;

                        ActionCooldown = 2.0f;
                        break;
                    //jump
                    case 2:
                        PState = State.Jump;
                        h = Random.Range(-1.0f, 1.0f);
                        v = Random.Range(-1.0f, 1.0f);


                        if (Mathf.Abs(v) > Mathf.Abs(h))
                        {
                            if (v > 0)
                                AnimationControl.Play("Run");
                            else
                                AnimationControl.Play("BackPedal");
                        }
                        else
                        {
                            if (h > 0)
                                AnimationControl.Play("StrafeRight");
                            else
                                AnimationControl.Play("StrafeLeft");
                        }

                        m_Jump = true;

                        ActionCooldown = 2.0f;
                        break;
                    //attack
                    case 3:
                        h = 0;
                        v = 0;
                        if (PState == State.Idle || PState == State.Walk || PState == State.Jump || PState == State.Block)
                        {
                            StopAllCoroutines();
                            StartCoroutine("Walk");


                            PState = State.Attack1;
                            AnimationControl.SetBool("Attack1", true);

                            ContinueCombo = false;
                            if (PState == State.Block)
                                Blocking = false;


                            //m_MoveSpeedMultiplier = 1.0f;
                        }
                        
                        ActionCooldown = 0.6f;
                        break;
                    case 4:
                        h = 0;
                        v = 0;
                        if (PState == State.Idle || PState == State.Walk || PState == State.Jump || PState == State.Block)
                        {
                            StopAllCoroutines();                          
                            StartCoroutine("Walk");
                            PState = State.Attack2;
                            AnimationControl.SetBool("Attack2", true);

                            ContinueCombo = false;
                            if (PState == State.Block)
                                Blocking = false;


                            //m_MoveSpeedMultiplier = 1.0f;
                        }

                        ActionCooldown = 0.6f;
                        break;
                    case 5:
                        h = 0;
                        v = 0;
                        if (PState == State.Idle || PState == State.Walk || PState == State.Jump || PState == State.Block)
                        {
                            StopAllCoroutines();
                            
                            StartCoroutine("Walk");
                            PState = State.Attack3;
                            AnimationControl.SetBool("Attack3", true);

                            ContinueCombo = false;
                            if (PState == State.Block)
                                Blocking = false;


                            //m_MoveSpeedMultiplier = 1.0f;
                        }

                        ActionCooldown = 0.6f;
                        break;
                    case 6:
                        h = 0;
                        v = 0;
                        if (PState == State.Idle || PState == State.Walk || PState == State.Jump || PState == State.Block)
                        {
                            StopAllCoroutines();                            
                            StartCoroutine("Walk");
                            PState = State.Attack4;
                            AnimationControl.SetBool("Attack4", true);
                            ContinueCombo = false;
                            if (PState == State.Block)
                                Blocking = false;


                            //m_MoveSpeedMultiplier = 1.0f;
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
                            AnimationControl.Play("Block");
                            AnimationControl.SetBool("Block", true);
                            AnimationControl.SetBool("Attack4", false);
                            AnimationControl.SetBool("Attack3", false);
                            AnimationControl.SetBool("Attack2", false);
                            AnimationControl.SetBool("Attack1", false);
                            BlockUp.SetActive(true);

                            m_MoveSpeedMultiplier = m_MoveBlock;
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
            if (Invincibility <= 0.0f)
            {

                Hit = true;
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
    protected override IEnumerator IsHit()
    {

        yield return new WaitForSeconds(0.8f);
        PState = State.Idle;
        
        StartCoroutine("AttackCombo");
        ActionCooldown = 0.01f;
        yield return null;
    }
}
