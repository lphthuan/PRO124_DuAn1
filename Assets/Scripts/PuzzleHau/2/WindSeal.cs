using UnityEngine;

public class WindSeal : MonoBehaviour
{
    [Header("Tường gió cần tắt")]
    public WindZone windZone;

    private static int sealsBroken = 0;
    private static int totalSeals = 4;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("WindSpell"))
        {
            Destroy(gameObject);
            sealsBroken++;

            if (sealsBroken >= totalSeals && windZone != null)
            {
                windZone.StopWind();
                sealsBroken = 0; 
            }
        }
    }
}
