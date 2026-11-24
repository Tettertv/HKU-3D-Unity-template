using UnityEngine;
using Unity.Cinemachine;
using System.Collections.Generic;

public class CombatTargetFinder : MonoBehaviour
{
    public static List<Transform> enemyTargetPool = new List<Transform>();

    private bool lockedOn;
    private Transform currentTarget;
    [SerializeField] private CinemachineCamera rotationalCamera;
    [SerializeField] private CinemachineCamera focusCamera;
    [SerializeField] private PlayerMovement playerMovementController;
    [SerializeField] private PlayerCombat playerCombatController;

    [SerializeField] private List<Transform> inspectorPool; //Just to see the pool in the editor

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
        if (enemyTargetPool == null)
        {
            enemyTargetPool = new List<Transform>();
        }

        if (target != null && !enemyTargetPool.Contains(target))
        {
            enemyTargetPool.Add(target);
        }
    }

    public static void RemoveFromTargetPool(Transform target)
    {
        if (enemyTargetPool == null || target == null)
        {
            return;
        }

        enemyTargetPool.Remove(target);
    }

    public void RetryCombatLock() //not sure what i wanted here yo
    {

    }

    private void LockOn() //The logic in here and in LockOFf() would probably better be if moved to a different camera script instead of being on the player like this with hard references huh?
    {
        if (currentTarget)
        {

            focusCamera.LookAt = currentTarget;
            focusCamera.gameObject.SetActive(true);

            rotationalCamera.gameObject.SetActive(false);
            lockedOn = true;

            AssignLockInfo();
        }
        else
        {
            Debug.Log("Tried to Lock on but no target at the momentl");
            //Debug, not really bad for code to get here though.
        }
    }

    private void LockOff()
    {
        if (!lockedOn)
        {
            return;
        }

        AllignRotationalCamera();

        focusCamera.LookAt = null;
        focusCamera.gameObject.SetActive(false);

        rotationalCamera.gameObject.SetActive(true);
        lockedOn = false;

        AssignLockInfo();
    }

    private void AllignRotationalCamera() //Also Different script Duh()
    {
        Vector3 cameraForward = Camera.main.transform.forward;

        float horizontal = Mathf.Atan2(cameraForward.x,cameraForward.z) * Mathf.Rad2Deg;

        float vertical = Mathf.Asin(cameraForward.y) * Mathf.Rad2Deg;

        rotationalCamera.GetComponent<CinemachineOrbitalFollow>().HorizontalAxis.Value = horizontal;
        rotationalCamera.GetComponent<CinemachineOrbitalFollow>().VerticalAxis.Value = vertical;
    }

    private void AssignLockInfo()
    {
        playerMovementController.ReceiveLockInfo(currentTarget, lockedOn);
        playerCombatController.ReceiveLockInfo(currentTarget, lockedOn);
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


