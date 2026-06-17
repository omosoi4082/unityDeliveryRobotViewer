using Cysharp.Threading.Tasks;
using System.Threading;
using UnityEngine;

public interface IRobotDataSource
{
    UniTask StartAsync(CancellationToken ct);
    UniTask StopAsync();
}
