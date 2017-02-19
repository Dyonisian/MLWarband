using UnityEngine;
using System.Collections;
using UnityStandardAssets.CrossPlatformInput;
using UnityEngine.UI;


public class PlayerScript : MonoBehaviour
{

    [SerializeField]
    protected float m_MovingTurnSpeed = 360;
    [SerializeField]
    protected float m_StationaryTurnSpeed = 180;
    [SerializeField]
    protected float m_JumpPower = 12f;
    [Range(1f, 4f)]
    [SerializeField]
    protected float m_GravityMultiplier = 2f;
    [SerializeField]
    protected float m_RunCycleLegOffset = 0.2f; //specific to the character in sample assets, will need to be modified to work with others
    [SerializeField]
    protected float m_MoveSpeedMultiplier = 1f;
    [SerializeField]
    protected float m_AnimSpeedMultiplier = 1f;
    [SerializeField]
    protected float m_GroundCheckDistance = 0.1f;

    [SerializeField]
    protected Rigidbody m_Rigidbody;
    //Animator m_Animator;
    protected bool m_IsGrounded;
    protected float m_OrigGroundCheckDistance;
    protected const float k_Half = 0.5f;
    protected float m_TurnAmount;
    protected Vector3 m_ForwardAmount;
    protected Vector3 m_GroundNormal;
    protected float m_CapsuleHeight;
    protected Vector3 m_CapsuleCenter;
    [SerializeField]

    protected CapsuleCollider m_Capsule;
    protected bool m_Crouching;
    protected bool m_Jump;
    protected bool ContinueCombo = false;
    protected bool Blocking = false;
    public bool Hit = false;
    protected Vector3 m_PlayerForward, m_Move, m_Direction;
    [SerializeField]

    protected Animator AnimationControl;


    public Animator SwordAnim;
    public AnimationClip Attack1;
    protected float AttackTime = 0.7f;
    protected float AttackCooldown = 0.0f;
    protected float DebugTimer = 0.0f;
    public float Invincibility = 0.0f;

    [SerializeField]
    protected float m_MoveBlock = 2.0f;
    [SerializeField]
    protected float m_MoveNormal = 4.0f;


    protected float h, v, h2, v2;

    public enum State { Idle, Walk, Jump, Attack, BlockUp, BlockDown, BlockLeft, BlockRight, Hit, Attack1, Attack2, Attack3, Attack4 };
    public enum Direction { Left, Right, Up, Down };


    public State PState;
    protected Direction PDirection;
    [SerializeField]
    protected Text StateText;
    [SerializeField]
    protected Text DirectionText;

    public GameObject Enemy;

    protected bool HumanPlayer;
    [SerializeField]

    EnemyScript EScript;
    // Use this for initialization

    public bool Warband = false;
    public GameObject Camera;

    [SerializeField]
    protected GameObject BlockLeft;

    [SerializeField]
    protected GameObject BlockRight;

    [SerializeField]
    protected GameObject BlockUp;

    [SerializeField]
    protected GameObject BlockDown;
    void Start()
    {
        PState = State.Idle;
        HumanPlayer = true;
        //m_Animator = GetComponent<Animator>();
        //m_Rigidbody = GetComponent<Rigidbody>();
        //m_Capsule = GetComponent<CapsuleCollider>();
        m_CapsuleHeight = m_Capsule.height;
        m_CapsuleCenter = m_Capsule.center;
        //AnimationControl = GetComponentInChildren<Animation>();
        //m_Rigidbody.constraints = RigidbodyConstraints.FreezeRotationX  | RigidbodyConstraints.FreezeRotationZ;
        m_OrigGroundCheckDistance = m_GroundCheckDistance;
        BlockLeft.SetActive(false);
        BlockRight.SetActive(false);
        BlockUp.SetActive(false);
        BlockDown.SetActive(false);
        PDirection = Direction.Up;
    }

