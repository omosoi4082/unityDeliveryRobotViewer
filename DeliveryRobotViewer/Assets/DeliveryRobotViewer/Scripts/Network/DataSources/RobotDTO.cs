using System;
using UnityEngine;
/// <summary>
/// 데이터 상자
/// </summary>
[Serializable]
public class RobotDTO 
{
    public string robotId;
    public float battery;
    public float px,py,pz;
    public float yaw;
    public bool hsaPayload;
}

[Serializable]
public class PinkyStateWrapper
{
    public PinkyState msg;
}

[Serializable]
public class PinkyState
{
    public Header header;
    public string robot_name;
    public string map_name;
    public float[] pose;   // [x, y, theta]
    public float battery_soc;
    public string state;
    public string nav2_state;
    public bool available;
    public bool emergency;
    public bool command_active;
    public string active_request_id;
    public string message;
}
[Serializable]
public class Header
{
    public Stamp stamp;
    public string frame_id;
}

[Serializable]
public class Stamp
{
    public int sec;
    public int nanosec;
}