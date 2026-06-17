using UnityEngine;

/// <summary>
/// 상태 3가지
/// </summary>

public enum RobotState
{
    Normal,
    Warning,
    Danger
}
public class RobotStateSystem 
{
    //임계차 값 설정 30,15
    private readonly float _warningValue;
    private readonly float _dangerValue;

    public RobotStateSystem(float warningValue, float dangerValue)
    {
        _warningValue = warningValue;
        _dangerValue = dangerValue;
    }   

    public void Evaluate(RobotModel model)
    {
        if (_dangerValue >= model.battery)
        {
            model.SetState(RobotState.Danger);
        }
        else if (_warningValue >= model.battery)
        {
            model.SetState(RobotState.Warning);
        }
        else
        {
            model.SetState(RobotState.Normal);
        }
    }
}
