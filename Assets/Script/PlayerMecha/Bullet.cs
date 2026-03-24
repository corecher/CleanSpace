using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float lifeTime = 10f; // 3초 뒤 자동 파괴

    void Start()
    {
        Destroy(gameObject, lifeTime);
    }
    void OnCollisionEnter(Collision collision)
    {
        EnemyController enemy = collision.gameObject.GetComponent<EnemyController>();

        if(enemy!=null)
        enemy.TakeDamage(1);
        
        Destroy(gameObject);
    }
}
