using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;

class LevelManager : MonoBehaviour
{//Singleton
    
    public static LevelManager Instance{ get; private set;}
    public Player player { get; private set; }
    public CameraController cameraController { get; private set; }
    public TimeSpan runningTime { get { return DateTime.UtcNow - _started; } }

    public int CurrentTimeBonus
    {
        get
        {
            var secondDifference = (int)(bonusCuoffSeconds - runningTime.TotalMilliseconds);
            Console.WriteLine("Mathf.Max(0, secondDifference) * bonusSecondMultiplier;");
            return Mathf.Max(0, secondDifference) * bonusSecondMultiplier;
            
        }
    }

    List<Checkpoint> _checkpoints;
    int _currentCheckpointIndex;
    DateTime _started;
    int _savedPoints;

    public Checkpoint DebugSpawn;
    public int bonusCuoffSeconds;           //max time player can go to the next point - bnonus trigger
    public int bonusSecondMultiplier;       //how many points over

    public void Awake()
    {
        Instance = this;
    }
    public void Start()
    { 
        _checkpoints = FindObjectsOfType<Checkpoint>().OrderBy(t => t.transform.position.x).ToList(); // findet alle Checkpoints in der Scene
        _currentCheckpointIndex = _checkpoints.Count > 0 ? 0 : -1; //Wenn keine Checkpoints drin sind auf -1 setzten

        player = FindObjectOfType<Player>();
        cameraController = FindObjectOfType<CameraController>();

        _started = DateTime.UtcNow; //Auf now setzten


#if UNITY_EDITOR //Wird nur aufgerufen wenn wir unser Lvl bearbeiten -> Späte kommt das neme rein
        if (DebugSpawn != null)
            DebugSpawn.SpawnPlayer(player);
        else if (_currentCheckpointIndex != -1)
            _checkpoints[_currentCheckpointIndex].SpawnPlayer(player);


#else
        if (_currentCheckpointIndex != -1)
            _checkpoints[_currentCheckpointIndex].SpawnPlayer(player);
#endif
    }
    public void Update()
    {
        var isAtLastCheckpoint = _currentCheckpointIndex + 1 >= _checkpoints.Count;
        if (isAtLastCheckpoint) //Letzter Checkpoint abgearbeitet
            return;

        var distanceToNextCheckpoint = _checkpoints[_currentCheckpointIndex + 1].transform.position.x - player.transform.position.x;
        if (distanceToNextCheckpoint >= 0)
            return;

        _checkpoints[_currentCheckpointIndex].PlayerLeftCheckpoint();
        _currentCheckpointIndex++;
        _checkpoints[_currentCheckpointIndex].PlayerHitCheckpoint();
        //Todo: TimeBonus
        GameManager.Instance.AddPoints(CurrentTimeBonus);
        _savedPoints = GameManager.Instance.points;
        _started = DateTime.UtcNow;

    }
    public void KillPlayer()
    {
        StartCoroutine(KillPlayerCo());
    }
    private IEnumerator KillPlayerCo()
    {
        player.Kill();
        cameraController.isFollowing = false;
        yield return new WaitForSeconds(2f);

        cameraController.isFollowing = true;

        if (_currentCheckpointIndex != -1)
            _checkpoints[_currentCheckpointIndex].SpawnPlayer(player);

        _started = DateTime.UtcNow;
        GameManager.Instance.ResetPoints(_savedPoints);
    }






}

