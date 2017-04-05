using UnityEngine;
using System.Collections;

public class EnemyScript : PlayerScript {
    bool aAttack, aLeft, aRight, aForward, aBack, aJump, aBlock;
    bool InRange;
    [SerializeField]
    float ActionCooldown;
    bool SetOnce;
    int RState = 0;
    [SerializeField]

    public PlayerScript OpponentScript;

    public bool IsSeeking = false;
    Vector3 SeekDirection;

    Quaternion TargetRotation, SeekRotation;

    public float HitDistance;
    public float CurrentDistance = 0.0f;
    public bool CanHit = false;
    int CurrentAnimation, LastAnimation;

     bool SetACOnce = false;
    
    public  struct LearningState
    {
        public PlayerScript.State PState;
        public bool CanHit;
    }

    public ReinforcementLearning RLScript;
    public ReinforcementLearningMove RLMoveScript;
    public bool IsReinforcementLearning;
    bool LastStateWasAttack;
    bool IsReacting;
    bool ReactionMode;
    float InitialCooldown;
    public bool IsNEAT;
	// Use this for initialization
	void Start () {
        InitialCooldown = 0.5f;
        PState = State.Idle;
        IsReacting = false;
        ReactionMode = false;
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
        HitDistance = 1.91f;
        CurrentAnimation = -2000;
        LastAnimation = -2000;
        SetOnce = false;
        LastStateWasAttack = false;
    }
	
