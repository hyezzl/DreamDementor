using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

public class EyeEffect : MonoBehaviour
{
    [SerializeField] private PostProcessVolume pp;
    [SerializeField] private Canvas eyecanvas;

    private DepthOfField dof;

    public void Effect01() {
        // ¡¶¿œ »Â∏¥
        if (pp.profile.TryGetSettings(out dof)) {
            dof.focusDistance.value = 0.1f;
            dof.aperture.value = 4.8f;
            dof.focalLength.value = 10f;
        }
    }

    public void Effect02() {
        if (pp.profile.TryGetSettings(out dof)) {
            dof.focusDistance.value = 0.22f;
            dof.aperture.value = 3.3f;
            dof.focalLength.value = 10f;
        }
    }

    public void EndTimeline() {
        eyecanvas.gameObject.SetActive(false);
        if (pp.profile.TryGetSettings(out dof)) {
            dof.focusDistance.value = 5f;
            dof.aperture.value = 2f;
        }

        pp.gameObject.SetActive(false);
    }

}
