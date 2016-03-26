using UnityEngine;
using System.Collections;

public class MobileInput : MonoBehaviour {
	int screenWidth = Screen.width;
	int screenHeight = Screen.height;
	
	public float dividerPos;
	Pane leftPane;
	Pane rightPane;
	public float noActionRadius = 5;

	Touch touches;

	int leftAction = 0;
	int rightAction = 0;
	Gesture g1;

	void Start () {
		dividerPos = screenWidth / 3;
		noActionRadius = dividerPos / 5;

		leftPane = new Pane (0, dividerPos);
		rightPane = new Pane (dividerPos, screenWidth);
		g1 = new Gesture (1);
	}

	void Update () {
		foreach (Touch touch in Input.touches) {
			int x = 1;

			switch (touch.phase) {

			case TouchPhase.Began:
				g1.SetStart (touch.position);
				//StartCoroutine ("DelayTapHold",.20f);
				break;

			case TouchPhase.Moved:
				g1.SetEnd (touch.position);
				g1.CalculateDirection ();
				//if (g1.CalculateGesture () == "left" || g1.CalculateGesture() == "right") {
					g1.SetGesture (g1.CalculateGesture ());
				//} 
				break;

			case TouchPhase.Ended:
				g1.SetGesture (g1.CalculateGesture ());
				if (g1.GetGesture () == "up" || g1.GetGesture () == "upRight" || g1.GetGesture () == "upLeft") {
					StartCoroutine ("DelayReset", .3f);
				} else
					g1.Reset ();
				//g1.Reset ();

				//g1.SetGesture("release");
				//print (g1.GetGesture ());
				break;
			}
		}
	}

	IEnumerator DelayTapHold(float time) {
		yield return new WaitForSeconds(time);
		g1.SetGesture ("taphold");
	}

	IEnumerator DelayReset(float time) {
		yield return new WaitForSeconds(time);
		g1.Reset ();

	}

	void SetLeftAction (int _action) {
		leftAction = _action;
	}

	public int GetLeftAction () {
		return leftAction;
	}

	void SetRightAction (int _action) {
		rightAction = _action;
	}

	public int GetRightAction () {
		return rightAction;
	}

	public float GetNoActionRadius() {
		return noActionRadius;
	}

	public string GetGestures() {
		return g1.GetGesture();
	}

	struct Pane {
		public float startPos;
		public float endPos;

		public Pane (float _startPos, float _endPos) {
			startPos = _startPos;
			endPos = _endPos;
		}

		public bool InBounds (Touch _touch) {
			if ((_touch.position.x > startPos && _touch.position.x < endPos) && (_touch.position.y > 0 && _touch.position.y < Screen.height))
				return true;
			else
				return false;
		}

	}

	struct Gesture {
		Vector2 start;
		Vector2 end;
		Vector2 direction; 
		string gesture;
		float noActionRadius;

		public Gesture (int i) {
			start = new Vector2(0,0);
			end = new Vector2 (0, 0);
			direction = new Vector2 (0, 0);
			gesture = "";
			//noActionRadius = (Screen.width/3)/30;
			noActionRadius = 10;
			print (noActionRadius);
		}

		public void SetStart (Vector2 _start) {
			start = _start;
		}

		public Vector2 GetStart () {
			return start;
		}

		public void SetEnd (Vector2 _end) {
			end = _end;
		}

		public Vector2 GetEnd () {
			return end;
		}

		public Vector2 GetDirection () {
			return direction;
		}

		public void CalculateDirection (){
			direction = end - start;
		} 

		public string CalculateGesture() {
			if (Mathf.Abs (direction.x) < noActionRadius && Mathf.Abs (direction.y) < noActionRadius)
				return "taphold";
			else if (direction.x > noActionRadius && direction.y > noActionRadius)
				return "upright";
			else if (direction.x < -noActionRadius && direction.y > noActionRadius)
				return "upleft";
			else if (direction.x < -noActionRadius && direction.y < -noActionRadius)
				return "downleft";
			else if (direction.x > noActionRadius && direction.y < -noActionRadius)
				return "downright";
			else if ((direction.x > -noActionRadius || direction.x < noActionRadius) && direction.y > noActionRadius)
				return "up";
			else if ((direction.x > -noActionRadius || direction.x < noActionRadius) && direction.y < -noActionRadius)
				return "down";
			else if ((direction.y > -noActionRadius || direction.y < noActionRadius) && direction.x > noActionRadius)
				return "right";
			else if ((direction.y > -noActionRadius || direction.y < noActionRadius) && direction.x < -noActionRadius)
				return "left";

			else return "release";

			//Reset ();
		}
	
		public void SetGesture(string _gesture) {
			gesture = _gesture;
		}

		public string GetGesture() {
			return gesture;
		}




		public void Reset() {
			start = new Vector2 (0, 0);
			end = new Vector2 (0, 0);
			direction = new Vector2 (0, 0);
			SetGesture ("");
		}
	}

}


