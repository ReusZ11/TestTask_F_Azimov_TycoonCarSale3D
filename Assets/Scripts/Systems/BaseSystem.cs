using UnityEngine;

public abstract class BaseSystem : MonoBehaviour
{
    protected virtual void Awake()
    {
        Initialize();
    }


    public virtual void Initialize() {}

    public virtual void SaveState() {}

    public virtual void LoadState() {}
}