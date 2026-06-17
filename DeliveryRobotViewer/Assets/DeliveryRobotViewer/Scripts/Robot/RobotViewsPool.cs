using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;
/// <summary>
/// 움직이는 로봇 풀
/// </summary>
public class RobotViewsPool : IObjectPool<RobotMoveItem>
{
    readonly RobotMoveItem _prefab;
    readonly Transform _prefabParent;
    readonly ObjectPool<RobotMoveItem> _pool; //UnityEngine.Pool사용  

    public RobotViewsPool(RobotMoveItem prefab, Transform prefabParent)
    {
        _prefab = prefab;
        _prefabParent = prefabParent;

        _pool = new ObjectPool<RobotMoveItem>(
            CreateFunc,
            OnGet,
            OnRelease,
            OnDestroy,
            collectionCheck: false,
            defaultCapacity: 5,
            maxSize: 20);
    }

    public RobotMoveItem CreateFunc()
    {
        var view = Object.Instantiate(_prefab, _prefabParent);
        return view;
    }

    public void OnGet(RobotMoveItem item)//꺼내기 액션
    {
        item.gameObject.SetActive(true);
    }
    public void OnRelease(RobotMoveItem item)//끝나고 넣기 액션
    {
        //item.reset//아이템 리셋 부분
        item.gameObject.SetActive(false);
    }
    public void OnDestroy(RobotMoveItem item)//
    {
        Object.Destroy(item.gameObject);
    }
    //인터페이스 정의
    public RobotMoveItem Get() => _pool.Get();//꺼낼곳
    public void Release(RobotMoveItem item) => _pool.Release(item);//넣을곳
    //디버깅용
    public int CountInactive => _pool.CountInactive;
    public int CountAll => _pool.CountAll;
}
