using System.Collections;
using UnityEngine;

public abstract class Enemy : MonoBehaviour
{
    public abstract IEnumerator Execute(EnemyController brain);
}
