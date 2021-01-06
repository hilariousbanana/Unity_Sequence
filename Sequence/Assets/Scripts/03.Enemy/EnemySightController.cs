using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySightController : MonoSingleton<EnemySightController>
{
    public float ViewAngle;
    public float ViewDistance;

    public LayerMask TargetMask;
    public LayerMask ObstacleMask;

    private Transform enemyTransform;

    [SerializeField]
    private Transform rayTransform;
    private bool bReadyToFind = false;

    private void Awake()
    {
        enemyTransform = GetComponent<Transform>();
        enemy = GetComponent<Enemy>();
    }


    // Update is called once per frame
    void Update()
    {
        DrawView();
        FindVisibleTargets();
    }

    public Vector3 DirFromAngle(float angleInDegrees)
    {
        angleInDegrees += transform.eulerAngles.y;

        return new Vector3(Mathf.Sin(angleInDegrees * Mathf.Deg2Rad), 0, Mathf.Cos(angleInDegrees * Mathf.Deg2Rad));
    }

    public void DrawView()
    {
        Vector3 leftBoundary = DirFromAngle(-ViewAngle / 2);
        Vector3 rightBoundary = DirFromAngle(ViewAngle / 2);
        Debug.DrawLine(rayTransform.position, enemyTransform.position + leftBoundary * ViewDistance, Color.blue);
        Debug.DrawLine(rayTransform.position, enemyTransform.position + rightBoundary * ViewDistance, Color.blue);
    }

    public void FindVisibleTargets()
    {
        Collider[] targets = Physics.OverlapSphere(enemyTransform.position, ViewDistance, TargetMask);

        for (int i = 0; i < targets.Length; i++)
        {
            Transform target = targets[i].transform;
            
            Vector3 dirToTarget = (target.position - enemyTransform.position).normalized;

            if (Vector3.Dot(enemyTransform.forward, dirToTarget) > Mathf.Cos((ViewAngle / 2) * Mathf.Deg2Rad))
            {
                float distToTarget = Vector3.Distance(enemyTransform.position, target.position);
                if (!Physics.Raycast(enemyTransform.position, dirToTarget, distToTarget, ObstacleMask))
                {
                    Debug.DrawLine(rayTransform.position, target.position, Color.red);
                    enemy.SetPlayerInRange(true);
                    enemy.SetPlayerTransform(target);
                    return;
                }
            }
        }
        enemy.SetPlayerInRange(false);
        return;
    }

    public void SetReadyToFindEnemy(bool _ReadyToFind)
    {
        bReadyToFind = _ReadyToFind;
    }
}
