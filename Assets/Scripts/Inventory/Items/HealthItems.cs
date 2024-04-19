using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthItems : MonoBehaviour
{
    public string journalEntry; // leave blank if there is no entry for this item
    public float amplitude = 5f; // 旋转的振幅大小
    public float frequency = 5f; // 旋转的频率
    private float timer = 0; // 计时器，用于控制旋转的频率

    public GameObject prefabItem;
    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        // 更新计时器
        timer += Time.deltaTime;
        // 计算旋转效果的角度
        float rotationAngle = Mathf.Sin(timer * frequency) * amplitude;

        // 应用旋转效果
        // 对于3D对象使用z轴
        transform.rotation = Quaternion.Euler(new Vector3(0, 0, rotationAngle));
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        JournalManager.FindEntry(journalEntry);
        //GameObject HealthItem = Instantiate(prefabItem);
        //float x = Random.Range(-10f, 10f);
        //HealthItem.transform.position = new Vector2(x, 0.9f);
        Destroy(this.gameObject);
    }
}