    // Update is called once per frame
    void Update()
    {
        if (!m_Jump)
        {
            if (PState != State.Attack1 && PState != State.Attack2 && PState != State.Attack3 && PState != State.BlockUp && PState != State.BlockDown && PState != State.BlockLeft && PState != State.BlockRight && PState != State.Attack4 && PState != State.Hit)
                m_Jump = CrossPlatformInputManager.GetButtonDown("Jump");

        }

    }
    void FixedUpdate()
    {
       

        //StateText.text = PState.ToString();
        DirectionText.text = PDirection.ToString();
        DebugTimer += Time.deltaTime;
        Invincibility -= Time.deltaTime;
        if (AnimationControl.GetCurrentAnimatorStateInfo(0).IsName("Attack4") )
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
        if (AnimationControl.GetCurrentAnimatorStateInfo(0).IsName("Idle") && AnimationControl.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.00f && !AnimationControl.IsInTransition(0))
        {
            PState = State.Idle;
            if (AnimationControl.GetBool("Hit"))
            {
                AnimationControl.SetBool("Hit", false);
                Invincibility = 0.2f;
            }

        }

        if (Hit)
        {
            PState = State.Hit;
            if (gameObject.CompareTag("Player"))
            {
                BlockUp.SetActive(false);
                BlockDown.SetActive(false);
                BlockLeft.SetActive(false);
                BlockRight.SetActive(false);
            }
            AnimationControl.Play("Hit");
            AnimationControl.SetBool("Hit",true);

            StopAllCoroutines();
            //StartCoroutine(IsHit());
            Hit = false;


        }
        if (PState != State.Hit)
        {
            h = CrossPlatformInputManager.GetAxis("Horizontal");
            v = CrossPlatformInputManager.GetAxis("Vertical");
            h2 = 0;
            v2 = 0;

            h2 = CrossPlatformInputManager.GetAxisRaw("Horizontal");
            v2 = CrossPlatformInputManager.GetAxisRaw("Vertical");

            if (Warband)
            {
                if (v2>0)
                {
                    PDirection = Direction.Up;
                }
                if (v2<0)
                {
                    PDirection = Direction.Down;
                }
                if (h2<0)
                {
                    PDirection = Direction.Left;
                }
                if (h2>0)
                {
                    PDirection = Direction.Right;
                }
            }
            StateText.text = PState.ToString();
            
            if (Input.GetButton("Fire2"))
            {
                ContinueCombo = false;
                Blocking = true;
                if (PState == State.Idle || PState == State.Walk)
                {
                    
                    AnimationControl.Play("Block");
                    AnimationControl.SetBool("Block",true);
                    AnimationControl.SetBool("Attack4", false);
                    AnimationControl.SetBool("Attack3", false);
                    AnimationControl.SetBool("Attack2", false);
                    AnimationControl.SetBool("Attack1", false);





                    //StartCoroutine("PauseAnimation");
                    switch (PDirection)
                    {
                        case Direction.Up:
                            BlockUp.SetActive(true);
                            PState = State.BlockUp;
                            break;
                        case Direction.Down:
                            BlockDown.SetActive(true);
                            PState = State.BlockDown;

                            break;
                        case Direction.Left:
                            BlockLeft.SetActive(true);
                            PState = State.BlockLeft;

                            break;
                        case Direction.Right:
                            BlockRight.SetActive(true);
                            PState = State.BlockRight;

                            break;
                        default:
                            BlockUp.SetActive(true);
                            break;
                    }
                    m_MoveSpeedMultiplier = m_MoveBlock;
                }

            }

            else
            {
                Blocking = false;
                //if (PState == State.Block)
                {
                    //PState = State.Idle;
                    AnimationControl.SetBool("Block", false);

                    m_MoveSpeedMultiplier = m_MoveNormal;
                    BlockUp.SetActive(false);
                    BlockDown.SetActive(false);
                    BlockLeft.SetActive(false);
                    BlockRight.SetActive(false);


                }
            }
            if (Input.GetButtonDown("Fire1"))
            {
                DebugTimer = 0.0f;
                if (PState == State.Idle || PState == State.Walk || PState == State.Jump || PState == State.BlockUp || PState == State.BlockDown || PState == State.BlockLeft || PState == State.BlockRight || PState == State.Attack1 || PState == State.Attack2 || PState == State.Attack3 || PState == State.Attack4)
                {
                    StopAllCoroutines();
                    if (Warband)
                    {
                        WarbandAttack();
                    }
                    else
                    {
                        AnimationControl.Play("Attack1");
                        SwordAnim.Play("Attack1");
                        /*
                        if (PState == State.Block)
                        {
                            Blocking = false;
                        }
                        */
                        PState = State.Attack1;


                        //m_MoveSpeedMultiplier = 1.0f;
                    }
                }
                
            }




            if (PState != State.Attack1 && PState != State.Attack2 && PState != State.Attack3 && PState != State.Attack4 && PState != State.BlockUp && PState != State.BlockDown && PState != State.BlockLeft && PState != State.BlockRight)
            {
                if (v == 0 && h == 0)
                {
                    AnimationControl.Play("Idle");

                    PState = State.Idle;
                }
                else
                {
                    
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
                        

                    PState = State.Walk;
                }
            }
        }
        else //hit state
        {
            h = 0;
            v = 0;
        }

        // v = 1.0f;
        m_PlayerForward = this.transform.forward;
        Vector3.Scale(transform.forward, new Vector3(1, 0, 1));
        m_Move = v * m_PlayerForward + h * this.transform.right;
        // Debug.Log(m_Move);
        Move(m_Move, false, m_Jump);
        m_Jump = false;

        // m_Rigidbody.velocity = transform.right * 2.0f;



    }
    public void Move(Vector3 move, bool crouch, bool jump)
    {

        // convert the world relative moveInput vector into a local-relative
        // turn amount and forward amount required to head in the desired
        // direction.
        if (move.magnitude > 1f) move.Normalize();
        //move = transform.InverseTransformDirection(move);
        CheckGroundStatus();
        move = Vector3.ProjectOnPlane(move, m_GroundNormal);
        //m_TurnAmount = Mathf.Atan2(move.x, move.z);
        m_ForwardAmount = move * m_MoveSpeedMultiplier;

        //ApplyExtraTurnRotation();

        // control and velocity handling is different when grounded and airborne:
        if (m_IsGrounded)
        {
            HandleGroundedMovement(crouch, jump);
        }
        else
        {
            HandleAirborneMovement();
        }

        //ScaleCapsuleForCrouching(crouch);
        //PreventStandingInLowHeadroom();

        // send input and other state parameters to the animator
        //UpdateAnimator(move);
    }
    protected void CheckGroundStatus()
    {
        RaycastHit hitInfo;
#if UNITY_EDITOR
        // helper to visualise the ground check ray in the scene view
        Debug.DrawLine(transform.position + (Vector3.up * 0.1f), transform.position + (Vector3.up * 0.1f) + (Vector3.down * m_GroundCheckDistance));
#endif
        // 0.1f is a small offset to start the ray from inside the character
        // it is also good to note that the transform position in the sample assets is at the base of the character
        if (Physics.Raycast(transform.position + (Vector3.up * 0.1f), Vector3.down, out hitInfo, m_GroundCheckDistance))
        {
            m_GroundNormal = hitInfo.normal;
            m_IsGrounded = true;
            //m_Animator.applyRootMotion = true;
        }
        else
        {
            m_IsGrounded = false;
            m_GroundNormal = Vector3.up;
            // m_Animator.applyRootMotion = false;
        }
    }
    protected void HandleAirborneMovement()
    {
        // apply extra gravity from multiplier:
        Vector3 extraGravityForce = (Physics.gravity * m_GravityMultiplier) - Physics.gravity;
        m_Rigidbody.AddForce(extraGravityForce);

        m_GroundCheckDistance = m_Rigidbody.velocity.y < 0 ? m_OrigGroundCheckDistance : 0.01f;
        if (HumanPlayer)
            PState = State.Jump;
        m_Rigidbody.velocity = new Vector3(m_ForwardAmount.x, m_Rigidbody.velocity.y, m_ForwardAmount.z);

    }


