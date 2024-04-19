using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthItems : MonoBehaviour
{
    public string journalEntry; // leave blank if there is no entry for this item
    public float amplitude = 5f; // ��ת�������С
    public float frequency = 5f; // ��ת��Ƶ��
    private float timer = 0; // ��ʱ�������ڿ�����ת��Ƶ��

    public GameObject prefabItem;
    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        // ���¼�ʱ��
        timer += Time.deltaTime;
        // ������תЧ���ĽǶ�
        float rotationAngle = Mathf.Sin(timer * frequency) * amplitude;

        // Ӧ����תЧ��
        // ����3D����ʹ��z��
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
