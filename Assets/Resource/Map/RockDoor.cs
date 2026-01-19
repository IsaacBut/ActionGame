using UnityEngine;
public class RockDoor : MonoBehaviour
{
    [SerializeField] private GameObject m_Door;

    [SerializeField] bool open;
    [SerializeField] bool close;



    const float closeAngle = 0;
    const float openAngle = 90;

    const float speed = 5f;

    void OpenDoor()
    {
        var diff = Mathf.DeltaAngle(m_Door.transform.localRotation.y, openAngle);
        if (diff < 0.1f) return;

        Quaternion targetRot = Quaternion.Euler(0, openAngle, 0);
        m_Door.transform.localRotation = Quaternion.Slerp(m_Door.transform.localRotation, targetRot, speed * Time.deltaTime);

        if(m_Door.transform.localRotation == Quaternion.Euler(0, openAngle, 0))
        {
            open = false;
        }
    }

    void CloseDoor()
    {
        var diff = Mathf.Abs(Mathf.DeltaAngle(m_Door.transform.localRotation.y, closeAngle));
        if (diff < 0.01f) return;

        Quaternion targetRot = Quaternion.Euler(0, closeAngle, 0);
        m_Door.transform.localRotation = Quaternion.Slerp(m_Door.transform.localRotation, targetRot, speed * Time.deltaTime);

        if(m_Door.transform.localRotation == Quaternion.Euler(0, closeAngle, 0))
        {
            close = false;

        }
    }

    private void Update()
    {
        if (open && close)
        {
            open = false;
            close = false;
        }

        if (open && !close) OpenDoor();
        else if (close && !open) CloseDoor();



    }


}
