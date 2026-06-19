using System;
using UnityEngine;
/// <summary>
/// 데이터 상자
/// </summary>
/*[Serializable]
public class RobotDTO 
{
    public string robotId;
    public float battery;
    public float px,py,pz;
    public float yaw;
    public bool hsaPayload;
}*/

[Serializable]
public class RobotDTO
{
    public string robotId;
    public string state;
    public float battery;
    public float px, py, pz;
    public float yaw;
    public bool hsaPayload;
}
