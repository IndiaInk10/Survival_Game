using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandController : MonoBehaviour
{
    // 현재 장착된 HandType
    [SerializeField]
    private Hand currentHand;

    private bool isAttack = false;
    private bool isSwing = false;

    // Raycast에 닿은 녀석의 정보를 저장할 수 있는 변수
    private RaycastHit hitInfo;

    void Update()
    {
        TryAttack();
    }

    private void TryAttack()
    {
        // 좌클릭 또는 Ctrl인데 해당 부분을 유니티에서 수정
        if(Input.GetButton("Fire1"))
        {
            if(!isAttack)
            {
                StartCoroutine(CoAttack());
            }
        }
    }

    IEnumerator CoAttack()
    {
        isAttack = true;
        currentHand.anim.SetTrigger("Attack");

        yield return new WaitForSeconds(currentHand.attackEnable);
        isSwing = true;

        StartCoroutine(CoHit());

        yield return new WaitForSeconds(currentHand.attackDisable);
        isSwing = false;

        yield return new WaitForSeconds(currentHand.attackDelay - (currentHand.attackEnable + currentHand.attackDisable));

        isAttack = false;
    }

    IEnumerator CoHit()
    {
        while(isSwing)
        {
            if(CheckObject())
            {
                isSwing = false;
            }
            yield return null;
        }
    }

    private bool CheckObject()
    {
        if(Physics.Raycast(transform.position, transform.forward, out hitInfo, currentHand.range))
        {
            return true;
        }
        return false;
    }
}
