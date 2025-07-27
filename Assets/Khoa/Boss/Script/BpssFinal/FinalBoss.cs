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
    [SerializeField] GameObject castSkill;
    [SerializeField] GameObject spawnAreaMonster;
    [SerializeField] GameObject effectSkill01;
    [SerializeField] GameObject effectSkill02;
    [SerializeField] GameObject effectSkill03;
    [SerializeField] GameObject effectSkill04;
    [SerializeField] GameObject BigEffectCastSkill;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

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




    private IEnumerator EffectcastSkillPhase2()
    {
        Vector3 spawnPos = new Vector3(transform.position.x +0.7f, transform.position.y + 8f, transform.position.z);
        GameObject BigEffectCastSkill1 = Instantiate(BigEffectCastSkill, spawnPos, Quaternion.identity);
        yield return new WaitForSeconds(2f);
        Vector3 spawnPos1 = new Vector3(transform.position.x + 4.5f, transform.position.y + 8f, transform.position.z);
        Quaternion rotation1 = Quaternion.Euler(0f, 0f, 40.379f);
        Vector3 spawnPos2 = new Vector3(transform.position.x - 4.3f, transform.position.y + 8f, transform.position.z);
        Quaternion rotation2 = Quaternion.Euler(0f, 0f, -28.83f);
        GameObject SkillSummonMagic1 = Instantiate(effectSkill01, spawnPos1, rotation1);
        GameObject SkillSummonMagic2 = Instantiate(effectSkill02, spawnPos2, rotation2);
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
        spawnMonsterArea1();
        yield return new WaitForSeconds(2f);
        spawnMonsterArea2();
        transformSpawnMonster1 -= 3f;
        transformSpawnMonster2 += 3f;
    }
}
