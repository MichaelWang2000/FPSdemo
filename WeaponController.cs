using System.Collections;
using System.Collections.Generic;
using UnityEngine; //命名空间
using UnityEngine.UI;

//武器射击

public class WeaponController : MonoBehaviour
{
    public PlayerMovement PM;
    public Transform shooterPoint; //射击的位置
    public int bulletsMag = 31; //弹夹容量
    public int range = 100;//武器射程
    public int bulletLeft = 300;//武器备用子弹数
    public int currentBullets; //当前子弹数


    private bool GunShootInput;//判断是否按下鼠标左键射击

    public float fireRate = 0.1f;//射速,越小设计速度越快
    private float fireTimer;//计时器

    public ParticleSystem muzzleFlash;//枪口火焰特效
    public Light muzzleFlashLight;//枪口火焰灯光
    public GameObject hitParticle;//子弹击中粒子特效
    public GameObject bullectHole;//弹孔


    //音频参数
    private AudioSource audioSource;
    public AudioClip AK47SoundClip; //枪射击音效
    public AudioClip reloadAmmoLeftClip;//换子弹声音1
    public AudioClip reloadOutOfAmmoLeftClip;//换子弹声音1

    private bool isReload;//判断是否在装弹
    private bool isAiming;//判断是否在瞄准
    private Camera mainCamera;

    [Header("键位设置")]
    [SerializeField][Tooltip("换弹按键")]public KeyCode reloadInputName;
    [SerializeField][Tooltip("查看武器按键")]public KeyCode inspectInputName;

    [Header("UI设置")]
    public Image CrossHairUI;
    public Text AmmoTextUI;

    private Animator anim;

    // Start is called before the first frame update
    void Start()
    {
        mainCamera = Camera.main;
        audioSource = GetComponent<AudioSource>();
        anim = GetComponent<Animator>();
        currentBullets = bulletsMag;
        UpdateAmmoUI();
    }

    // Update is called once per frame
    void Update()
    {
        GunShootInput = Input.GetMouseButton(0);
        if(GunShootInput)
        {
            GunFire();
        }
        else
        {
           muzzleFlashLight.enabled = false; 
        }

        AnimatorStateInfo info = anim.GetCurrentAnimatorStateInfo(0);
        //两种换子弹的动画
        if(info.IsName("reload_out_of_ammo") || info.IsName("reload_ammo_left"))
        {
            isReload = true;
        }
        else{
            isReload = false; 
        }
        //判断是否换子弹
        if(Input.GetKeyDown(reloadInputName) && (currentBullets < bulletsMag) && (bulletLeft >0))
        {
            Reload();
        }

        DoingAim(); //瞄准

        //每帧减少射线的执行次数，从而实现控制
        if(fireTimer < fireRate)
        {
            fireTimer += Time.deltaTime;
        }

        if(Input.GetKeyDown(inspectInputName))
        {
            //查看武器动画
            anim.SetTrigger("Inspect");
        }

        anim.SetBool("Run",PM.isRun);
        anim.SetBool("Walk",PM.isWalk);
    }

    //射击
    public void GunFire()
    {
        if(fireTimer < fireRate || currentBullets <= 0 || isReload || PM.isRun) return; //控制射速，如果计时器值比射速还小或者当前子弹数不足，那么跳出方法

        RaycastHit hit;
        Vector3 shootDirection = shooterPoint.forward; //当前射击方向

        if(Physics.Raycast(shooterPoint.position,shootDirection,out hit,range))
        {
            Debug.Log(hit.transform.name+ "打到了");
            GameObject hitParticleEffect = Instantiate(hitParticle,hit.point,Quaternion.FromToRotation(Vector3.up,hit.normal)); //生成子弹击中的火光特效
            GameObject bullectHoleEffect = Instantiate(bullectHole,hit.point,Quaternion.FromToRotation(Vector3.up,hit.normal)); //生成弹孔特效

            //回收特效
            Destroy(hitParticleEffect,1f);
            Destroy(bullectHoleEffect,3f);
        }

        PlayerShootSound(); //播放武器声音
        muzzleFlash.Play(); //播放灯光特效
        muzzleFlashLight.enabled = true;
        currentBullets--;//射出一次子弹减少
        UpdateAmmoUI();
        fireTimer = 0f; //重置计时器

        

        if(!isAiming)//如果不是瞄准
        {
            anim.CrossFadeInFixedTime("fire",0.1f); //普通开火动画，并且进行缓入缓出
        }
        else //在瞄准状态下
        {
            
        }
    }

    //更新子弹数，在屏幕上显示
    public void  UpdateAmmoUI()
    {
        AmmoTextUI.text = currentBullets + "/" +bulletLeft;
    }

    //填装子弹
    public void Reload()
    {
        DoReloadAnimation();
        int needReloadBullet;
        needReloadBullet = bulletsMag -  currentBullets; //计算需要填装的子弹数

        //计算备弹扣除子弹数
        if( (bulletLeft - needReloadBullet) >= 0)
        {
            bulletLeft = bulletLeft - needReloadBullet;
        }
        else
        {
            needReloadBullet = bulletLeft;
            bulletLeft = 0;
        }

        currentBullets += needReloadBullet;

        UpdateAmmoUI();
    }

    //开枪声音
    public void PlayerShootSound()
    {
        audioSource.clip = AK47SoundClip;
        audioSource.Play();
    }

    //装弹播放两种动画
    public void DoReloadAnimation()
    {
        if(currentBullets >0)
        {
            //播放动画1
            anim.Play("reload_ammo_left",0,0);
            audioSource.clip = reloadAmmoLeftClip;
            audioSource.Play();
        }
        if(currentBullets == 0)
        {
            //播放动画2
            anim.Play("reload_out_of_ammo",0,0);
           audioSource.clip = reloadOutOfAmmoLeftClip;
           audioSource.Play(); 
        }
    }

    //实现瞄准效果
    public void DoingAim() //
    {
        if(Input.GetMouseButton(1) && !isReload && !PM.isRun ) //按下鼠标右键同时不在换弹和奔跑期间
        {
            //瞄准  准星消失，视野向前靠
            isAiming = true;
            anim.SetBool("Aim",true);
            CrossHairUI.gameObject.SetActive(false);  //关闭准星
            mainCamera.fieldOfView = 25; //瞄准时视野变小
        }
        else
        {
            //非瞄准
            isAiming = false;
            anim.SetBool("Aim",false);
            CrossHairUI.gameObject.SetActive(true);
           mainCamera.fieldOfView = 60; 
        }
    }
}
