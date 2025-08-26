public interface IPoolable
{
    void OnSpawn();   // 对象激活时调用
    void OnDespawn(); // 对象回收时调用
}
