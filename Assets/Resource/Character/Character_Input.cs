using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

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
        CameraMove();

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

    #region Camera&Canvas

    private void CameraMove() => InGameManager.Instance.CameraFollow(this.transform.position);

    public Image markBasePrefab;
    public Image markArrowPrefab;

    public Canvas mainCanvas;

    private Image markBase;
    private Image markArrow;
    private void TouchMarkBase(Vector2 targetPoint)
    {
        if (markBase != null) return;

        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            mainCanvas.transform as RectTransform,
            targetPoint,
            mainCanvas.renderMode == RenderMode.ScreenSpaceOverlay
                ? null
                : mainCanvas.worldCamera,
            out Vector2 localPos
        );

        markBase = Instantiate(markBasePrefab, mainCanvas.transform);
        markBase.GetComponent<RectTransform>().anchoredPosition = localPos;
    }
    private void TouchMarkArrow(Vector2 targetPoint)
    {

        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            mainCanvas.transform as RectTransform,
            targetPoint,
            mainCanvas.renderMode == RenderMode.ScreenSpaceOverlay
                ? null
                : mainCanvas.worldCamera,
            out Vector2 localPos
        );

        if (markArrow == null)
        {
            markArrow = Instantiate(markArrowPrefab, mainCanvas.transform);
        }

        markArrow.GetComponent<RectTransform>().anchoredPosition = localPos;
    }


    private void MarkDele()
    {
        Destroy(markBase);
        Destroy(markArrow);
    }
    #endregion



    #region Human 

    private float minRollDistance = 10f; 
    //private float middleRollDistance = 150f;
    private float maxRollDistance = 40; 

    private const float rollTimeCD = 0.3f;     // 多长按不会触发
    private float roolTimeCount;

    private Vector2 clickPoint;
    private Vector2 nowPoint;

    private void RoolInput()
    {
        if (!character_Anime.canRool) return;

        if (input.mouse.leftButton.wasPressedThisFrame)
        {
            clickPoint = input.mouse.position.ReadValue();
            roolTimeCount = 0f;
        }

        if (input.mouse.leftButton.isPressed)
        {
            roolTimeCount += Time.deltaTime;
        }

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
        else if (dragDistance > minRollDistance/* && dragDistance < maxRollDistance*/) return character_Anime.moveSpeed * 3;
        //else if (dragDistance > maxRollDistance) return character_Anime.moveSpeed * 3;

        Debug.LogWarning("Move Speed Error");
        return 0;
    }


    public void MoveInput()
    {
        if (character_Anime.nowStage != CharacterStage.Move) MarkDele();
        //if (input.mouse.leftButton.wasPressedThisFrame) clickPoint = input.mouse.position.ReadValue();

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

            TouchMarkBase(nowPoint);
            TouchMarkArrow(nowPoint);

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
