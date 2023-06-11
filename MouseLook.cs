using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//摄像机的旋转
//玩家左右控制旋转视角
//摄像机上下旋转实现上下旋转

public class MouseLook : MonoBehaviour
{
    public float mouseSensitivity = 100f; //视线灵敏度
    public Transform playerBody; //玩家位置
    public float xRotaion = 0f;
    // Start is called before the first frame update
    void Start()
    {
        //隐藏光标,将其锁定在游戏窗口的中心
        Cursor.lockState = CursorLockMode.Locked;
    }

    // Update is called once per frame
    void Update()
    {
        float mouseX = Input.GetAxis("Mouse X")*mouseSensitivity*Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y")*mouseSensitivity*Time.deltaTime;
        xRotaion -= mouseY;//将上下旋转的轴体值进行累加

        xRotaion = Mathf.Clamp(xRotaion,-80f,80f);//限制轴值为80度
        transform.localRotation = Quaternion.Euler(xRotaion,0f,0f);
        playerBody.Rotate(Vector3.up*mouseX);//实现轴体的横向旋转i]
    }
}
