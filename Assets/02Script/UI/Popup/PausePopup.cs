using UnityEngine;
using UnityEngine.UI;

public class PausePopup : MonoBehaviour
{
    [Header("UI Refs")]
    [SerializeField] private Button resume;
    [SerializeField] private Button option;
    [SerializeField] private Button temp;
    [SerializeField] private Button quit;

    private PauseMode pm;

    private void Awake()
    {
        pm = FindAnyObjectByType<PauseMode>();
        if(pm == null) Debug.Log("PausePopup - Failed to Load PauseMode");
    }

    private void OnEnable()
    {
        resume.onClick.AddListener(pm.ExitPause);
    }
    private void OnDisable()
    {
        resume.onClick.RemoveListener(pm.ExitPause);
    }
}
