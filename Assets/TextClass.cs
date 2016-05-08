using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class TextClass : MonoBehaviour
{
	public int doOnce = 0;
	public bool isHidden = true;
	public float timepassed = 0;
	//public string text; //gotta set this as the text given
	public float xcoord;
	public GameObject gObj;	
	public GameObject myObj;
	//public Renderer myRenderer;
	public GUIText myGUI;
	public float pos = -99; //perhaps an ill-used dummy value
	
	// Use this for initialization
	void Start ()
	{
	//	isHidden = true;
	//xcoord = 24234;
	//text = gameObject.GetComponent<GUIText>(TextField); //gotta set this as the text given
	myObj = gameObject;
	//myRenderer = myObj.GetComponent<Renderer>();
	myGUI = this.GetComponent<GUIText>();
	myGUI.enabled = false;
	xcoord = myGUI.transform.position.x; //gotta set this as coord given
    //myRenderer.enabled = false;
	gObj = GameObject.Find ("Player");	


	}
	
	// Update is called once per frame
	void Update ()
	{
		gObj = GameObject.Find ("Player");	
		if (gObj) {
			pos = gObj.transform.position.x; 
		}
	
		if (doOnce == 0) {
			if (pos > xcoord - 0.02 && pos < xcoord + 0.02) { 
			isHidden = false; 
			doOnce = 1; 
			myGUI.enabled = true;
            //myRenderer.enabled = true; 
			}
		}

		else if (doOnce == 1 && timepassed < 5) {
			//doOnce = 2
			timepassed = timepassed + Time.deltaTime; 

		}

		else if (doOnce == 1 && timepassed >= 5) {
					doOnce = 2; 
		}

		else if (doOnce == 2) {
					myGUI.enabled = false;
			        //myRenderer.enabled = false; 
					//gameObject.guiText.Destroy;
					doOnce = 3; 
		}
	}
}

