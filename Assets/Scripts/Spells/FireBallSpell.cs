using UnityEngine;

public class FireBallSpell : SpellPage
{
    [SerializeField] private GameObject fireballPrefab;
    [SerializeField] private float speed = 10f;

    protected override void ExecuteSpell(GameObject caster)
    {
        GameObject fireball = Instantiate(fireballPrefab, caster.transform.position, Quaternion.identity);
        Rigidbody2D rb = fireball.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.linearVelocity = caster.transform.right * speed;
        }
    }
}
