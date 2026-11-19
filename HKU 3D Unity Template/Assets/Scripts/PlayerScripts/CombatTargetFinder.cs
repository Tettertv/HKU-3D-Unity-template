using UnityEngine;
using Unity.Cinemachine;
using System.Collections.Generic;

public class CombatTargetFinder : MonoBehaviour
{
    public static List<Transform> enemyTargetPool;

    private bool lockedOn;
    private Transform currentTarget;
    [SerializeField] private CinemachineCamera rotationalCamera;
    [SerializeField] private CinemachineCamera focusCamera;

    [SerializeField] private List<Transform> inspectorPool; //Just to see the pool in the editor

    private void Start()
    {
        if ( enemyTargetPool == null)
        {
            enemyTargetPool = new List<Transform>();
        }
    }
    private void OnEnable()
    {
        if (InputManager.Instance == null)
        {
            Debug.Log("No Input manager found :(");
            return;
        }
        InputManager.Instance.OnCombatLock += TryCombatLock;
        InputManager.Instance.OnCombatLockOff += LockOff;
    }

    private void OnDisable()
    {
        if (InputManager.Instance == null)
        {
            return;
        }
        InputManager.Instance.OnCombatLock -= TryCombatLock;
        InputManager.Instance.OnCombatLockOff -= LockOff;
    }

    private void Update()
    {
        inspectorPool = enemyTargetPool;
    }

    private void TryCombatLock()
    {
        Debug.Log("I am trying to lock the camera");

        currentTarget = GetCenterTarget();

        if (currentTarget != null)
        {
            LockOn();
        }
        else
        {
            if (!lockedOn)
            {
                Debug.Log("Im already locked off");
                return;
            }
            LockOff();
        }
    }

    public static void AddToTargetPool(Transform target)
    {
        if (!enemyTargetPool.Contains(target) && enemyTargetPool != null)
        {
            enemyTargetPool.Add(target);
        }
    }

    public static void RemoveFromTargetPool(Transform target)
    {
        if (enemyTargetPool.Contains(target) && enemyTargetPool != null)
        {
            enemyTargetPool.Remove(target);
        }
    }

    public void RetryCombatLock()
    {

    }

    private void LockOn()
    {
        if (currentTarget)
        {
            focusCamera.LookAt = currentTarget;
            focusCamera.gameObject.SetActive(true);

            rotationalCamera.gameObject.SetActive(false);
            lockedOn = true;
        }
    }

    private void LockOff()
    {
        focusCamera.LookAt = null;
        focusCamera.gameObject.SetActive(false);

        rotationalCamera.gameObject.SetActive(true);
        lockedOn = false;
    }

    private Transform GetCenterTarget() //gets the target closest to the center of the screen
    {
        if (enemyTargetPool == null ||  enemyTargetPool.Count == 0)
        {
            Debug.Log("Either No TargetPool or Nothing in there");
            return null;
        }

        Vector3 center = new Vector3(0.5f, 0.5f, 0f);
        Camera cam = Camera.main;

        int minIndex = 0;
        float shortestDistance = 1000f;

        for(int i = 0; i < enemyTargetPool.Count; i++)
        {
            if(enemyTargetPool[i] != currentTarget)
            {
                Vector3 targetViewport = cam.WorldToViewportPoint(enemyTargetPool[i].position);
                targetViewport -= Vector3.forward * targetViewport.z; //Removes the Depth part of it for now?

                float targetDistanceFromCenter = Vector3.Distance(targetViewport, center);

                if (targetDistanceFromCenter < shortestDistance)
                {
                    shortestDistance = targetDistanceFromCenter;
                    minIndex = i;
                }
            }
        }

        return enemyTargetPool[minIndex];
    }

}


