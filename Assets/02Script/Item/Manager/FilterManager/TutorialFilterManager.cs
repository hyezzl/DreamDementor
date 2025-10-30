using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialFilterManager : FilterManager
{
    //[Header("Filter Refs")]
    //[SerializeField] CameraFilterPack_TV_MovieNoise oldMovieFilter;
    //[SerializeField] CameraFilterPack_FX_Glitch1 glitchFilter;

    CameraFilterPack_TV_MovieNoise oldMovieFilter;
    CameraFilterPack_FX_Glitch1 glitchFilter;

    protected override void Awake()
    {
        base.Awake();

        oldMovieFilter = mainCam?.GetComponent<CameraFilterPack_TV_MovieNoise>();
        if (oldMovieFilter == null) Debug.Log("TutorialFilterManager - Failed to Load OldMovieFilter");

        glitchFilter = mainCam?.GetComponent<CameraFilterPack_FX_Glitch1>();
        if(glitchFilter == null) Debug.Log("TutorialFilterManager - Failed to Load GlitchFilter");
    }

    protected override void CamFilterOn(GameEvents.FilterOn evt)
    {
        switch (evt.type) {
            case FilterType.OldMovie:
                if (evt.isLasting)
                {
                    oldMovieFilter.enabled = true;
                }
                else if (!evt.isLasting)
                {
                    StartCoroutine(FiniteFilter(oldMovieFilter, evt.duration));
                }
                else return;
                break;

            case FilterType.Glitch:

                if (evt.isLasting)
                {
                    glitchFilter.enabled = true;
                }
                else if (!evt.isLasting)
                {
                    StartCoroutine(FiniteFilter(glitchFilter, evt.duration));
                }
                else return;
                break;
        }

    }
}
