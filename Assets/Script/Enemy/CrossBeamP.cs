using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class CrossBeamP : Enemy
{
    public GameObject warningPrefab;
    public GameObject beamPrefab;
    public float telegraphTime = 1.5f;
    public float beamDuration = 2.0f;
    public float beamOffsetHeight = 1.0f;
    public float maxBeamLength = 20f;     // 빔의 최종 길이 (Z축 스케일 목표값)
    public float growthDuration = 0.5f;
    public override IEnumerator Execute(EnemyController brain)
    {
        
        Vector3 spawnPosition = brain.transform.position + Vector3.up * beamOffsetHeight;
        float[] angles = { 0f, 90f };
        List<GameObject> warningLines = new List<GameObject>();

        foreach (float angle in angles)
        {
            Quaternion rotation = Quaternion.Euler(0f, angle, 0f);
            GameObject warning = Instantiate(warningPrefab, spawnPosition, rotation);
            
            // 경고선은 서서히 길어질 필요 없이, 처음부터 최대 길이(maxBeamLength)로 X축을 늘려줍니다.
            Vector3 warningScale = warning.transform.localScale;
            warningScale.x = maxBeamLength; 
            warning.transform.localScale = warningScale;

            warningLines.Add(warning);
        }

        // telegraphTime(예: 1.5초) 동안 경고선 유지하며 대기
        yield return new WaitForSeconds(telegraphTime);

        foreach (GameObject warning in warningLines)
        {
            if (warning != null) Destroy(warning);
        }

        // 실제 빔 소환 및 길어지기 코루틴 실행
        List<GameObject> spawnedBeams = new List<GameObject>();

        foreach (float angle in angles)
        {
            Quaternion beamRotation = Quaternion.Euler(0f, angle, 0f);
            GameObject beam = Instantiate(beamPrefab, spawnPosition, beamRotation);
            spawnedBeams.Add(beam);

            // X축으로 길어지는 코루틴 실행
            StartCoroutine(GrowBeamRoutine(beam.transform));
        }

        yield return new WaitForSeconds(growthDuration + beamDuration);

        foreach (GameObject beam in spawnedBeams)
        {
            if (beam != null) Destroy(beam);
        }
        brain.StateChange(EnemyController.EnemyState.Idle);
    }

    // --- X축으로 빔을 서서히 길어지게 만드는 코루틴 ---
    IEnumerator GrowBeamRoutine(Transform beamTransform)
    {
        Vector3 initialScale = beamTransform.localScale;
        initialScale.x = 0f; // X축 길이 0에서 시작
        beamTransform.localScale = initialScale;

        Vector3 targetScale = initialScale;
        targetScale.x = maxBeamLength; // 목표 길이는 maxBeamLength

        float elapsedTime = 0f;

        while (elapsedTime < growthDuration)
        {
            elapsedTime += Time.deltaTime;
            float progress = elapsedTime / growthDuration;
            
            beamTransform.localScale = Vector3.Lerp(initialScale, targetScale, progress);
            yield return null;
        }

        beamTransform.localScale = targetScale;
    }

}

