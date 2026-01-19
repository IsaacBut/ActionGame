using UnityEngine;
using System.Collections.Generic;

public class InGameManager : MonoBehaviour
{
    public static InGameManager Instance;


    [Header("Camera")]
    public Camera camera;
    public Camera miniMapCamera;

    private readonly Quaternion cameraQuaternion = Quaternion.Euler(45, -45, 0);
    private readonly Vector3 cameraPos = new Vector3(14f, 21.8f, -13.5f);
    private const float miniMapCameraPosY = 30f;

    [Header("Character")]
    public GameObject player;
    public HashSet<Character> allCharacter;

    private void FindAllTheCharacter()
    {
        allCharacter = new HashSet<Character>(
            FindObjectsByType<Character>(FindObjectsSortMode.None)
        );
    }

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(this);
    }
    private void Start()
    {
        FindAllTheCharacter();
        camera = Camera.main;
    }

    public void CameraFollow(Vector3 targetVector3)
    {
        var finallyVector3 = targetVector3 + cameraPos;
        var miniMapCameraPos = new Vector3(targetVector3.x, miniMapCameraPosY, targetVector3.z);

        camera.transform.position = finallyVector3;
        if (miniMapCamera != null) miniMapCamera.transform.position = miniMapCameraPos;
    }



}