    protected void HandleGroundedMovement(bool crouch, bool jump)
    {
        // check whether conditions are right to allow a jump:
        if (jump && !crouch && m_IsGrounded)
        {
            // jump!
            Vector3 v;
            v = transform.right * h;
            m_Rigidbody.velocity = new Vector3(m_Rigidbody.velocity.x, m_JumpPower, m_Rigidbody.velocity.z);
            m_IsGrounded = false;
            // m_Animator.applyRootMotion = false;
            m_GroundCheckDistance = 0.1f;
        }
        //Move
        m_Rigidbody.velocity = new Vector3(m_ForwardAmount.x, m_Rigidbody.velocity.y, m_ForwardAmount.z);


    }
    /*
    protected IEnumerator AttackCombo()
    {
        while (true)
        {
            if (PState == State.Attack1)
            {
                
                SwordAnim.speed = 0.7f;
                //Debug.Log("Waiting at attack1");
               // while (AnimationControl.IsPlaying("attack 1"))
                    yield return new WaitForFixedUpdate();
                //yield return new WaitForSeconds(AttackTime);
                //if (!AnimationControl.IsPlaying("attack 1"))
                
                {
                    if (!ContinueCombo)
                    {
                        if (!Blocking)
                        {
                            PState = State.Idle;
                            //m_MoveSpeedMultiplier = 2.0f;
                        }
                        else
                        {
                            PState = State.Block;
                            AnimationControl.Play("block");
                            StartCoroutine("PauseAnimation");

                        }
                    }
                    else
                    {
                        if (Warband)
                            WarbandAttack();
                        else
                        {
                            PState = State.Attack2;
                            AnimationControl.Play("attack 2");

                            SwordAnim.Play("attack 2");

                            ContinueCombo = false;
                        }
                    }
                }
            }
            if (PState == State.Attack2)
            {
                // Debug.Log("Waiting at attack2");
                //AnimationControl["attack 2"].speed = 0.7f;
                SwordAnim.speed = 0.7f;
               // while (AnimationControl.IsPlaying("attack 2"))
                    yield return new WaitForFixedUpdate();
                //yield return new WaitForSeconds(AttackTime);
                //Debug.Log("Not waiting!");

                //                if (!AnimationControl.IsPlaying("attack 2"))
                {
                    if (!ContinueCombo)
                    {
                        if (!Blocking)
                        {
                            PState = State.Idle;
                            //m_MoveSpeedMultiplier = 2.0f;
                        }
                        else
                        {
                            PState = State.Block;
                            AnimationControl.Play("block");
                            StartCoroutine("PauseAnimation");
                        }
                    }

                    else
                    {
                        if (Warband)
                            WarbandAttack();

                        else
                        {
                            PState = State.Attack3;
                            AnimationControl.Play("attack 3");
                            SwordAnim.Play("attack 3");
                            ContinueCombo = false;
                        }
                    }
                }
            }
            if (PState == State.Attack3)
            {
                //Debug.Log("Waiting at attack3");
                //AnimationControl["attack 3"].speed = 0.7f;
                SwordAnim.speed = 0.7f;
                //while (AnimationControl.IsPlaying("attack 3"))
                    yield return new WaitForFixedUpdate();
                // yield return new WaitForSeconds(AttackTime);

                //                if (!AnimationControl.IsPlaying("attack 3"))
                if (Warband && !ContinueCombo)
                {
                    if (!Blocking)
                    {
                        PState = State.Idle;
                        //m_MoveSpeedMultiplier = 2.0f;
                    }
                    else
                    {
                        PState = State.Block;
                        AnimationControl.Play("block");
                        StartCoroutine("PauseAnimation");
                    }
                }
                else if (Warband)
                {
                    WarbandAttack();

                }
            }
            if (PState == State.Attack4)
            {
                //Debug.Log("Coroutine attack 4");
               // AnimationControl["attack 6"].speed = 0.7f;
                SwordAnim.speed = 0.7f;
                //while (AnimationControl.IsPlaying("attack 6") && AnimationControl["attack 6"].time <=1.0)
                {
                    yield return new WaitForFixedUpdate();
                }
                 yield return new WaitForSeconds(0.5f);

                //                if (!AnimationControl.IsPlaying("attack 3"))
                if (Warband && !ContinueCombo)
                {
                    if (!Blocking)
                    {
                        PState = State.Idle;
                        //m_MoveSpeedMultiplier = 2.0f;
                    }
                    else
                    {
                        PState = State.Block;
                        AnimationControl.Play("block");
                        StartCoroutine("PauseAnimation");
                    }
                }
                else if (Warband)
                {
                    WarbandAttack();

                }
            }
            yield return null;

        }

    }
    
    protected IEnumerator PauseAnimation()
    {
        Debug.Log("Pausing animation");
        yield return new WaitForSeconds(0.3f);
        if (PState == State.Block)
            AnimationControl.Stop();
        yield return null;
    }
    */
    virtual protected IEnumerator IsHit()
    {
        yield return new WaitForSeconds(0.8f);
        PState = State.Idle;
       

        yield return null;
    }

