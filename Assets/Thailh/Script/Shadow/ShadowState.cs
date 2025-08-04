using UnityEngine;

public partial class ShadowController : MonoBehaviour
{
    private void PatrolState()
    {
        if (player == null)
        {
            animator.SetBool("IsRun", true);
            transform.position = Vector2.MoveTowards(transform.position, targetPoint.position, patrolSpeed * Time.deltaTime);
            if (Vector2.Distance(transform.position, targetPoint.position) < 0.1f)
            {
                if (targetPoint == pointA)
                {
                    targetPoint = pointB;
                }
                else
                {
                    targetPoint = pointA;
                }
                Flip();
            }
            return;
        }

        if (currentHealth < healThreshold && Time.time > lastHealTime + healCooldown)
        {
            currentState = State.Heal;
            animator.SetBool("IsHeal", true);
            animator.SetBool("IsRun", false);
            rb.velocity = Vector2.zero;
            healTimer = 0f;
            return;
        }

        animator.SetBool("IsRun", true);
        hasTriggeredAttack = false;

        transform.position = Vector2.MoveTowards(transform.position, targetPoint.position, patrolSpeed * Time.deltaTime);
        if (Vector2.Distance(transform.position, targetPoint.position) < 0.1f)
        {
            if (targetPoint == pointA)
            {
                targetPoint = pointB;
            }
            else
            {
                targetPoint = pointA;
            }
            Flip();
        }
    }

    private void AttackState()
    {
        if (player == null)
        {
            currentState = State.Patrol;
            animator.SetBool("IsRun", true);
            return;
        }

        rb.velocity = Vector2.zero;
        LookAtPlayer();

        if (!hasTriggeredAttack && Time.time > lastAttackTime + attackCooldown)
        {
            animator.SetBool("IsAttack1", true);
            lastAttackTime = Time.time;
            hasTriggeredAttack = true;
        }
    }

    private void Attack2State()
    {
        if (player == null)
        {
            currentState = State.Patrol;
            animator.SetBool("IsRun", true);
            return;
        }

        rb.velocity = Vector2.zero;
        LookAtPlayer();

        if (!hasTriggeredAttack)
        {
            animator.SetBool("IsAttack2", true);
            hasTriggeredAttack = true;
        }
    }

    private void HealState()
    {
        rb.velocity = Vector2.zero;
        animator.SetBool("IsHeal", true);
        hasTriggeredAttack = false;

        healTimer += Time.deltaTime;
        if (healTimer >= healDuration)
        {
            currentHealth = maxHealth;
            lastHealTime = Time.time;
            animator.SetBool("IsHeal", false);
            CheckPlayerRangeAndSetState();
        }
    }

    private void DefendState()
    {
        rb.velocity = Vector2.zero;
        defendTimer += Time.deltaTime;

        if (defendTimer >= defendDuration)
        {
            animator.SetBool("IsDefend", false);
            CheckPlayerRangeAndSetState();

            if (currentState == State.Patrol)
            {
                LookAtTarget(targetPoint);
            }
        }
    }
}