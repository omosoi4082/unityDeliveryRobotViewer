using System.Collections.Generic;
using UnityEngine;

public class RobotPresenterFactory 
{
    public RobotViewsPool _robotViewsPool;//움직이는 로봇 pool
    public RobotListViewsPool _robotListViewsPool;//ui리스트 pool    
    private readonly RobotEventChannelSO _eventChannelSO;//이벤트 

   public Dictionary<string, RobotPresenter> robots=new();
    public RobotPresenter _currentPresenter;
    public RobotPresenterFactory(RobotViewsPool robotViewsPool, RobotListViewsPool robotListViewsPool, RobotEventChannelSO eventChannelSO)
    {
        _robotViewsPool = robotViewsPool;
        _robotListViewsPool = robotListViewsPool;
        _eventChannelSO = eventChannelSO;
        _eventChannelSO.OnRaised += OnRobotUpdate;

    }   

    public void PresenterGetOrCreate(RobotModel model) 
    {
        if(robots.TryGetValue(model.robotId, out var robot))
        {
            return;
        }
        var move = _robotViewsPool.Get();
        var list=_robotListViewsPool.Get();

        var presenter = new RobotPresenter(list, move,this, model);
        robots.Add(model.robotId, presenter);
        
    }

    public void OnRobotUpdate(RobotModel model)
    {
        if (!robots.TryGetValue(model.robotId, out var robot))
        {
            return;
        }
      
        robot.AllUPdate(model);
    }
    public void SelcetPresent(RobotPresenter presenter)
    {
        if(_currentPresenter==presenter)return;
       
        _currentPresenter?.Deselect();
        _currentPresenter = presenter;
        _currentPresenter.Select();
    }

    public void Dispose()
    {
        _eventChannelSO.OnRaised -= OnRobotUpdate;
    }
}
