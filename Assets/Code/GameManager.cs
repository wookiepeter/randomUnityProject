using UnityEngine;

class GameManager 
{
    private static GameManager _instance;
    public static GameManager Instance { get { return _instance ?? (_instance = new GameManager()); } }
    public int points { get; private set; }
    private GameManager() //Nobody can instance this fuckingshit
    {

    }
    public void Reset()
    {
        points = 0;
    }

    public void ResetPoints(int pointsToReset) {

        points = pointsToReset;
    }
    public void AddPoints(int pointsToAdd)
    {
        points += pointsToAdd;
    }
}

