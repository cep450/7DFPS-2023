using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.Rendering;

public class GrapplingRope : MonoBehaviour
{
    private Spring spring;
    private LineRenderer lr = null;
    private Vector3 currentGrapplePosition;
    [SerializeField] GrapplingHook grapplingHook = null;
    [SerializeField] int quality = 2;
    [SerializeField] float damper = 1;
    [SerializeField] float strength = 1;
    [SerializeField] float veloctiy = 1;
    [SerializeField] float waveCount = 2;
    [SerializeField] float waveHeight = 2;
    [SerializeField] AnimationCurve effectCurve = null;
    PlayerPhysics player = null;

    // Start is called before the first frame update
    void Start()
    {
        lr = GetComponent<LineRenderer>();
        spring = new();
        spring.SetTarget(0); 
        player = GetComponentInParent<PlayerPhysics>();
    }

    private void LateUpdate() {
        DrawRope();
    }

    void DrawRope() {
        if (!player.IsGrappling && !grapplingHook.IsGrappling) {
            currentGrapplePosition = grapplingHook.GrappleGunTipPos;
            spring.Reset();
            if (lr.positionCount > 0) {
                lr.positionCount = 0;
            }
            return;
        }

        if (lr.positionCount < quality+1) {
            lr.positionCount = quality + 1;
            spring.SetVelocity(veloctiy);
        }

        spring.SetDamper(damper);
        spring.SetStrength(strength);
        spring.Update(Time.deltaTime);

        var grapplePoint = grapplingHook.GetGrapplePoint();
        var gunTipPos = grapplingHook.GrappleGunTipPos;

        var up = Quaternion.LookRotation((grapplePoint - gunTipPos).normalized) * Vector3.up;

        currentGrapplePosition = Vector3.Lerp(currentGrapplePosition, grapplePoint, Time.deltaTime*12);

        for (int i = 0; i < quality+1; i++) {
            float delta = i/ (float)quality;
            Vector3 offset = up * waveHeight * Mathf.Sin(delta * waveCount * Mathf.PI) * spring.Value * effectCurve.Evaluate(delta);

            lr.SetPosition(i, Vector3.Lerp(gunTipPos, currentGrapplePosition, delta) + offset);
        }

       // lr.SetPosition(quality, grapplePoint);
    }
}
