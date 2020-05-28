using System;
using UnityEngine;
using System.Collections;
public class DashEnemy:Enemy
{
    public GameObject dashEffect;

    private bool isDashing = false;

    private bool isHurtPlayerInDashing = false;

    protected override void Death()
    {
        base.Death();
    }

    protected override void Attack()
    {
        if (coolDownAttack)
            return;
        if (!isDashing)
        {
            StartCoroutine(Dash());
        }
    }

    private IEnumerator Dash()
    {
        isDashing = true;
        Vector3 targetPos = player.transform.position;
        Vector3 curPos = transform.position;
        Vector3 lastPos = targetPos;
        //Instantiate(dashEffect, transform.position, Quaternion.identity);
        while (Vector3.Distance(curPos,targetPos) > 0.1f)
        {
            transform.position = Vector2.MoveTowards(transform.position, targetPos, 5 * speed * Time.deltaTime);
            //Debug.Log("dash " + targetPos);
            yield return null;
            curPos = transform.position;
        }
        //Debug.Log("dash finish");
        isDashing = false;
        isHurtPlayerInDashing = false;
        StartCoroutine(CoolDown());
    }


    protected override void OnCollisionEnter2D(Collision2D collision)
    {
        if (isDashing && !isHurtPlayerInDashing)
        {
            GameController.DamagePlayer(1);
            isHurtPlayerInDashing = true;
        }
        base.OnCollisionEnter2D(collision);
    }

}
