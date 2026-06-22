using UnityEngine;

public class DataMapper 
{
   private readonly RobotRegisty _registy;  
    public DataMapper(RobotRegisty registy)
    {
        _registy = registy;   
    }

    public void Apply(RobotDTO dTO)
    {

        float unityX = dTO.px ;
        float unityZ = dTO.py ;
        float yawDeg = dTO.yaw -90f;
        float reb = dTO.battery * 100f;
        _registy.UpdateRobotDate(
            dTO.robotId,
            reb,
            new Vector3(unityX, 0f, unityZ),
            Quaternion.Euler(0f, yawDeg, 0f),
            dTO.state
        );
        /*_registy.UpdateRobotDate(
            dTO.robotId,
            dTO.battery,
            new Vector3(dTO.px, dTO.py, dTO.pz),
            Quaternion.Euler(0, dTO.yaw, 0),//회전값 넣을때 사용
            dTO.state
           );*/
    }
}
