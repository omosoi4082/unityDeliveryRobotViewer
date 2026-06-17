using System.Collections.Concurrent;
using UnityEngine;

public class RobotDataQueue 
{
   private readonly ConcurrentQueue<RobotDTO> queue = new ConcurrentQueue<RobotDTO>();

    public void Enqueue(RobotDTO dTO)
    {
        if(dTO!=null)queue.Enqueue(dTO);
    }

    public bool TryDequeue(out RobotDTO dTO)
    {
        return queue.TryDequeue(out dTO);   //queue에 반환값이 있으면 참
    }
}
