using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class BattleVisuals : MonoBehaviour
{
    [SerializeField] private Slider hpSlider;
    [SerializeField] private TextMeshProUGUI levelText;
    [SerializeField] private Animator animator;

    private float curHP;
    private float maxHP;
    private int level;

    private const string LEVEL_ABV = "Lvl: ";
    private const string ATTACK_ANIM_TRIGGER = "attack";
    private const string IS_HIT_ANIM_TRIGGER = "isHit";
    private const string IS_DEAD_ANIM_TRIGGER = "IsDead";

    private void Start()
    {
        
    }

    public void SetStartingValues(float currentHP, float maxHP, int level)
    {
        curHP = currentHP;
        this.maxHP = maxHP;
        this.level = level;
        hpSlider.maxValue = maxHP;
        hpSlider.value = currentHP;
        levelText.text = LEVEL_ABV + level;
    }

    public void ChangeHealth(float currentHP)
    {
        curHP = currentHP;

        if (curHP < 0)
        {
            PlayIsDeadAnim();
            //Destroy(gameObject, 1f);
        }
        UpdateHPBar();
    }

    public void UpdateHPBar()
    {
        hpSlider.value = curHP;
    }

    public void PlayAttackAnim()
    {
        animator.SetTrigger(ATTACK_ANIM_TRIGGER);
    }

    public void PlayIsHitAnim()
    {
        animator.SetTrigger(IS_HIT_ANIM_TRIGGER);
    }

    public void PlayIsDeadAnim()
    {
        animator.SetBool(IS_DEAD_ANIM_TRIGGER, true);
    }

}
