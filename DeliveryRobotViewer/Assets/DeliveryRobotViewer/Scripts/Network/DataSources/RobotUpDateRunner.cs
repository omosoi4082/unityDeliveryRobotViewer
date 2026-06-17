using Cysharp.Threading.Tasks;
using System;
using System.Threading;
using UnityEngine;

public class RobotUpDateRunner 
{
  private readonly RobotDataQueue _dataQueue;   
    private readonly DataMapper _mapper;    
    public RobotUpDateRunner(RobotDataQueue queue,DataMapper mapper)
    {
        _dataQueue = queue;
        _mapper = mapper;
    }

    public void StartRun(CancellationToken token)
    {
        RnuAsync(token).Forget();
    }

    private async UniTask RnuAsync(CancellationToken token)
    {
        try
        {
            while(!token.IsCancellationRequested)//연결중
            {
                while (_dataQueue.TryDequeue(out var dTO))
                {
                    _mapper.Apply(dTO);
                }
                await UniTask.Yield(PlayerLoopTiming.Update, token);
            }
        }
        catch(OperationCanceledException)
        {
            //정상 중지
        }
    }
}
