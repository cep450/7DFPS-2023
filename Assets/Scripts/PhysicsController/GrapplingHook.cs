using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class GrapplingHook : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private LineRenderer lr = null;
    [SerializeField] private Transform gunTip = null;

    [Header("Grappling Parameters")]
    [SerializeField] private float maxGrappleDistance = 15;
    [SerializeField] private float grappleDuration = 1;
    [SerializeField] private float overShootYAxis = 1;
    [SerializeField] private LayerMask whatIsGrappleable;

    [Header("Cooldown")]
    [SerializeField] private float grapplingCooldown;


    PlayerPhysics player = null;
    private Vector3 grapplePoint;
    private float grapplingCooldownTimer;
    private bool grappling = false;
    private Transform cameraTransform;
    private WaitForSeconds grappleDelay;

    // Start is called before the first frame update
    void Start()
    {
        player = GetComponent<PlayerPhysics>();
        cameraTransform = GetComponentInChildren<Camera>().transform;
        grappleDelay = new WaitForSeconds(grappleDuration);
    }


    public void StartGrapple(InputAction.CallbackContext context) {
        if (context.phase != InputActionPhase.Performed)
            return;

        if (!player.CanGrapple)
            return;

        if (grappling) {
            Debug.Log("Already grappling");
            return;
        }

        if (grapplingCooldownTimer > 0) {
            return;
        }

        //grappling = true;
        player.ToggleFreeze(true);
        Debug.DrawRay(cameraTransform.position, cameraTransform.forward * maxGrappleDistance, Color.green, 2);
        if (Physics.Raycast(cameraTransform.position, cameraTransform.forward, out RaycastHit hit, maxGrappleDistance)) {
            Debug.Log("hit");
            grapplePoint = hit.point;
            ExectueGrapple(grappleDuration);
            // Invoke grapple
        }
        else {
            grapplePoint = cameraTransform.position + cameraTransform.forward * maxGrappleDistance;
            //Grapple(0, true);
            ExectueGrapple(grappleDuration, false);
            // invoke stop grapple
        }
    }

    private void StartGrapple() {

    }

    private void ExectueGrapple(float grappleTime, bool hit = true) {
        grappling = true;
        lr.enabled = true;
        lr.SetPosition(0, gunTip.position);
        lr.SetPosition(1, grapplePoint);

        if (hit) {
            Vector3 lowestPoint = new Vector3(transform.position.x, transform.position.y - 1f, transform.position.z);

            float grapplePointRelativeYPos = grapplePoint.y - lowestPoint.y;
            float highestPointOnArc = grapplePointRelativeYPos + overShootYAxis;

            if (grapplePointRelativeYPos < 0) highestPointOnArc = overShootYAxis;

            player.GrappleJump(grapplePoint, highestPointOnArc);
        }

        StartCoroutine(Coroutine_Grapple(grappleTime));
    }

    private IEnumerator Coroutine_Grapple(float grappleTime) {

        //yield return grappleDelay;
        Debug.Log("Starting grapple for " + grappleTime + " Seconds");
        while (grappleTime > 0) {
            grappleTime-= Time.deltaTime;
            lr.SetPosition(0, gunTip.position);
            yield return null;
        }
        grappling = false;
        lr.enabled= false;
        player.ToggleFreeze(false);
    }
}
