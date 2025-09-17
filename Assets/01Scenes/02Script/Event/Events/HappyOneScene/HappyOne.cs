using UnityEngine;

public class HappyOne : MonoBehaviour
{
    void Start()
    {
        Cursor.visible = false;

        EventBus.Instance.Publish<GameEvents.AspectChange>(new GameEvents.AspectChange(AspectMode.OnepersonMode));
    }
}
