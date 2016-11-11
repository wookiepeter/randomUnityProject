using UnityEngine;
using System.Collections;


class Checkpoint : MonoBehaviour
{
    public void Start()
    {

    }
    public void PlayerHitCheckpoint()
    {

    }
    private IEnumerator PlayerHitCheckpointCo(int bonus)
    {
        yield break;
    }
    public void PlayerLeftCheckpoint()
    {

    }
    public void SpawnPlayer(Player player)
    {
        player.RespawnAt(transform); //Spieler am transform beleben

    }
    public void AssignObjectToCheckpoint()
    {

    }
}

