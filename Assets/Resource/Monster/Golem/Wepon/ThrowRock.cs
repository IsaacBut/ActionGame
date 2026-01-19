using UnityEngine;
using System.Collections;

public class ThrowRock : MonoBehaviour
{
    public GameObject rock;
    public GameObject barrel;

    public GameObject dropSpot;
    private RockDropSpot rockDropSpot;
    private Quaternion dropSpotRotation => Quaternion.Euler(90f, 0f, 0f);
    private Vector3 targetSpot;

    private GameObject createdRock;

    private GameObject RandomRock()
    {
        int random;
        random = Random.Range(0, 10);

        GameObject target = random == 4 ? barrel : rock;

        return target;
    }

    public IEnumerator DropSpotAmine(Vector3 targetArea)
    {
        targetSpot = targetArea;
        float posY = 0.1f;
        Vector3 area = new Vector3(targetArea.x, posY, targetArea.z);

        GameObject spot = Instantiate(dropSpot, area, dropSpotRotation);

        rockDropSpot = spot.GetComponent<RockDropSpot>();
        if (rockDropSpot == null)
        {
            Debug.LogError("RockDropSpot component missing on dropSpot prefab");
            yield break;
        }

        if (rockDropSpot.bigger == null)
        {
            rockDropSpot.bigger =
                rockDropSpot.StartCoroutine(rockDropSpot.Bigger());
        }

        yield return rockDropSpot.bigger;
    }

    public void CreateRock()
    {
        if (createdRock == null) createdRock = Instantiate(RandomRock(), transform.position, Quaternion.identity);
        createdRock.transform.parent = transform;
    }

    public void Throw()
    {
        if (createdRock != null)
        {
            createdRock.transform.parent = null;
            createdRock.GetComponent<Rock>().enabled = true;
            createdRock.GetComponent<Rock>().targetPos = targetSpot;
        }
    }

    public void NullRock() => createdRock = null;
}
