
public interface IActionNpc
{
    int GetNpcID();

    void Init(IDatabase db);

    void Interact();
}
