using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BarUIManager : MonoBehaviour
{
    public static BarUIManager Instance { get; private set; }
    public Image bossHpBar;
    public Image playerHpBar;

    private void Awake()
    {
        Instance = this;
    }

    public void SetPlayerHp(int hp,int maxHp)
    {
        playerHpBar.fillAmount = (float)hp / maxHp;
    }

    public void SetBossHp(int hp, int maxHp)
    {
        bossHpBar.fillAmount = (float)hp / maxHp;
    }
}
