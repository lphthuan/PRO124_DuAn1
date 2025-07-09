using UnityEngine;

[CreateAssetMenu(fileName = "New Spell", menuName = "Spell/Spell Data")]
public class SpellData : ScriptableObject
{
	public string spellName;
	public GameObject spellPrefab;
	public float spellSpeed = 10f;
	public string animationTrigger; // VD: "Cast_Fire"
	public Sprite icon;
}
