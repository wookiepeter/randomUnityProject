using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class ButtonController : MonoBehaviour {

    public Sprite happy;
    public Sprite unhappy;

    Button playButton;
    int counter;

	// Use this for initialization
	void Start () {
        counter = 0;
        GameObject gaOb = GameObject.Find("PlayButton");
        playButton = gaOb.GetComponent<Button>();

	}
	
	// Update is called once per frame
	void Update () {

       


	}

    public void ClickTest()
    {
        Debug.Log("CLCKED");
    }
    public void ClickTest2(string text)
    {
        Debug.Log(text);
    }

    public void ChangeToUnhappy()
    {
   
            playButton.image.sprite = unhappy;
      
       
        
    }
    public void ChangeToHappy(string text)
    {
        
        counter++;
        if (counter == 3)
        {
            counter = 0;
            playButton.image.sprite = happy;
            Invoke("ChangeToUnhappy", 3f);
        }
        else
        {
            Debug.Log(text + counter);
        }
        
    }

    void OnMouseUpAsButton()
    {
        Debug.Log("TRUE STORY");
    }
    void OnMouseDown()
    {
        Debug.Log("TRUE LOVESTORY");

    }
}