    void OnTriggerEnter(Collider col)
    {
        if (col.tag == "EnemySword")
        {
            Debug.Log("Im Invincible!");
            if (Invincibility <= 0.0f && (EScript.PState==State.Attack1|| EScript.PState == State.Attack2|| EScript.PState == State.Attack3|| EScript.PState == State.Attack4))
            {
                Debug.Log("Im hit!");
                Hit = true;
            }
            /*
            if (PState == State.Block)
            {
                EScript.Hit = true;
                //EScript.StartCoroutine("")
            }
            */
            //Destroy(col.gameObject);
        }
    }
    public State GetState()
    {
        return PState;
    }
    public void WarbandAttack()
    {
        AnimationControl.SetBool("Attack4", false);
        AnimationControl.SetBool("Attack3", false);
        AnimationControl.SetBool("Attack2", false);
        AnimationControl.SetBool("Attack1", false);
        if (!Warband)
        {
            return;
        }
        else
        {
            switch (PDirection)
            {
                case Direction.Up:
                    PState = State.Attack3;
                    AnimationControl.SetBool("Attack3", true);
                    ContinueCombo = false;
                    break;
                case Direction.Down:
                    PState = State.Attack4;
                    AnimationControl.SetBool("Attack4", true);

                    ContinueCombo = false;
                    break;
                case Direction.Left:
                    PState = State.Attack2;
                    AnimationControl.SetBool("Attack2", true);

                    ContinueCombo = false;

                    break;
                case Direction.Right:
                    PState = State.Attack1;
                    AnimationControl.SetBool("Attack1", true);

                    ContinueCombo = false;

                    break;
                default:
                    break;
            }
        }
    }
}
