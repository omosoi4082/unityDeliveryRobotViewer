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
        _registy.UpdateRobotDate(dTO.robotId,
            dTO.battery,
            new Vector3(dTO.px, dTO.py, dTO.pz),
            Quaternion.Euler(0, dTO.yaw, 0),//회전값 넣을때 사용
            dTO.hsaPayload);
    }
}
