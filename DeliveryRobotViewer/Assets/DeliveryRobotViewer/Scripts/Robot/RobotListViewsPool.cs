using UnityEngine;
using UnityEngine.Pool;

public class RobotListViewsPool : IObjectPool<RobotListItem>
{
    readonly RobotListItem _prefab;
    readonly Transform _prefabParent;  
    readonly ObjectPool<RobotListItem> _listPool;   
    public RobotListViewsPool(RobotListItem prefab, Transform prefabParent)
    {
        _prefab = prefab;
        _prefabParent = prefabParent;

        _listPool = new ObjectPool<RobotListItem>(
            OnCreate,
            OnGet,
            OnRelease,
            OnDestory,
            collectionCheck: false,
            defaultCapacity: 5,
            maxSize: 20
            );
    }
    public RobotListItem OnCreate()
    {
        return GameObject.Instantiate(_prefab, _prefabParent);
    }

    public void OnGet(RobotListItem item)
    {
        item.gameObject.SetActive( true );  
    }
    public void OnRelease(RobotListItem item)
    {
        item.gameObject.SetActive(false);
    }
    public void OnDestory(RobotListItem item)
    {
       Object.Destroy( item.gameObject );
    }


    public RobotListItem Get()=>_listPool.Get();    
   

    public void Release(RobotListItem item)=>_listPool.Release(item);
   

}
