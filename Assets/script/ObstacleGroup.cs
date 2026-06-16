using UnityEngine;

public class ObstacleGroup : MonoBehaviour
{
    // 障碍物群组移动速度（可在Inspector面板调整）
    public float moveSpeed = 2f;
    // 当障碍物群组的Z轴位置小于这个值时，将其重置到前方
    public float resetZ = -50f;
    // 重置时，障碍物群组移动到的Z轴起始位置
    public float startZ = 100f;

    void Update()
    {
        if (UIController.Instance.Begin == false) return;
        // 沿Z轴负方向移动（Vector3.back就是Z轴负方向）
        transform.Translate(Vector3.right * moveSpeed * Time.deltaTime);

        // 当整个障碍群组移动到重置位置后，重置到前方
        if (transform.position.x < resetZ)
        {
            ResetPosition(); // 调用重置函数，把群组移动到前方
        }

    }

    void ResetPosition()
    {
        // 复制当前群组的位置
        Vector3 pos = transform.position;
        // 将Z轴位置设置为起始位置，使障碍物重新出现在玩家前方
        pos.x = startZ;
        // 应用修改后的位置
        transform.position = pos;
    }
}