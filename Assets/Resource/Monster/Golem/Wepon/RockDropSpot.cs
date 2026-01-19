using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class RockDropSpot : MonoBehaviour
{
    public GameObject inside;
    Vector3 targetScale = Vector3.one;

    private bool isTargetScale => Vector3.Distance(targetScale, inside.transform.localScale) < 0.01f;

    public Coroutine bigger;

    private void Start()
    {
        inside.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
    }

    float timer;
    float duration = 0.5f;

    public IEnumerator Bigger()
    {
        timer = 0f;

        Vector3 startScale = inside.transform.localScale;

        while (!isTargetScale)
        {
            timer += Time.deltaTime;
            float t = Mathf.Clamp01(timer / duration);

            inside.transform.localScale =
                Vector3.Lerp(startScale, targetScale, t);

            yield return null;
        }
        inside.transform.localScale = targetScale;

        yield return null;
        Destroy(gameObject);
    }

}
