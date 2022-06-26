using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hand : MonoBehaviour
{
    public enum HandType
    {
        Basic,
        Axe,
        Pickaxe,
        Meat
    };
    public HandType handState = HandType.Basic;
    public float range;
    public int damage;
    public float workSpeed;
    public float attackDelay;
    // 공격 활성화, 비활성화 시점
    public float attackEnable;
    public float attackDisable;

    public Animator anim;
}
