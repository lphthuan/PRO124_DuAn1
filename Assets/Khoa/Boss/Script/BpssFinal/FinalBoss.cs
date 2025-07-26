using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FinalBoss : MonoBehaviour
{
    public Animator animator;
    public int phaseFinalBoss = 0;
    [SerializeField] GameObject castSkill;
    [SerializeField] GameObject spawnAreaMonster;
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
        animator.SetBool("Phase1", true);
    }
    public void CastSkillSummonMagic1()
    {
        Vector3 spawnPos = new Vector3(transform.position.x - 7.5f, transform.position.y + 4f, transform.position.z);
        GameObject SkillSummonMagic1 = Instantiate(castSkill, spawnPos, Quaternion.identity);
        Destroy(SkillSummonMagic1, 1.7f);
    }
    public void CastSkillSummonMagic2()
    {
        Vector3 spawnPos = new Vector3(transform.position.x + 7.5f, transform.position.y + 4f, transform.position.z);
        GameObject SkillSummonMagic2 = Instantiate(castSkill, spawnPos, Quaternion.identity);
        Destroy(SkillSummonMagic2, 1.7f);
    }
    public void spawnMonsterArea1()
    {
        Vector3 spawnPos = new Vector3(transform.position.x - 15f, transform.position.y - 5.5f, transform.position.z);
        GameObject spawnMonsterArea1 = Instantiate(spawnAreaMonster, spawnPos, Quaternion.identity);
        Destroy(spawnMonsterArea1, 5f);
    }

    public void spawnMonsterArea2()
    {
        Vector3 spawnPos = new Vector3(transform.position.x + 15f, transform.position.y - 5.5f, transform.position.z);
        GameObject spawnMonsterArea2 = Instantiate(spawnAreaMonster, spawnPos, Quaternion.identity);
        Destroy(spawnMonsterArea2, 5f);
    }





    private IEnumerator castSkillPhase1()
    {
        CastSkillSummonMagic1();
        yield return new WaitForSeconds(2f);
        CastSkillSummonMagic2();
        yield return new WaitForSeconds(2f);
        spawnMonsterArea1();
        yield return new WaitForSeconds(2f);
        spawnMonsterArea2();
    }
}
