using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FinalBoss : MonoBehaviour
{
    public Animator animator;
    public int phaseFinalBoss = 0;
    public float transformSpawnMonster1 = 15f;
    public float transformSpawnMonster2 = -15f;
    public int countSummon = 0;
    public int countHardSkill = 0;
    public bool hardSkill = false;
    private BossHealth bossHealth;
    GameObject skill1;
    GameObject skill2;
    GameObject skill3;
    GameObject skill4;
    GameObject skill5;
    GameObject skill6;
    GameObject skill7;
    [SerializeField] GameObject castSkill;
    [SerializeField] GameObject spawnAreaMonster;
    [SerializeField] GameObject effectSkill01;
    [SerializeField] GameObject effectSkill02;
    [SerializeField] GameObject effectSkill03;
    [SerializeField] GameObject effectSkill04;
    [SerializeField] GameObject BigEffectCastSkill;
    [SerializeField] GameObject BigEffectSummon;
    [SerializeField] GameObject arrowSpawn;
    [SerializeField] GameObject DeadEffectBoss;
    [SerializeField] GameObject miniBoss;
    // Start is called before the first frame update
    void Start()
    {
        bossHealth = GetComponent<BossHealth>();
    }

    // Update is called once per frame
    void Update()
    {
        phaseBossController();
    }
    public void phase1BossFinal()
    {
        animator.SetBool("IsPhase1", true);
    }
    public void phase2BossFinal()
    {
        animator.SetBool("IsPhase1", false);
        animator.SetBool("IsPhase2", true);
        StartCoroutine(EffectcastSkillPhase2());
    }
    public void phase3BossFinal()
    {
        animator.SetBool("IsPhase1", false);
        animator.SetBool("IsPhase2", false);
        animator.SetBool("IsPhase3", true);
        EffectcastSkillPhase3();
        StartCoroutine(summonMiniBoss());
        StartCoroutine(castSkillPhase3());
    }
    public void CastSkillSummonMagic1()
    {
        Vector3 spawnPos = new Vector3(transform.position.x - 7.5f, transform.position.y + 4f, transform.position.z);
        GameObject SkillSummonMagic1 = Instantiate(castSkill, spawnPos, Quaternion.identity);
        Destroy(SkillSummonMagic1, 1.7f);
    }
    public void CastSkillSummonMagic2()
    {
        Vector3 spawnPos = new Vector3(transform.position.x + 8f, transform.position.y + 4f, transform.position.z);
        GameObject SkillSummonMagic2 = Instantiate(castSkill, spawnPos, Quaternion.identity);
        Destroy(SkillSummonMagic2, 1.7f);
    }
    public void spawnMonsterArea1()
    {
        Vector3 spawnPos = new Vector3(transform.position.x + transformSpawnMonster1, transform.position.y - 5.5f, transform.position.z);
        GameObject spawnMonsterArea1 = Instantiate(spawnAreaMonster, spawnPos, Quaternion.identity);
        Destroy(spawnMonsterArea1, 5f);
    }

    public void spawnMonsterArea2()
    {
        Vector3 spawnPos = new Vector3(transform.position.x + transformSpawnMonster2, transform.position.y - 5.5f, transform.position.z);
        GameObject spawnMonsterArea2 = Instantiate(spawnAreaMonster, spawnPos, Quaternion.identity);
        Destroy(spawnMonsterArea2, 5f);
    }


    //bossHealt.currentHealth <= 0 máu boss là 3k

    private IEnumerator EffectcastSkillPhase2()
    {
        Vector3 spawnPos = new Vector3(transform.position.x +0.7f, transform.position.y + 8f, transform.position.z);
        skill5 = Instantiate(BigEffectCastSkill, spawnPos, Quaternion.identity);
        yield return new WaitForSeconds(2f);
        Vector3 spawnPos1 = new Vector3(transform.position.x + 4.5f, transform.position.y + 8f, transform.position.z);
        Quaternion rotation1 = Quaternion.Euler(0f, 0f, 40.379f);
        Vector3 spawnPos2 = new Vector3(transform.position.x - 4.3f, transform.position.y + 8f, transform.position.z);
        Quaternion rotation2 = Quaternion.Euler(0f, 0f, -28.83f);
        skill1 = Instantiate(effectSkill01, spawnPos1, rotation1);
        skill2 = Instantiate(effectSkill02, spawnPos2, rotation2);
    }


    private void EffectcastSkillPhase3()
    {
        Vector3 spawnPos1 = new Vector3(transform.position.x + 5f, transform.position.y + 8.4f, transform.position.z);
        Quaternion rotation1 = Quaternion.Euler(0f, 0f, 26f);
        Vector3 spawnPos2 = new Vector3(transform.position.x - 3.7f, transform.position.y + 8.2f, transform.position.z);
        Quaternion rotation2 = Quaternion.Euler(0f, 0f, -22f);
        skill3 = Instantiate(effectSkill03, spawnPos1, rotation1);
        skill4 = Instantiate(effectSkill04, spawnPos2, rotation2);
    }
    private IEnumerator castSkillPhase1()
    {
        if (countSummon > 2)
        {
            transformSpawnMonster1 = 15f;
            transformSpawnMonster2 = -15f;
            countSummon = 0;
        }
        else
        {
            CastSkillSummonMagic1();
            yield return new WaitForSeconds(2f);
            CastSkillSummonMagic2();
            yield return new WaitForSeconds(2f);
            spawnMonsterArea1();
            yield return new WaitForSeconds(2f);
            spawnMonsterArea2();
            transformSpawnMonster1 -= 3f;
            transformSpawnMonster2 += 3f;
            countSummon += 1;
        }
    }
    private IEnumerator castSkillPhase2()
    {
        if (countSummon > 2)
        {
            transformSpawnMonster1 = 15f;
            transformSpawnMonster2 = -15f;
            countSummon = 0;
        }
        else
        {
            spawnMonsterArea1();
            yield return new WaitForSeconds(2f);
            spawnMonsterArea2();
            transformSpawnMonster1 -= 3f;
            transformSpawnMonster2 += 3f;
            countSummon += 1;
        }
    }


    private IEnumerator castSkillPhase3()
    {
        yield return new WaitForSeconds(3f);
        Vector3 spawnPos1 = new Vector3(transform.position.x - 17.5f, transform.position.y + 9f, transform.position.z);        
        Vector3 spawnPos2 = new Vector3(transform.position.x - 11.67f, transform.position.y + 9f, transform.position.z);        
        Vector3 spawnPos3 = new Vector3(transform.position.x - 5.83f, transform.position.y + 9f, transform.position.z);
        Vector3 spawnPos4 = new Vector3(transform.position.x, transform.position.y + 9f, transform.position.z);
        Vector3 spawnPos5 = new Vector3(transform.position.x + 5.83f, transform.position.y + 9f, transform.position.z);
        Vector3 spawnPos6 = new Vector3(transform.position.x + 11.67f, transform.position.y + 9f, transform.position.z);
        Vector3 spawnPos7 = new Vector3(transform.position.x + 17.5f, transform.position.y + 9f, transform.position.z);
        GameObject SummonArrow1 = Instantiate(arrowSpawn, spawnPos1, Quaternion.identity);
        Destroy(SummonArrow1, 3f);
        yield return new WaitForSeconds(0.3f);
        GameObject SummonArrow2 = Instantiate(arrowSpawn, spawnPos2, Quaternion.identity);
        Destroy(SummonArrow2, 3f);
        yield return new WaitForSeconds(0.3f);
        GameObject SummonArrow3 = Instantiate(arrowSpawn, spawnPos3, Quaternion.identity);
        Destroy(SummonArrow3, 3f);
        yield return new WaitForSeconds(0.3f);
        GameObject SummonArrow4 = Instantiate(arrowSpawn, spawnPos4, Quaternion.identity);
        Destroy(SummonArrow4, 3f);
        yield return new WaitForSeconds(0.3f);
        GameObject SummonArrow5 = Instantiate(arrowSpawn, spawnPos5, Quaternion.identity);
        Destroy(SummonArrow5, 3f);
        yield return new WaitForSeconds(0.3f);
        GameObject SummonArrow6 = Instantiate(arrowSpawn, spawnPos6, Quaternion.identity);
        Destroy(SummonArrow6, 3f);
        yield return new WaitForSeconds(0.3f);
        GameObject SummonArrow7 = Instantiate(arrowSpawn, spawnPos7, Quaternion.identity);
        Destroy(SummonArrow7, 3f);



        yield return new WaitForSeconds(3.5f);
        GameObject SummonArrow8 = Instantiate(arrowSpawn, spawnPos7, Quaternion.identity);
        Destroy(SummonArrow8, 3f);
        yield return new WaitForSeconds(0.3f);
        GameObject SummonArrow9 = Instantiate(arrowSpawn, spawnPos6, Quaternion.identity);
        Destroy(SummonArrow9, 3f);
        yield return new WaitForSeconds(0.3f);
        GameObject SummonArrow10 = Instantiate(arrowSpawn, spawnPos5, Quaternion.identity);
        Destroy(SummonArrow10, 3f);
        yield return new WaitForSeconds(0.3f);
        GameObject SummonArrow11 = Instantiate(arrowSpawn, spawnPos4, Quaternion.identity);
        Destroy(SummonArrow11, 3f);
        yield return new WaitForSeconds(0.3f);
        GameObject SummonArrow12 = Instantiate(arrowSpawn, spawnPos3, Quaternion.identity);
        Destroy(SummonArrow12, 3f);
        yield return new WaitForSeconds(0.3f);
        GameObject SummonArrow13 = Instantiate(arrowSpawn, spawnPos2, Quaternion.identity);
        Destroy(SummonArrow13, 3f);
        yield return new WaitForSeconds(0.3f);
        GameObject SummonArrow14 = Instantiate(arrowSpawn, spawnPos1, Quaternion.identity);
        Destroy(SummonArrow14, 3f);
        yield return new WaitForSeconds(3.5f);
        animator.SetBool("IsHardSkill", true);
    }


        public void countHardSkillCheck()
    {
        if (countHardSkill > 1)
        {
            animator.SetBool("IsHardSkill", false);
            countHardSkill = 0;
        }
        else
        {
            countHardSkill++;
        } 
            
    }








    private void castSkillPhase3hard1()
    {
        Vector3 spawnPos1 = new Vector3(transform.position.x - 17.5f, transform.position.y + 9f, transform.position.z);
        Vector3 spawnPos2 = new Vector3(transform.position.x - 11.67f, transform.position.y + 9f, transform.position.z);
        Vector3 spawnPos3 = new Vector3(transform.position.x - 5.83f, transform.position.y + 9f, transform.position.z);
        Vector3 spawnPos4 = new Vector3(transform.position.x, transform.position.y + 9f, transform.position.z);
        Vector3 spawnPos5 = new Vector3(transform.position.x + 5.83f, transform.position.y + 9f, transform.position.z);
        Vector3 spawnPos6 = new Vector3(transform.position.x + 11.67f, transform.position.y + 9f, transform.position.z);
        Vector3 spawnPos7 = new Vector3(transform.position.x + 17.5f, transform.position.y + 9f, transform.position.z);
        GameObject SummonArrow15 = Instantiate(arrowSpawn, spawnPos1, Quaternion.identity);
        Destroy(SummonArrow15, 3f);
        GameObject SummonArrow16 = Instantiate(arrowSpawn, spawnPos2, Quaternion.identity);
        Destroy(SummonArrow16, 3f);
        GameObject SummonArrow17 = Instantiate(arrowSpawn, spawnPos3, Quaternion.identity);
        Destroy(SummonArrow17, 3f);
        GameObject SummonArrow18 = Instantiate(arrowSpawn, spawnPos4, Quaternion.identity);
        Destroy(SummonArrow18, 3f);
        GameObject SummonArrow19 = Instantiate(arrowSpawn, spawnPos5, Quaternion.identity);
        Destroy(SummonArrow19, 3f);
        GameObject SummonArrow20 = Instantiate(arrowSpawn, spawnPos6, Quaternion.identity);
        Destroy(SummonArrow20, 3f);
        GameObject SummonArrow21 = Instantiate(arrowSpawn, spawnPos7, Quaternion.identity);
        Destroy(SummonArrow21, 3f);
        
    }

    private void castSkillPhase3hard2()
    {
        Vector3 spawnPos1 = new Vector3(transform.position.x - 16.0f, transform.position.y + 9f, transform.position.z);
        Vector3 spawnPos2 = new Vector3(transform.position.x - 9.6f, transform.position.y + 9f, transform.position.z);
        Vector3 spawnPos3 = new Vector3(transform.position.x - 3.2f, transform.position.y + 9f, transform.position.z);
        Vector3 spawnPos5 = new Vector3(transform.position.x + 3.2f, transform.position.y + 9f, transform.position.z);
        Vector3 spawnPos6 = new Vector3(transform.position.x + 9.6f, transform.position.y + 9f, transform.position.z);
        Vector3 spawnPos7 = new Vector3(transform.position.x + 16.0f, transform.position.y + 9f, transform.position.z);
        GameObject SummonArrow1 = Instantiate(arrowSpawn, spawnPos1, Quaternion.identity);
        Destroy(SummonArrow1, 3f);
        GameObject SummonArrow2 = Instantiate(arrowSpawn, spawnPos2, Quaternion.identity);
        Destroy(SummonArrow2, 3f);
        GameObject SummonArrow3 = Instantiate(arrowSpawn, spawnPos3, Quaternion.identity);
        Destroy(SummonArrow3, 3f);
        GameObject SummonArrow4 = Instantiate(arrowSpawn, spawnPos5, Quaternion.identity);
        Destroy(SummonArrow4, 3f);
        GameObject SummonArrow5 = Instantiate(arrowSpawn, spawnPos6, Quaternion.identity);
        Destroy(SummonArrow5, 3f);
        GameObject SummonArrow6 = Instantiate(arrowSpawn, spawnPos7, Quaternion.identity);
        Destroy(SummonArrow6, 3f);
        countHardSkillCheck();
    }


    public void isDead()
    {
        
        Vector3 spawnPos = new Vector3(transform.position.x, transform.position.y, transform.position.z);
        Instantiate(DeadEffectBoss, spawnPos, Quaternion.identity);

        StartCoroutine(destroy());
        animator.SetBool("IsDead", true);
    }

    public IEnumerator destroy()
    {
        yield return new WaitForSeconds(2f);
        Destroy(skill1);
        Destroy(skill2);
        Destroy(skill3);
        Destroy(skill4);
        Destroy(skill5);
        Destroy(gameObject);
    }




    private IEnumerator castPhase2()
    {
        animator.SetBool("IsPhase1", false);
        yield return new WaitForSeconds(3f);
        phase2BossFinal();
    }
    private IEnumerator castPhase3()
    {
        animator.SetBool("IsPhase2", false);
        yield return new WaitForSeconds(3f);
        phase3BossFinal();
    }
    private IEnumerator castPhaseDead()
    {
        animator.SetBool("IsPhase3", false);
        yield return new WaitForSeconds(3f);
        isDead();
    }

    public void phaseBossController()
    {
        if (bossHealth.currentHealth <= 0 && phaseFinalBoss < 4)
        {
            phaseFinalBoss = 4;
            StartCoroutine(castPhaseDead());
        }
        else if (bossHealth.currentHealth <= 1000 && phaseFinalBoss < 3)
        {
            phaseFinalBoss = 3;
            StartCoroutine(castPhase3());

        }
        else if (bossHealth.currentHealth <= 2000 && phaseFinalBoss < 2)
        {
            phaseFinalBoss = 2;
            StartCoroutine(castPhase2());
        }
    }
    private IEnumerator summonMiniBoss()
    {
        Vector3 spawnPos = new Vector3(transform.position.x -7f, transform.position.y, transform.position.z);
        skill6 = Instantiate(BigEffectSummon, spawnPos, Quaternion.identity);
        yield return new WaitForSeconds(0.2f);
        Vector3 spawnPos1 = new Vector3(transform.position.x - 7f, transform.position.y - 6.4f, transform.position.z);
        skill7 = Instantiate(miniBoss, spawnPos1, Quaternion.identity);
    }

}
