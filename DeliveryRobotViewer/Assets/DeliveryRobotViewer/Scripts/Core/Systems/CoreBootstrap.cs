using Cysharp.Threading.Tasks;
using System.Threading;
using UnityEngine;

public class CoreBootstrap : MonoBehaviour
{
    [Header("Prefab")]
    [SerializeField] private RobotListItem listItem;
    [SerializeField] private Transform listItemParent;
    [SerializeField] private RobotMoveItem moveItem;
    [SerializeField] private Transform moveItemParent;

    [Header("SO")]
    [SerializeField] private RobotEventChannelSO eventChannelSO;
    [SerializeField] private MqttTopicConfig topicConfig;

    [SerializeField] private LivenessSystem liveness;

    private float warningValue = 30f;
    private float dangerValue = 15f;

    MqttDataSource mqttsource;
    CancellationTokenSource tokenSource;
    RobotUpDateRunner upDateRunner;
    RobotPresenterFactory factory;
    private void Awake()
    {
        var stateSystem = new RobotStateSystem(warningValue, dangerValue);
        var listPool = new RobotListViewsPool(listItem, listItemParent);
        var movePool=new RobotViewsPool(moveItem, moveItemParent);
        factory = new RobotPresenterFactory(movePool, listPool, eventChannelSO);
        var registy=new RobotRegisty(stateSystem, factory, eventChannelSO);
        var queue = new RobotDataQueue();
        mqttsource = new MqttDataSource(queue, topicConfig);
        tokenSource=new CancellationTokenSource();
        var map = new DataMapper(registy);
        upDateRunner = new RobotUpDateRunner(queue, map);
        liveness.Initialized(registy);
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        mqttsource.StartAsync(tokenSource.Token).Forget();
        upDateRunner.StartRun(tokenSource.Token); 
    }

    private void OnDestroy()
    {
        CleanupAsync().Forget();
    }

    private async UniTask CleanupAsync()
    {
        tokenSource?.Cancel();  
        if(mqttsource!=null)await mqttsource.StopAsync();
        
        tokenSource?.Dispose();
        factory?.Dispose(); 
        

    }

}
