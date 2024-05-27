using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BarUIManager : MonoBehaviour
{
    public static BarUIManager Instance { get; private set; }
    public Image bossHpBar;
    public Image playerHpBar, playerHpBarBg;

    public Animation healthBarAnim;

    private void Awake()
    {
        Instance = this;
    }

    public void SetPlayerHp(int hp,int maxHp)
    {
        float newHealth = (float)hp / maxHp;
        if (newHealth > playerHpBarBg.fillAmount) playerHpBarBg.fillAmount = newHealth;

        healthBarAnim.Play();

        playerHpBar.fillAmount = newHealth;
    }

    public void SetBossHp(int hp, int maxHp)
    {
        bossHpBar.fillAmount = (float)hp / maxHp;
    }

    private void Update()
    {
        playerHpBarBg.fillAmount = Mathf.MoveTowards(playerHpBarBg.fillAmount, playerHpBar.fillAmount, Time.unscaledDeltaTime / 4f);
    }
}
