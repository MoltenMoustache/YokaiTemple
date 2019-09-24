using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestTurret : MonoBehaviour
{
    [SerializeField] GameObject bulletPrefab;
    [SerializeField] float shootInterval;
    bool canShoot = true;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (canShoot)
        {
            StartCoroutine(ShootProjectile());
        }
    }

    IEnumerator ShootProjectile()
    {
        Instantiate(bulletPrefab, transform.position, transform.rotation);
        canShoot = false;
        yield return new WaitForSeconds(shootInterval);
        canShoot = true;
    }
}
