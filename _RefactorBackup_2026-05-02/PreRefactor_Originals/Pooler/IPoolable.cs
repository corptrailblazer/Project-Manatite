// IPoolable.cs (optional hook)
public interface IPoolable
{
    void OnSpawned();
    void OnDespawned();
}