	// Update is called once per frame
	void Update () {
        
    }
    void FixedUpdate()
    {
        InitialCooldown -= Time.deltaTime;
        if (InitialCooldown > 0.0f)
            return;
        if (IsNEAT)
            return;
        if (Enemy == null)
            Destroy(gameObject);

        //StateText.text = PState.ToString();
        DebugTimer += Time.deltaTime;
        Invincibility -= Time.deltaTime;
        ActionCooldown -= Time.deltaTime;

        //Keeping a record of last and current animation
        if (AnimationControl.GetCurrentAnimatorStateInfo(0).fullPathHash != CurrentAnimation)
        {
            LastAnimation = CurrentAnimation;
            CurrentAnimation = AnimationControl.GetCurrentAnimatorStateInfo(0).fullPathHash;
        }

        if (AnimationControl.GetCurrentAnimatorStateInfo(0).IsName("Attack4"))
        {
            PState = State.Attack4;
            LastStateWasAttack = true;
            AnimationControl.SetBool("Attack4", false);
        }
        if (AnimationControl.GetCurrentAnimatorStateInfo(0).IsName("Attack3"))
        {
            PState = State.Attack3;
            LastStateWasAttack = true;
            AnimationControl.SetBool("Attack3", false);
        }
        if (AnimationControl.GetCurrentAnimatorStateInfo(0).IsName("Attack2"))
        {
            PState = State.Attack2;
            LastStateWasAttack = true;
            AnimationControl.SetBool("Attack2", false);
        }
        if (AnimationControl.GetCurrentAnimatorStateInfo(0).IsName("Attack1"))
        {
            PState = State.Attack1;
            LastStateWasAttack = true;
            AnimationControl.SetBool("Attack1", false);
        }
        if (AnimationControl.GetCurrentAnimatorStateInfo(0).IsName("Idle") && AnimationControl.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.0f && !AnimationControl.IsInTransition(0))
        {
            PState = State.Idle;
            SetACOnce = true;
            /*
            if(SetOnce)
            {
                ActionCooldown = 0.0f;

                SetACOnce = true;
                SetOnce = false;
            }
            */
            if (AnimationControl.GetBool("Hit"))
            {
                AnimationControl.SetBool("Hit", false);
                Invincibility = 0.3f;
                if (SetOnce)
                {
                    ActionCooldown = 0.0f;

                    SetACOnce = true;
                    SetOnce = false;
                }
            }

        }

        if (Hit)
        {
            PState = State.Hit;
            m_MoveSpeedMultiplier = m_MoveNormal;
            BlockUp.SetActive(false);
            BlockDown.SetActive(false);
            BlockLeft.SetActive(false);
            BlockRight.SetActive(false);
            
           
            AnimationControl.Play("Hit");
            AnimationControl.SetBool("Hit",true);
            AnimationControl.SetBool("Block", false);



            StopAllCoroutines();
            //StartCoroutine("IsHit");
            h = 0;
            v = 0;
            StartCoroutine("Walk");
            ActionCooldown = 3.0f;
            if(transform.root.CompareTag("Enemy"))
            if (IsReinforcementLearning)
            {
                RLGiveReward(-1.5f, OpponentScript.GetState(), CanHit);
            }

            Hit = false;


        }
        if (PState != State.Hit)
        {
            if (Enemy == null)
                Destroy(gameObject);
            m_Direction = Enemy.transform.position - transform.position;
            CurrentDistance = m_Direction.magnitude;
            SeekRotation = Quaternion.LookRotation(m_Direction);
            if (m_Direction.magnitude < (HitDistance + 1.5f) && SetACOnce && PState != State.Attack1 && PState != State.Attack2 && PState != State.Attack3 && PState != State.Attack4)
            {
                if (ActionCooldown > 0.2f)
                {
                    ActionCooldown = 0.2f;
                }
                SetACOnce = false;
                IsReacting = true;
               // Debug.Log("React!" + ActionCooldown);

            }
            if (m_Direction.magnitude < HitDistance)
            {
                CanHit = true;
                
            }
            else
            {
                CanHit = false;
            }
            //react when the player starts attacking
            if (SetACOnce && PState != State.Attack1 && PState != State.Attack2 && PState != State.Attack3 && PState != State.Attack4 && (OpponentScript.GetState()==State.Attack1|| OpponentScript.GetState() == State.Attack2|| OpponentScript.GetState() == State.Attack3|| OpponentScript.GetState() == State.Attack4))
            {
                if (ActionCooldown > 0.2f)
                {
                    ActionCooldown = 0.2f;
                }
                SetACOnce = false;
                IsReacting = true;
               // Debug.Log("React!");

            }
            //75% chance of taking a reactionary stance that'll only change on reaction, instead of taking a random action 
            float rdm = Random.Range(0.0f, 100.0f);
            if (ActionCooldown<=0.0f && !IsReacting && rdm>25.0f && IsReinforcementLearning&& !ReactionMode)
            {
                ActionCooldown = 4.0f;
                h = 0;
                v = 0;
                ReactionMode = true;
                IsSeeking = true;

            }

            //Debug.Log("Changing rotation");
            if (ActionCooldown <= 0.0f)
            {
                IsSeeking = false;
                //IsSeeking = true;
                SetOnce = true;
                
                AnimationControl.SetBool("Block", false);
                m_MoveSpeedMultiplier = m_MoveNormal;
                BlockUp.SetActive(false);
                BlockDown.SetActive(false);
                BlockLeft.SetActive(false);
                BlockRight.SetActive(false);

                if (IsReinforcementLearning)
                {
                    LearningState RLState = new LearningState();
                    RLState.CanHit = CanHit;
                    RLState.PState = OpponentScript.PState;
                    RState = RLScript.RLStep(RLState);
                }
                else
                {
                    RState = Random.Range(0, 101);
                    
                    if (CanHit)
                    {
                        if (RState > 50)
                        {
                            RState = Random.Range(3, 7);
                        }
                        else if (RState > 25)
                        {
                            RState = Random.Range(7, 11);
                        }
                        else
                        {
                            RState = 12;
                        }
                    }
                    
                    else
                    
                    {
                        if (RState > 50)
                        {
                            RState = 11;
                        }
                        else
                        {
                            RState = Random.Range(0, 13);
                        }
                    }
                }
                if(m_Direction.magnitude>15.0f)
                {
                    if(!IsReinforcementLearning || !IsReacting)
                    RState = 11;
                }
                //Debug only!
               // if (RState == 4)
                //    RState--;
                //Debug.Log("Switching to state" + RState);

                if(RState>=3&& RState<=6)
                {
                    if(!CanHit)
                    {
                        h = 0;
                        v = 1;
                    }
                   else
                    {
                        h = 0;
                        v = 0;
                    }
                }

                switch (RState)
                {
                    //idle
                    case 0:
                        PState = State.Idle;
                        h = 0;
                        v = 0;
                        AnimationControl.Play("Idle");
                        ActionCooldown = 1.0f;
                        StartCoroutine(ResetSetACOnce(0.2f));
                        break;
                    //walk
                    case 1:
                        PState = State.Walk;
                        h = Random.Range(-1.0f, 1.0f);
                        v = Random.Range(-1.0f, 1.0f);
                        Vector3 RandomTarget;
                        RandomTarget.x = Random.Range(0, 100);
                        RandomTarget.y = 0;
                        RandomTarget.z = Random.Range(0, 100);
                        TargetRotation = Quaternion.LookRotation(RandomTarget);

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
                        StartCoroutine(ResetSetACOnce(0.2f));

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
                        StartCoroutine(ResetSetACOnce(0.2f));

                        break;
                    //attack
                    case 3:
                        
                        if (PState == State.Idle || PState == State.Walk || PState == State.Jump || PState == State.BlockUp || PState == State.BlockDown || PState == State.BlockLeft || PState == State.BlockRight)
                        {
                            StopAllCoroutines();
                            StartCoroutine("Walk");
                            if (PState == State.BlockUp || PState == State.BlockDown || PState == State.BlockLeft || PState == State.BlockRight)
                                Blocking = false;

                            PState = State.Attack1;
                            AnimationControl.SetBool("Attack1", true);

                            ContinueCombo = false;


                            IsSeeking = true;
                            //m_MoveSpeedMultiplier = 1.0f;
                        }
                        

                        ActionCooldown = 2.6f;
                        StartCoroutine(ResetSetACOnce(ActionCooldown));
                        break;
                    case 4:
                        
                        if (PState == State.Idle || PState == State.Walk || PState == State.Jump || PState == State.BlockUp || PState == State.BlockDown || PState == State.BlockLeft || PState == State.BlockRight)
                        {
                            StopAllCoroutines();                          
                            StartCoroutine("Walk");
                            if (PState == State.BlockUp || PState == State.BlockDown || PState == State.BlockLeft || PState == State.BlockRight)
                                Blocking = false;

                            PState = State.Attack2;
                            AnimationControl.SetBool("Attack2", true);

                            ContinueCombo = false;
                            IsSeeking = true;


                            //m_MoveSpeedMultiplier = 1.0f;
                        }

                        ActionCooldown = 2.6f;
                        StartCoroutine(ResetSetACOnce(ActionCooldown));
                        break;
                    case 5:
                        
                        if (PState == State.Idle || PState == State.Walk || PState == State.Jump || PState == State.BlockUp || PState == State.BlockDown || PState == State.BlockLeft || PState == State.BlockRight)
                        {
                            StopAllCoroutines();
                            
                            StartCoroutine("Walk");
                            if (PState == State.BlockUp || PState == State.BlockDown || PState == State.BlockLeft || PState == State.BlockRight)
                                Blocking = false;
                            PState = State.Attack3;
                            AnimationControl.SetBool("Attack3", true);

                            ContinueCombo = false;

                            IsSeeking = true;

                            //m_MoveSpeedMultiplier = 1.0f;
                        }

                        ActionCooldown = 2.6f;
                        StartCoroutine(ResetSetACOnce(ActionCooldown));
                        break;
                    case 6:
                        
                        if (PState == State.Idle || PState == State.Walk || PState == State.Jump || PState == State.BlockUp || PState == State.BlockDown || PState == State.BlockLeft || PState == State.BlockRight)
                        {
                            StopAllCoroutines();                            
                            StartCoroutine("Walk");
                            if (PState == State.BlockUp || PState == State.BlockDown || PState == State.BlockLeft || PState == State.BlockRight)
                                Blocking = false;
                            PState = State.Attack4;
                            AnimationControl.SetBool("Attack4", true);
                            ContinueCombo = false;

                            IsSeeking = true;

                            //m_MoveSpeedMultiplier = 1.0f;
                        }

                        ActionCooldown = 2.6f;
                        StartCoroutine(ResetSetACOnce(ActionCooldown));
                        break;
                    //Block
                    case 7:
                        ContinueCombo = false;
                        Blocking = true;
                        if (PState == State.Idle || PState == State.Walk)
                        {
                            PState = State.BlockUp;
                            AnimationControl.Play("Block");
                            AnimationControl.SetBool("Block", true);
                            AnimationControl.SetBool("Attack4", false);
                            AnimationControl.SetBool("Attack3", false);
                            AnimationControl.SetBool("Attack2", false);
                            AnimationControl.SetBool("Attack1", false);
                            BlockUp.SetActive(true);

                            m_MoveSpeedMultiplier = m_MoveBlock;
                            IsSeeking = true;
                        }
                        
                        ActionCooldown = 2.0f;
                        StartCoroutine(ResetSetACOnce(ActionCooldown));

                        break;
                    case 8:
                        ContinueCombo = false;
                        Blocking = true;
                        if (PState == State.Idle || PState == State.Walk)
                        {
                            PState = State.BlockDown;
                            AnimationControl.Play("Block");
                            AnimationControl.SetBool("Block", true);
                            AnimationControl.SetBool("Attack4", false);
                            AnimationControl.SetBool("Attack3", false);
                            AnimationControl.SetBool("Attack2", false);
                            AnimationControl.SetBool("Attack1", false);
                            BlockDown.SetActive(true);

                            m_MoveSpeedMultiplier = m_MoveBlock;
                            IsSeeking = true;
                        }
                        ActionCooldown = 2.0f;
                        StartCoroutine(ResetSetACOnce(ActionCooldown));

                        break;
                    case 9:
                        ContinueCombo = false;
                        Blocking = true;
                        if (PState == State.Idle || PState == State.Walk)
                        {
                            PState = State.BlockLeft;
                            AnimationControl.Play("Block");
                            AnimationControl.SetBool("Block", true);
                            AnimationControl.SetBool("Attack4", false);
                            AnimationControl.SetBool("Attack3", false);
                            AnimationControl.SetBool("Attack2", false);
                            AnimationControl.SetBool("Attack1", false);
                            BlockLeft.SetActive(true);

                            m_MoveSpeedMultiplier = m_MoveBlock;
                            IsSeeking = true;
                        }
                        ActionCooldown = 2.0f;
                        StartCoroutine(ResetSetACOnce(ActionCooldown));

                        break;
                    case 10:
                        ContinueCombo = false;
                        Blocking = true;
                        if (PState == State.Idle || PState == State.Walk)
                        {
                            PState = State.BlockRight;
                            AnimationControl.Play("Block");
                            AnimationControl.SetBool("Block", true);
                            AnimationControl.SetBool("Attack4", false);
                            AnimationControl.SetBool("Attack3", false);
                            AnimationControl.SetBool("Attack2", false);
                            AnimationControl.SetBool("Attack1", false);
                            BlockRight.SetActive(true);

                            m_MoveSpeedMultiplier = m_MoveBlock;
                            IsSeeking = true;
                        }
                        ActionCooldown = 2.0f;
                        StartCoroutine(ResetSetACOnce(ActionCooldown));

                        break;
                        
                        //seek
                    case 11:
                        PState = State.Walk;
                       
                        TargetRotation = SeekRotation;
                        h = 0;
                        v = 1;
                        IsSeeking = true;

                       
                                AnimationControl.Play("Run");
                       

                        m_Jump = false;

                        ActionCooldown = 2.0f;
                        StartCoroutine(ResetSetACOnce(0.2f));
                        break;
                    case 12:
                        PState = State.Walk;

                        TargetRotation = SeekRotation;
                        h = 0;
                        v = -1;
                        IsSeeking = true;


                        AnimationControl.Play("BackPedal");


                        m_Jump = false;

                        ActionCooldown = 2.0f;
                        StartCoroutine(ResetSetACOnce(1.0f));

                        break;
                    default:
                        PState = State.Idle;
                        h = 0;
                        v = 0;
                        AnimationControl.Play("Idle");
                        ActionCooldown = 0.2f;
                        break;
                }
                IsReacting = false;
                ReactionMode = false;
                
                /*
                if (IsReinforcementLearning)
                {
                    LearningState RLMoveState = new LearningState();
                    RLMoveState.CanHit = CanHit;
                    RLMoveState.PState = (State)RState;
                    RState = RLMoveScript.RLStep(RLMoveState);
                }
                */

                //Debug.Log("Switched to State " + PState + " Velocity " + h + " " + v);





            }
        }
    }
    
