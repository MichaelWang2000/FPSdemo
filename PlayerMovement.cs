using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    private CharacterController characterController; 
    public float walkSpeed = 10f; //行走速度    
    private float speed;//移动速度

    public float runSpeed = 15f; //奔跑速度
    public bool isRun;//判断是否在奔跑
    public bool isWalk;//是否在行走

    public float jumpForce = 3f;//跳跃的力度
    public Vector3 velocity;//设置玩家向上的模拟力变化
    public float gravity = -9f;//设置重力
    private bool isJump;//判断是否跳跃

    private Transform groundCheck;//地面检测
    private float groundDistance = 0.1f;//与地面的距离
    public LayerMask  groundMash;
    private bool isGround;


    public Vector3 moveDirection;//设置移动方向



    [SerializeField]private float slopeForce = 6.0f; //走斜坡时施加的力度
    [SerializeField]private float slopeForceRayLength = 2.0f; //斜坡射线长度

    [Header("声音设置")]
    [SerializeField]private AudioSource audioSource;
    public AudioClip walkingSound;
    public AudioClip runingSound;

    [Header("键位设置")]
    [SerializeField][Tooltip("奔跑按键")]private KeyCode runInputName; //奔跑键位
    [SerializeField][Tooltip("跳跃按键")]private KeyCode jumpInputName; //跳跃键位

    private void Start()
    {
        /*获取player身上的Controller组件*/
        characterController = GetComponent<CharacterController>();
        audioSource = GetComponent<AudioSource>();
        groundCheck = GameObject.Find("Player/CheckGround").GetComponent<Transform>();


        runInputName = KeyCode.LeftShift;
        jumpInputName = KeyCode.Space;
    }
   
   private void Update()
   {
    CheckGround();
    Move();
    PlayFootStepSound();
   }

   public void Move()  //角色移动
   {
    //两个水平轴值
    float h = Input.GetAxis("Horizontal");
    float v = Input.GetAxis("Vertical");

    isRun =  Input.GetKey(runInputName);
    if(!isRun)
    {
    isWalk = (Mathf.Abs(h)>0 || Mathf.Abs(v) > 0)? true:false;
    }
   

    speed = isRun? runSpeed:walkSpeed; //设计行走或奔跑的速度 

    //设置玩家移动方向
    moveDirection = (transform.right * h + transform.forward * v).normalized; //设置玩家移动方向
    characterController.Move(moveDirection*speed*Time.deltaTime);

    

    if(isGround == false)   //不在地面给一个累加的重力
    {
        velocity.y += gravity * Time.deltaTime;
    }
     
    characterController.Move(velocity*Time.deltaTime);

    Jump();

    //如果在斜坡上移动
    if(Onslope())
    {
        //向下增加力
        characterController.Move(Vector3.down*characterController.height/2*slopeForce*Time.deltaTime);
    }
   }


    //跳跃
   public void Jump()
   {
    isJump = Input.GetKey(jumpInputName);
    if(isJump && isGround)
    {
        velocity.y = Mathf.Sqrt(jumpForce * -2f * gravity);
    }
   }

    //地面检测
   public void CheckGround()
   {
    //判断是否在地面上
    isGround = Physics.CheckSphere(groundCheck.position,groundDistance,groundMash);
    //在地面上给一个向下的力
    if(isGround && velocity.y <= 0)
    {
        velocity.y = -2f;
    }
   }

    //判断是否在斜坡上
    public bool Onslope()
    {
        if(isJump)
            return false;

        RaycastHit hit;
        //向下打出射线判断是否在斜坡上
        if(Physics.Raycast(transform.position,Vector3.down,out hit,characterController.height/2 * slopeForceRayLength))
        {
            //接触到的点发现不在（0，1，0）方向上，就在斜坡上
            if(hit.normal != Vector3.up)
            {
                return true;
            }
        }
        return false;
    }

    //播放音效
    public void PlayFootStepSound()
    {
        if(isGround && (moveDirection.sqrMagnitude > 0.9f))
        {
            audioSource.clip = isRun? runingSound : walkingSound; //设置行走或奔跑音效
            if(!audioSource.isPlaying)
            {
               audioSource.Play(); 
            }
        }
        else
        {
            if(audioSource.isPlaying)
            {
               audioSource.Stop(); 
            }
        }
    }
}
