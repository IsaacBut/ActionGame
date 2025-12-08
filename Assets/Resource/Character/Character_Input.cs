using Unity.VisualScripting;
using UnityEngine;
using static Character;

public class Character_Input : MonoBehaviour
{
    private InPut input = new InPut();
    public Character_Anime character_Anime;
    private void Awake()
    {
        input.GetInputDevice();
    }

    private void Start()
    {
        character_Anime.CharacterInit();
    }

    private void Update()
    {
        character_Anime.Timer();
        character_Anime.AnimeTimer();
        character_Anime.ReturnIdle();

        MoveInput();
        RoolInput();
        AttackInput();
    }

    private void FixedUpdate()
    {
        character_Anime.CharacterStatus();
        character_Anime.BuffTimer();
        character_Anime.RightWeponStatus();
    }

    private const float rollTime = 1.5f;
    private const float attackTime = 1.5f;


    #region Move And Roll

    private float minRollDistance = 50f; 
    //private float middleRollDistance = 150f;
    private float maxRollDistance = 100; 

    private const float rollTimeCD = 0.3f;     // 多长按不会触发
    private float roolTimeCount;

    private Vector2 clickPoint;
    private Vector2 nowPoint;

    private void RoolInput()
    {
        if (!character_Anime.canRool) return;

        // 按下
        if (input.mouse.leftButton.wasPressedThisFrame)
        {
            clickPoint = input.mouse.position.ReadValue();
            roolTimeCount = 0f;
        }

        // 按住中（累計時間）
        if (input.mouse.leftButton.isPressed)
        {
            roolTimeCount += Time.deltaTime;
        }

        // 松开
        if (input.mouse.leftButton.wasReleasedThisFrame)
        {
            nowPoint = input.mouse.position.ReadValue();
            var dragDistance = (nowPoint - clickPoint).magnitude;

            // 1. 长按 → 不触发 roll
            if (roolTimeCount > rollTimeCD || dragDistance < minRollDistance)
            {
                roolTimeCount = 0;
                return;
            }


            Vector3 aPoint = new Vector3(clickPoint.x, 0, clickPoint.y);
            Vector3 bPoint = new Vector3(nowPoint.x, 0, nowPoint.y);
            Vector3 dir = bPoint - aPoint;

            var angle = Mathf.Atan2(dir.x, dir.z) * Mathf.Rad2Deg;
            angle += Camera.main.transform.eulerAngles.y;
            transform.rotation = Quaternion.Euler(0, angle, 0);

            StartCoroutine(character_Anime.Rool());
            roolTimeCount = 0;
        }
    }

    float pressTime;
    bool MousePressed(float sec)
    {
        if (input.mouse.leftButton.isPressed)
        {
            pressTime += Time.deltaTime;
        }
        else
        {
            pressTime = 0;

        }

        return pressTime >= sec;

    }

    float MoveSpeed(Vector2 start, Vector2 end)
    {
        var dragDistance = (end - start).magnitude;

        if (dragDistance < minRollDistance) return 0;
        else if (dragDistance > minRollDistance && dragDistance < maxRollDistance) return character_Anime.moveSpeed;
        else if (dragDistance > maxRollDistance) return character_Anime.moveSpeed * 3;

        Debug.LogWarning("Move Speed Error");
        return 0;
    }


    public void MoveInput()
    {
        if (input.mouse.leftButton.wasPressedThisFrame) clickPoint = input.mouse.position.ReadValue();

        if (!MousePressed(0.25f))
        {
            character_Anime.Stop();
            return;
        }
        else
        {
            if (!character_Anime.ChangeStage(CharacterStage.Move)) return;

            character_Anime.lastInputTime = character_Anime.characterTimer;
            nowPoint = input.mouse.position.ReadValue();

            Vector3 aPoint = new Vector3(clickPoint.x, 0, clickPoint.y);
            Vector3 bPoint = new Vector3(nowPoint.x, 0, nowPoint.y);
            Vector3 dir = bPoint - aPoint;

            var angle = Mathf.Atan2(dir.x, dir.z) * Mathf.Rad2Deg;
            angle += Camera.main.transform.eulerAngles.y;

            character_Anime.Move();
            transform.rotation = Quaternion.Euler(0, angle, 0);
            transform.position += transform.forward * MoveSpeed(aPoint, bPoint) * Time.deltaTime;


        }

    }
    #endregion

    #region Attack

    private void AttackInput()
    {
        if (input.mouse.leftButton.wasPressedThisFrame&& character_Anime.canAttack)
        {
            if (!character_Anime.ChangeStage(CharacterStage.Attack)) return;
            character_Anime.lastInputTime = character_Anime.characterTimer;

            StartCoroutine(character_Anime.Attack());
        }
    }


    #endregion


}
