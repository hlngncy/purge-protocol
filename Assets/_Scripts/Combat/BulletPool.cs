using UnityEngine;
using UnityEngine.Pool;
using UnityEngine.Serialization;

public class BulletPool : MonoBehaviour
{
    [SerializeField] private ProjectileVisual _projectile;
    private ObjectPool<ProjectileVisual> _pool;
    private Transform _owner;
    private Vector3 _hitPosition;
    
    
    void Start()
    {
        _pool = new ObjectPool<ProjectileVisual>(
            () => { return Instantiate(_projectile); },
            GetDamageTextFromPool,
            ReleaseDamageTextFromPool,
            text => { Destroy(text.gameObject); }, 
            false, 
            10,
            20);
        _pool.Get();
    }

    private void GetDamageTextFromPool(ProjectileVisual obj)
    {
        obj.transform.position = _owner.transform.position;
        obj.transform.rotation = _owner.transform.rotation;
        obj.SetHit(_hitPosition,default,false, this);
    }
    
    private void ReleaseDamageTextFromPool(ProjectileVisual obj)
    {
        //idk
    }

    public void GetProjectile(Transform owner, Vector3 hitPosition)
    {
        _owner = owner;
        _hitPosition = hitPosition;
        _pool.Get();
        Debug.Log("BULLET GET");
    }

    public void ReleaseProjectile(ProjectileVisual projectile)
    {
        _pool.Release(projectile);
    }
    
}
