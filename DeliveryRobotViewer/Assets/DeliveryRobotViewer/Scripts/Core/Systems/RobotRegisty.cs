using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// 등록 관리
/// </summary>
public class RobotRegisty
{
    public readonly Dictionary<string, RobotModel> robotModels = new Dictionary<string, RobotModel>();
    private readonly RobotPresenterFactory presenterFactory;
    private readonly RobotEventChannelSO eventChannelSO;
    private readonly RobotStateSystem _stateSystem;
    private readonly LivenessSystem livenessSystem; 
    public RobotRegisty(RobotStateSystem stateSystem, RobotPresenterFactory factory, RobotEventChannelSO robotEvent)
    {
        _stateSystem = stateSystem;
        presenterFactory = factory;
        eventChannelSO = robotEvent;
        livenessSystem=new LivenessSystem();    
    }

    private RobotModel GetOrCreate(string id)
    {
        if (robotModels.TryGetValue(id, out var value))
        {
            return value;
        }
        var robotModel = new RobotModel(id);
        robotModels.Add(id, robotModel);
        return robotModel;
    }

    public void UpdateRobotDate(string id, float battery, Vector3 pos, Quaternion quaternion,string state ,bool paylaod=false)
    {
        var model = GetOrCreate(id);
        presenterFactory.PresenterGetOrCreate(model);
        model.UpdateRobotInfo(battery, pos, quaternion, state, paylaod);
       
        _stateSystem.Evaluate(model);
        eventChannelSO.Raise(model);

    }
    public IEnumerable<RobotModel> GetAll()=>robotModels.Values;
}
