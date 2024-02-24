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
            DontDestroyOnLoad(gameObject); // 使得该对象不会在加载新场景时被销毁
        }
        else if (Instance != this)
        {
            Destroy(gameObject); // 确保场景中只有一个实例
        }
    }

    // 一个通用的方法，用于延迟生成物品
    public IEnumerator DelaySpawnItem(Vector3 position, GameObject prefabItem)
    {
        yield return new WaitForSeconds(1.0f); // 延迟时间
        Instantiate(prefabItem, position, Quaternion.identity);
    }
}
