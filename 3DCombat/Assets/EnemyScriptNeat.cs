using UnityEngine;
using System.Collections;

public class EnemyScriptNeat : PlayerScript
{
    [SerializeField]
    public float ActionCooldown;
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

    public class LearningStateNeat
    {
        public PlayerScript.State PState;
        public float Distance;
    }

    public NeatScript NScript;
    public bool IsReinforcementLearning;
    bool LastStateWasAttack;
    bool IsReacting;
    bool ReactionMode;

    public bool IsNEAT;
    float InitialCooldown;

    public int MyHits, MyAttacks, MyBlocks;
    public int OpponentHits, OpponentAttacks, OpponentBlocks;
    bool TryingDodge;
    // Use this for initialization
    void Start()
    {
        PState = State.Idle;
        IsReacting = false;
        ReactionMode = false;
        TryingDodge = false;
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
    void Update()
    {

    }
    void FixedUpdate()
    {
        //Give a short cooldown for enemy and scripts to be assigned, required during training
        InitialCooldown -= Time.deltaTime;
        if (InitialCooldown > 0.0f)
            return;


        DebugTimer += Time.deltaTime;
        Invincibility -= Time.deltaTime;
        ActionCooldown -= Time.deltaTime;

        //Keeping a record of last and current animation, not used currently
        if (AnimationControl.GetCurrentAnimatorStateInfo(0).fullPathHash != CurrentAnimation)
        {
            LastAnimation = CurrentAnimation;
            CurrentAnimation = AnimationControl.GetCurrentAnimatorStateInfo(0).fullPathHash;
        }

        //Set correct PStates and turn booleans false for animator to ensure only one boolean is true at a time
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
        //A hit or attack animation just ended, switched back to Idle. Set state back to idle
        if (AnimationControl.GetCurrentAnimatorStateInfo(0).IsName("Idle") && AnimationControl.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.0f && !AnimationControl.IsInTransition(0))
        {
            PState = State.Idle;
            SetACOnce = true;
            if (AnimationControl.GetBool("Hit"))
            {
                AnimationControl.SetBool("Hit", false);
                Invincibility = 0.1f;
                //Sets cooldown back to 0 as an attack animation ended and another action is possible now
                if (SetOnce)
                {
                    ActionCooldown = 0.0f;

                    SetACOnce = true;
                    SetOnce = false;
                }
            }

        }
        //Hit by opponent
        if (Hit)
        {
            PState = State.Hit;
            m_MoveSpeedMultiplier = m_MoveNormal;
            BlockUp.SetActive(false);
            BlockDown.SetActive(false);
            BlockLeft.SetActive(false);
            BlockRight.SetActive(false);

            AnimationControl.Play("Hit");
            AnimationControl.SetBool("Hit", true);
            AnimationControl.SetBool("Block", false);

            StopAllCoroutines();
            //StartCoroutine("IsHit");
            h = 0;
            v = 0;
            StartCoroutine("Walk");
            ActionCooldown = 3.0f;
            if (transform.root.CompareTag("Enemy"))
                Hit = false;
            Invincibility = 0.1f;
            //This is for the fitness of the neural network, for NEAT
            OpponentHits++;
            if(TryingDodge)
            {
                TryingDodge = false;
                OpponentAttacks -= 2;
            }
        }
    }
    public void UpdateForNeat(int RState, float hSpeed, float vSpeed)
    {
        //Some rules for allowing the AI to be reactionary - Used with Reinforcement Learning, not with NEAT
        //Reacts when opponent comes within range
        //If in reactionary mode, will not choose an action - waits for the opponent to do something that causes a reaction             
        /*
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
        if (SetACOnce && PState != State.Attack1 && PState != State.Attack2 && PState != State.Attack3 && PState != State.Attack4 && (OpponentScript.GetState() == State.Attack1 || OpponentScript.GetState() == State.Attack2 || OpponentScript.GetState() == State.Attack3 || OpponentScript.GetState() == State.Attack4))
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
        /*
        float rdm = Random.Range(0.0f, 100.0f);
        if (ActionCooldown <= 0.0f && !IsReacting && rdm > 25.0f && IsReinforcementLearning && !ReactionMode)
        {
            ActionCooldown = 4.0f;
            h = 0;
            v = 0;
            ReactionMode = true;
            IsSeeking = true;

        }*/

        //Action is possible, take the action. RState is selected by the Neural Network
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

            PlayerScript.State tempState = OpponentScript.GetState();
            TryingDodge = false;
            h = hSpeed;
            v = vSpeed;
            //Perform the correct action and animation. Set ActionCooldown according to length of animation or duration of action.
            switch (RState)
            {
                //idle
                case 0:
                    PState = State.Idle;                   
                    AnimationControl.Play("Idle");
                    ActionCooldown = 1.0f;
                    StartCoroutine(ResetSetACOnce(0.2f));
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

                    break;
                //walk, or wander
                case 1:
                    PState = State.Walk;
                    //Random magnitude of movement                   
                    //Random rotation target, so the AI faces a random direction
                    Vector3 RandomTarget;
                    RandomTarget.x = Random.Range(0, 100);
                    RandomTarget.y = 0;
                    RandomTarget.z = Random.Range(0, 100);
                    TargetRotation = Quaternion.LookRotation(RandomTarget);

                    //Play the correct animation based on movement magnitude
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

                    ActionCooldown = 0.5f;
                    //ResetSetACOnce allows a reaction to be taken again after x seconds, allowing for an earlier reaction if enemy comes in range
                    StartCoroutine(ResetSetACOnce(0.2f));

                    if (tempState == PlayerScript.State.Attack1 || tempState == PlayerScript.State.Attack2 || tempState == PlayerScript.State.Attack3 || tempState == PlayerScript.State.Attack4)
                    {
                        OpponentAttacks += 3;
                        TryingDodge = true;
                    }
                    break;
                //Jump action is not used anymore
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

                    ActionCooldown = 0.5f;
                    StartCoroutine(ResetSetACOnce(0.2f));

                    break;
                //Case 3,4,5,6 are directional attacks
                case 3:

                    //Set Blocking to false, set the correct State, play the correct animation
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
                    }
                    ActionCooldown = 2.6f;
                    MyAttacks++;
                    StartCoroutine(ResetSetACOnce(ActionCooldown));
                    break;

                case 4:
                    //Set Blocking to false, set the correct State, play the correct animation

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
                    
                    MyAttacks++;
                    StartCoroutine(ResetSetACOnce(ActionCooldown));
                    break;
                case 5:
                    //Set Blocking to false, set the correct State, play the correct animation

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
                    MyAttacks++;
                    StartCoroutine(ResetSetACOnce(ActionCooldown));
                    break;
                case 6:
                    //Set Blocking to false, set the correct State, play the correct animation

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
                    MyAttacks++;
                    StartCoroutine(ResetSetACOnce(ActionCooldown));
                    break;
                //Cases 7,8,9,10 are directional blocks
                case 7:
                    //Interrupt combo, only relevant for old combat system
                    ContinueCombo = false;
                    //Select correct state and animation
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

                    ActionCooldown = 0.5f;
                    StartCoroutine(ResetSetACOnce(ActionCooldown));

                    break;
                case 8:
                    //Select correct state and animation
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
                    ActionCooldown = 0.5f;
                    StartCoroutine(ResetSetACOnce(ActionCooldown));

                    break;
                case 9:
                    //Select correct state and animation
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
                    ActionCooldown = 0.5f;
                    StartCoroutine(ResetSetACOnce(ActionCooldown));

                    break;
                case 10:
                    //Select correct state and animation
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
                    ActionCooldown = 0.5f;
                    StartCoroutine(ResetSetACOnce(ActionCooldown));

                    break;

                //Seek - Sets IsSeeking to true, moves forward while facing opponent
                //Select correct state and animation
                case 11:
                    PState = State.Walk;
                    TargetRotation = SeekRotation;
                    h = 0;
                    v = 1;
                    IsSeeking = true;
                    AnimationControl.Play("Run");
                    m_Jump = false;
                    ActionCooldown = 0.5f;
                    StartCoroutine(ResetSetACOnce(0.2f));

                   
                    break;
                //Evade - Sets IsSeeking to true, moves backwards while facing opponent
                //Select correct state and animation
                case 12:
                    PState = State.Walk;

                    TargetRotation = SeekRotation;
                    h = 0;
                    v = -1;
                    IsSeeking = true;


                    AnimationControl.Play("BackPedal");


                    m_Jump = false;

                    ActionCooldown = 2.0f;
                    StartCoroutine(ResetSetACOnce(0.5f));

                    if (tempState == PlayerScript.State.Attack1 || tempState == PlayerScript.State.Attack2 || tempState == PlayerScript.State.Attack3 || tempState == PlayerScript.State.Attack4)
                    {
                        OpponentAttacks += 3;
                        TryingDodge = true;
                    }

                    break;
                default:
                    PState = State.Idle;
                    h = 0;
                    v = 0;
                    AnimationControl.Play("Idle");
                    ActionCooldown = 0.2f;
                    break;
            }
           
            
                
                
            
            
            //Booleans used by ReinforcementLearning AI 
            IsReacting = false;
            ReactionMode = false;
        }
    }
    //Gets the data required for NEAT Learning, and builds a LearningStateNeat for it
    public LearningStateNeat BuildLearningState()
    {
        m_Direction = Enemy.transform.position - transform.position;
        CurrentDistance = m_Direction.magnitude;
        SeekRotation = Quaternion.LookRotation(m_Direction);
        //CanHit is used by ReinforcementLearning
        //NEAT uses distance between AI and Opponent
        if (m_Direction.magnitude < HitDistance)
        {
            CanHit = true;

        }
        else
        {
            CanHit = false;
        }
        LearningStateNeat LS = new LearningStateNeat();
        LS.Distance = CurrentDistance;
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
            m_PlayerForward = this.transform.forward;
            m_Move = v * m_PlayerForward + h * this.transform.right;
            //If IsSeeking, face the opponent
            if (IsSeeking)
            {
                m_Direction = Enemy.transform.position - transform.position;
                CurrentDistance = m_Direction.magnitude;
                SeekRotation = Quaternion.LookRotation(m_Direction);
                TargetRotation = SeekRotation;
               // Debug.Log("Seeking!");
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
    //Used by ReinforcementLearning AI, allows a reaction to be taken while a movement action is being taken
    public IEnumerator ResetSetACOnce(float time)
    {
        yield return new WaitForSeconds(time);
        SetACOnce = true;
        yield return null;
    }
    //Change MyHits for NEAT fitness
    public void ChangeMyHits(int h)
    {
        NScript.MyHits = NScript.MyHits + h;
    }
    public void ChangeMyAttacks(int h)
    {
        NScript.MyAttacks = NScript.MyAttacks + h;
    }
    public void ChangeMyBlocks(int h)
    {
        NScript.MyBlocks = NScript.MyBlocks + h;
    }
    //Change OpponentHits for NEAT fitness
    public void ChangeOpponentHits(int h)
    {
        NScript.OpponentHits = NScript.OpponentHits + h;

    }
    public void ChangeOpponentAttacks(int h)
    {
        NScript.OpponentAttacks = NScript.OpponentAttacks + h;

    }
    public void ChangeOpponentBlocks(int h)
    {
        NScript.OpponentBlocks = NScript.OpponentBlocks + h;

    }
}
