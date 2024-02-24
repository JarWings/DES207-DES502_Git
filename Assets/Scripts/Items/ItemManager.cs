using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemManager : MonoBehaviour
{
    public static ItemManager Instance;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // ʹ�øö��󲻻��ڼ����³���ʱ������
        }
        else if (Instance != this)
        {
            Destroy(gameObject); // ȷ��������ֻ��һ��ʵ��
        }
    }

    // һ��ͨ�õķ����������ӳ�������Ʒ
    public IEnumerator DelaySpawnItem(Vector3 position, GameObject prefabItem)
    {
        yield return new WaitForSeconds(1.0f); // �ӳ�ʱ��
        Instantiate(prefabItem, position, Quaternion.identity);
    }
}
