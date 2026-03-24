using UnityEngine;
using System.Collections;
public class VacuityP : Enemy
{
    public GameObject warningLine;
    public GameObject bacuity;
    public override IEnumerator Execute(EnemyController brain)
    {
        warningLine.SetActive(true);
        yield return new WaitForSeconds(2f);
        warningLine.SetActive(false);
        yield return new WaitForSeconds(0.5f);
        bacuity.SetActive(true);
        yield return new WaitForSeconds(5f);
        bacuity.SetActive(false);
        brain.StateChange(EnemyController.EnemyState.Idle);
    }
}
