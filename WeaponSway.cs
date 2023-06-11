using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponSway : MonoBehaviour
{
    /*摇摆参数*/

    public float amount;//摇摆幅度
    public float smoothAmount; //平滑值
    public float maxAmount;//最大幅度摇摆

    [SerializeField] private Vector3 originPosition;//初始位置


    // Start is called before the first frame update
    void Start()
    {
        //自身位置(相对于父级物体的变换)
        originPosition = transform.localPosition;

    }

    // Update is called once per frame
    void Update()
    {
        //获取鼠标轴值
        float movementX = -Input.GetAxis("Mouse X");
        float movementY = -Input.GetAxis("Mouse Y");

        //限制大小
        movementX = Mathf.Clamp(movementX, -maxAmount,maxAmount);
        movementY = Mathf.Clamp(movementY, -maxAmount,maxAmount);
        //手臂变化幅度
        Vector3 finallyPosition = new Vector3(movementX,movementY,0);

        //平滑过度通过每秒进行变换
        transform.localPosition = Vector3.Lerp(transform.localPosition,finallyPosition + originPosition,Time.deltaTime *smoothAmount);
    }
}
