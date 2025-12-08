using UnityEngine;
using UnityEngine.InputSystem;
using static UnityEngine.GraphicsBuffer;

public class TestingMove : MonoBehaviour
{
    InPut inPut = new InPut();
    public Camera camera;
    public GameObject prefab;
    float yRedio;

    private void Start()
    {
        inPut.GetInputDevice();
        yRedio = camera.transform.position.y - this.transform.position.y;
    }

    //Vector3 pointA;   // 鼠标第一次点击的位置（世界坐标）
    //Vector3 pointB;   // 鼠标按住时实时更新的位置（世界坐标）
    //void Move()
    //{
    //    // 1. 按下鼠标的瞬间 → 记录 A
    //    if (Mouse.current.leftButton.wasPressedThisFrame)
    //    {
    //        pointA = GetMouseWorld();   // 你自己的方式获得点击位置
    //    }

    //    // 2. 按住鼠标 → 持续记录 B
    //    if (Mouse.current.leftButton.isPressed)
    //    {
    //        pointB = GetMouseWorld();   // 你自己的方式获得当前鼠标世界坐标
    //        RotateObject(pointA, pointB);
    //    }

    //}

    //Vector3 GetMouseWorld()
    //{
    //    // ❗你必须把这里改成你自己的鼠标 → 世界坐标获取方法
    //    return yourMouseWorldPosition;
    //}


    //void RotateObject(Vector3 A, Vector3 B)
    //{
    //    Vector3 dir = B - A;
    //    dir.y = 0;  // 水平角

    //    float angle = Mathf.Atan2(dir.x, dir.z) * Mathf.Rad2Deg;
    //    transform.rotation = Quaternion.Euler(0, angle, 0);
    //}


    private void Update()
    {
        Move();


    }

    Vector2 clickPoint;


    void Move()
    {
        if (!inPut.mouse.leftButton.IsPressed()) return;

        if (inPut.mouse.leftButton.wasPressedThisFrame) clickPoint = inPut.mouse.position.ReadValue();

        var nowPoint = inPut.mouse.position.ReadValue();

        Vector3 aPoint = new Vector3(clickPoint.x, 0, clickPoint.y);
        Vector3 bPoint = new Vector3(nowPoint.x, 0, nowPoint.y);
        Vector3 dir = aPoint - bPoint;
        var angle = Mathf.Atan2(dir.x, dir.z) * Mathf.Rad2Deg;

        transform.rotation = Quaternion.Euler(0, angle, 0);

    }



}
