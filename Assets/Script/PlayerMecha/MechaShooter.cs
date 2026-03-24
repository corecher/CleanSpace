using UnityEngine;

public class MechaShooter : MonoBehaviour
{
    public GameObject bulletPrefab; // 발사할 총알 프리팹
    public Transform firePoint;     // 총알이 생성될 위치 및 방향
    public float bulletSpeed = 30f; // 총알 속도

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Shoot();
        }
    }

    void Shoot()
    {
        GameObject bullet = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
        Rigidbody rb = bullet.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.linearVelocity = firePoint.forward * bulletSpeed;
        }
    }
}
