using UnityEngine;

public class tall : MonoBehaviour
{
    [Header("x的范围")]
    public float minX = -1.426f;
    public float maxX = 1.446f;

    [Header("速度")]
    public float speed = 1.0f;

    private float offset;   // 随机时间偏移，使各长板运动不同步

    void Start()
    {
        offset = Random.Range(0f, 100f);
    }

    void Update()
    {
        float length = maxX - minX;
        // 加入偏移使每个物体的运动起始位置不同
        float pingp = minX + Mathf.PingPong((Time.time + offset) * speed, length);
        Vector3 pos = transform.position;
        pos.x = pingp;
        transform.position = pos;
    }
}