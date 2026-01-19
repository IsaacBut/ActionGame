using UnityEngine;
using System.Collections.Generic;
using TMPro;
using static UnityEngine.GraphicsBuffer;

public class Character_AI : MonoBehaviour
{
    public Character_Anime character_Anime;
    private Collider _hitBox => character_Anime.hitBox;
    [SerializeField] private HashSet<Character> _allCharacter => InGameManager.Instance.allCharacter;
    [SerializeField] private GameObject _player => InGameManager.Instance.player;

    private float hitBoxDistance;
    private const float characterDistance = 1.5f;

    public float rotateSpeed = 2.0f;
    public float moveSpeed = 2.0f;

    private void HitBoxDistanceInit()
    {
        if(_hitBox == null )
        {
            Debug.LogWarning("[Character_AI] No hitBox");
            return;
        }

        var height = _hitBox.bounds.max.y - transform.position.y;
        var width = _hitBox.bounds.max.x - transform.position.x;

        hitBoxDistance = Mathf.Sqrt(height * height + width * width);

    }

    private void SeparateRule()
    {
        Vector3 separation = Vector3.zero;

        foreach (Character character in _allCharacter)
        {
            if (character == this) continue;

            float distance = Vector3.Distance(transform.position, character.transform.position);
            if (distance > hitBoxDistance * characterDistance) continue;

            Vector3 away = transform.position - character.transform.position;
            away.y = 0f;

            if (away.sqrMagnitude < 0.0001f) continue;

            separation += away.normalized / distance; 
        }

        if (separation != Vector3.zero)
        {
            //transform.position += separation.normalized * character_Anime.moveSpeed * Time.deltaTime;
            transform.position += separation.normalized * moveSpeed * Time.deltaTime;

        }
    }

    private void AlignmentRule()
    {
        Vector3 dir = _player.transform.forward;
        dir.y = 0f;

        Quaternion targetRot = Quaternion.LookRotation(dir);

        transform.rotation = Quaternion.Slerp(
            transform.rotation,
            targetRot,
            rotateSpeed * Time.deltaTime
        );
    }

    private void ReuniteRule()
    {

        Vector3 Reunification = Vector3.zero;

        foreach (Character character in _allCharacter)
        {
            if (character == this) continue;
            float distance = Vector3.Distance(transform.position, character.transform.position);
            if (distance < hitBoxDistance * characterDistance) continue;


        }
    }

    private void Start()
    {
        HitBoxDistanceInit();
    }

    private void LateUpdate()
    {
        SeparateRule();
        AlignmentRule();
        ReuniteRule();
    }
}
