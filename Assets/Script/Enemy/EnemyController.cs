using System.Collections;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    public Transform player;
    public float rotationSpeed = 5f;
    public Enemy[] patterns;
    public enum EnemyState
    {
        Idle,
        Attack,
        Die
    }
    EnemyState state;
    public int hp = 100;
    void Start()
    {
        StateChange(EnemyState.Idle);
    }
    void Update()
    {
        if(state == EnemyState.Idle)
           LookPlayer();
        
    }
    public void PlayPattern(int index)
    {
        if (index >= 0 && index < patterns.Length && patterns[index] != null)
        {
            StartCoroutine(patterns[index].Execute(this));
        }
        else
        {
            Debug.LogWarning("해당 슬롯에 패턴 스크립트가 없습니다!");
        }
    }
    public void TakeDamage(int damage)
    {
        hp-=damage;
        if(hp<=0)
        {
            Die();
        }
    }
    void Die()
    {
        Destroy(gameObject);
    }
    void LookPlayer()
    {
        if (player != null)
        {
            Vector3 direction = player.position - transform.position;
            direction.y = 0; 
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }
    }
    public void StateChange(EnemyState newState)
    {
        state = newState;
        switch(state)
        {
           case EnemyState.Idle : StartCoroutine(BreakTime());break;
           case EnemyState.Attack :PlayPattern(Random.Range(0,patterns.Length)); break;
           case EnemyState.Die : break;
        }
    }
    IEnumerator BreakTime()
    {
        yield return new WaitForSeconds(3f);
        StateChange(EnemyState.Attack);
    }
}