    public LearningState BuildLearningState()
    {
        m_Direction = Enemy.transform.position - transform.position;
        CurrentDistance = m_Direction.magnitude;
        SeekRotation = Quaternion.LookRotation(m_Direction);    
        if (m_Direction.magnitude < HitDistance)
        {
            CanHit = true;

        }
        else
        {
            CanHit = false;
        }
        LearningState LS = new LearningState();
        LS.CanHit = CanHit;
        LS.PState = OpponentScript.PState;
        return LS;

    }
    /*
    void OnTriggerEnter(Collider col)
    {
        if (col.tag == "PlayerSword")
        {
            if (Invincibility <= 0.0f && (OpponentScript.PState==State.Attack1 || OpponentScript.PState == State.Attack2 || OpponentScript.PState == State.Attack3 || OpponentScript.PState == State.Attack4))
            {

                Hit = true;
                Invincibility = 0.1f;
                if (IsReinforcementLearning)
                {
                    RLGiveReward(-0.7f);
                }

            }
            
           
        }
    }
    */
    IEnumerator Walk()
    {
        while (true)
        {
            m_PlayerForward = this.transform.forward;//Vector3.Scale(this.transform.forward, new Vector3(1, 0, 1)).normalized;
            
            m_Move = v * m_PlayerForward + h * this.transform.right;
            if (IsSeeking)
            {
                TargetRotation = SeekRotation;

            }
            transform.rotation = TargetRotation;
            
            // Debug.Log(m_Move);
            Move(m_Move, false, m_Jump);
            m_Jump = false;
            yield return null;

        }



    }
    protected IEnumerator IsHit()
    {

        yield return new WaitForSeconds(0.8f);
        PState = State.Idle;
        
        //StartCoroutine("AttackCombo");
        ActionCooldown = 0.00f;
       
        yield return null;
    }
    public void RLGiveReward(float Reward, PlayerScript.State PState, bool CanHit)
    {
        LearningState LS = new LearningState();
        LS.PState = PState;
        LS.CanHit = CanHit;
        RLScript.UpdateQValues(Reward, LS);
    }
    public IEnumerator ResetSetACOnce(float time)
    {
        yield return new WaitForSeconds(time);
        SetACOnce = true;
        yield return null;
    }
}
