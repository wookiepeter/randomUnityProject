using UnityEngine;
using System.Collections;
using System;

class PointStar : MonoBehaviour, IPlayerRespawnListener
{
    public GameObject effect; //wenn collected
    public int pointsToAdd = 10;



    public void OnTriggerEnter2D(Collider2D other)
    {
        if (other.GetComponent<Player>() == null)
            return;

        GameManager.Instance.AddPoints(pointsToAdd);
        Instantiate(effect, transform.position, transform.rotation); //Create Particel
        gameObject.SetActive(false); //Nur false, damit man wenn der Spieler stirbt ->respawn
    }

    //Wenn der Player wieder belebt wurde
    public void OnPlayerRespawnInThisCheckpoint(Checkpoint checkpoint, Player player)
    {
        gameObject.SetActive(true);
    }
}

