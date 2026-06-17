using System;
using UnityEngine;

[CreateAssetMenu(menuName ="Event/RobotUpdate")]
public class RobotEventChannelSO : ScriptableObject
{
    public Action<RobotModel> OnRaised;  //명사
    public void Raise(RobotModel model)//동사
    {
        OnRaised?.Invoke(model);
    }
}
