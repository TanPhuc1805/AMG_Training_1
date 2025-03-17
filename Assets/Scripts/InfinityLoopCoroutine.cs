using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InfinityLoopCoroutine : MonoBehaviour
{
    public float speed = 2f;
    public float size = 3f;
    private float t = 0f;
    private Vector3 startPos;
    void Start()
    {
        startPos = transform.position;
        StartCoroutine(MoveInInfinityLoop());
    }

    IEnumerator MoveInInfinityLoop()
    {
        while (true)
        {
            t += speed * Time.deltaTime;

            float x = size * Mathf.Cos(t) + startPos.x;
            float y = (size / 2) * Mathf.Sin(2 * t) + startPos.y;

            transform.position = new Vector3(x, y, 0);

            yield return null;
        }
    }
}
