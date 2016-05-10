using UnityEngine;
using System.Collections;

public class MobileInput : MonoBehaviour
{
	int screenWidth = Screen.width;
	int screenHeight = Screen.height;
	
	public float dividerPos;
	public Pane leftPane;
	public Pane rightPane;
	public float noActionRadius = 5;

	Touch touches;

	Gesture leftGesture;
	Gesture rightGesture;

	void Start ()
	{
		dividerPos = screenWidth / 3;
		noActionRadius = dividerPos / 5;

		leftPane = new Pane (0, dividerPos);
		rightPane = new Pane (dividerPos, screenWidth);
		leftGesture = new Gesture (1);
		rightGesture = new Gesture (1);
	}

	void Update ()
	{
		foreach (Touch touch in Input.touches) {
			int x = 1;

			switch (touch.phase) {

			case TouchPhase.Began:
				if (leftPane.InBounds (touch)) {
					leftGesture.SetStart (touch.position);
				}

				if (rightPane.InBounds (touch)) {
					rightGesture.SetStart (touch.position);
				}
				//StartCoroutine ("DelayTapHold",.20f);
				break;

			case TouchPhase.Moved:
				if (leftPane.InBounds (touch)) {
					leftGesture.SetEnd (touch.position);
				}

				if (rightPane.InBounds (touch)) {
					rightGesture.SetEnd (touch.position);
				}
				break;

			case TouchPhase.Ended:
				if (leftPane.InBounds (touch)) {
					leftGesture.CalculateDirection ();
					leftGesture.SetGesture (leftGesture.CalculateLeftPaneGesture ());
					print ("Left Gesture: " + leftGesture.GetGesture ());
					if (leftGesture.GetGesture () == "up" || leftGesture.GetGesture () == "upright" || leftGesture.GetGesture () == "upleft") {
						StartCoroutine ("DelayResetLeftGesture", .3f);
					} else {
						leftGesture.Reset ();
					}
				}

				if (rightPane.InBounds (touch)) {
					rightGesture.CalculateDirection ();
					rightGesture.SetGesture (rightGesture.CalculateRightPaneGesture ());
					print ("Right Gesture: " + rightGesture.GetGesture ());
					if (rightGesture.GetGesture () == "left" || rightGesture.GetGesture () == "right" ||
					    rightGesture.GetGesture () == "upright" || rightGesture.GetGesture () == "upleft" ||
					    rightGesture.GetGesture () == "downright" || rightGesture.GetGesture () == "downleft") {
							
						StartCoroutine ("DelayResetRightGesture", .2f);
					} else {
						rightGesture.Reset ();
					}
				}
				break;
			}
		}
	}

	IEnumerator DelayTapHold (float time)
	{
		yield return new WaitForSeconds (time);
		leftGesture.SetGesture ("taphold");
	}

	IEnumerator DelayResetRightGesture (float time)
	{
		yield return new WaitForSeconds (time);
		rightGesture.Reset ();
	}

	IEnumerator DelayResetLeftGesture (float time)
	{
		yield return new WaitForSeconds (time);
		leftGesture.Reset ();
	}

	public string GetLeftGestures ()
	{
		return leftGesture.GetGesture ();
	}

	public string GetRightGestures ()
	{
		return rightGesture.GetGesture ();
	}

	public struct Pane
	{
		public float startPos;
		public float endPos;

		public Pane (float _startPos, float _endPos)
		{
			startPos = _startPos;
			endPos = _endPos;
		}

		public bool InBounds (Touch _touch)
		{
			if ((_touch.position.x > startPos && _touch.position.x < endPos) && (_touch.position.y > 0 && _touch.position.y < Screen.height))
				return true;
			else
				return false;
		}

	}

	public struct Gesture
	{
		Vector2 start;
		Vector2 end;
		Vector2 direction;
		string gesture;
		float noActionRadius;
		Vector2 noActionRect;

		public Gesture (int i)
		{
			start = new Vector2 (0, 0);
			end = new Vector2 (0, 0);
			direction = new Vector2 (0, 0);
			gesture = "";
			noActionRadius = (Screen.width/3)/30;
			noActionRect = new Vector2 ((Screen.width / 3) / 5, Screen.height / 10);
			noActionRadius = 30;
			print (noActionRadius);
		}

		public void SetStart (Vector2 _start)
		{
			start = _start;
		}

		public Vector2 GetStart ()
		{
			return start;
		}

		public void SetEnd (Vector2 _end)
		{
			end = _end;
		}

		public Vector2 GetEnd ()
		{
			return end;
		}

		public Vector2 GetDirection ()
		{
			return direction;
		}

		public void CalculateDirection ()
		{
			direction = end - start;
		}


		public string CalculateRightPaneGesture ()
		{

			if (direction.x > noActionRadius && direction.y > noActionRadius)
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
			else
				return "release";
		}


		public string CalculateLeftPaneGesture ()
		{
			
			if (direction.x > noActionRect.x && direction.y > -noActionRect.y && direction.y < noActionRect.y)
				return "right";
			if (direction.x < -noActionRect.x && direction.y > -noActionRect.y && direction.y < noActionRect.y)
				return "left";
			if (direction.y > noActionRect.y && direction.x > -noActionRect.x && direction.x < noActionRect.x)
				return "up";
			else if (direction.y > noActionRect.y && direction.x < -noActionRect.x)
				return "upleft";
			else if (direction.y > noActionRect.y && direction.x > noActionRect.x)
				return "upright";
			else
				return "release";
		}

		public void SetGesture (string _gesture)
		{
			gesture = _gesture;
		}

		public string GetGesture ()
		{
			return gesture;
		}

		public void Reset ()
		{
			start = new Vector2 (0, 0);
			end = new Vector2 (0, 0);
			direction = new Vector2 (0, 0);
			SetGesture ("");
		}
	}
}


