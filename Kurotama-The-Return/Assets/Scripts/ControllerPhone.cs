using UnityEngine;
using System.Collections;

public class ControllerPhone : MonoBehaviour {
	int screenWidth = Screen.width;
	int screenHeight = Screen.height;

	float movingAreaXMax;
	float moveAreaXMin = 0;

	Touch touches;
	Vector2 screenDimensions;


	void Start() {
		movingAreaXMax = screenWidth / 3;
		screenDimensions = new Vector2 (screenWidth, screenHeight);
	}

	void Update() {
		int fingerCount = 0;
		foreach (Touch touch in Input.touches) {
			if(touch.phase != TouchPhase.Ended && touch.phase != TouchPhase.Canceled) {
				fingerCount++;
				print ("Touch X: " + touch.position.x + " Touch Y: " + touch.position.y);
			}
		}
	}

	public Vector2 GetScreenDimensions() {
		return screenDimensions;
	}
		
}


