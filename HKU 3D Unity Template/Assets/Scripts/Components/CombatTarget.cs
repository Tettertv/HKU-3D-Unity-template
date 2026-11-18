using UnityEngine;

public class CombatTarget : MonoBehaviour
{
    private Collider collider;
    private Camera activeCam; //This is the active camera

    private Plane[] frustrum;
    private bool visible; //Might not need this but can't hurt that much
    private bool inPool;

    private void Start()
    {
        collider = GetComponent<Collider>();
        activeCam = Camera.main;
    }
    private void Update()
    {
        CheckIfInFrame();
    }

    private void CheckIfInFrame()
    {
        frustrum = GeometryUtility.CalculateFrustumPlanes(activeCam);

        visible = GeometryUtility.TestPlanesAABB(frustrum, collider.bounds);



        if (visible && !inPool) //This whole inPool variable is so that List.Add() isnt run every frame, but that might not be needed if im not running this function every frame
        {
            CombatTargetFinder.AddToTargetPool(this.transform);
            inPool = true;
        }
        else
        {
            if (!visible)
            {
                CombatTargetFinder.RemoveFromTargetPool(this.transform);
                inPool = false;
            }
        }
    }
}
