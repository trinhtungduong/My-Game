using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RocketBullet : MonoBehaviour
{
    public Rigidbody rb;
    public GameObject bullet;
    public float speed;
    public ParticleSystem hitEffect;
    private bool shooting;
    private float timeExist;
    [HideInInspector]
    public RocketGun gun;
    private void Update()
    {
        if (shooting)
        {
            timeExist += Time.deltaTime;
            if(timeExist > 3f)
            {
                hitEffect.Play(true);
                bullet.SetActive(false);
                shooting = false;
                rb.velocity = Vector3.zero;
            }
        }
    }
    public void Shot(Vector3 dir, RocketGun _gun)
    {
        gun = _gun;
        rb.velocity = dir * speed;
        timeExist = 0f;
        shooting = true;
    }
    private void OnCollisionEnter(Collision collision)
    {
        hitEffect.Play(true);
        bullet.SetActive(false);
        shooting = false;
        rb.velocity = Vector3.zero;
        collision.gameObject.GetComponentInParent<IDamageMonster>()?.TakeDamage(gun.damage);
    }
}
