using UnityEngine;
using System.Collections;

public class MobileInput : MonoBehaviour
{
	int screenWidth = Screen.width;
	int screenHeight = Screen.height;

	public float dividerPos;
	Pane leftPane;
	Pane rightPane;
	public float noActionRadius;

	Touch touches;

	int leftAction = 0;
	int rightAction = 0;
	Gesture g1;

	void Start ()
	{
		dividerPos = screenWidth / 3;
		noActionRadius = dividerPos / 5;
		leftPane = new Pane (0, dividerPos);
		rightPane = new Pane (dividerPos, screenWidth);
		g1 = new Gesture (1);

	}

	void Update ()
	{
		foreach (Touch touch in Input.touches) {
			int x = 1;

			switch (touch.phase) {

			case TouchPhase.Began:
				g1.SetStart(touch.position);
				break;

			case TouchPhase.Moved:

				break;

			case TouchPhase.Ended:
				g1.SetEnd (touch.position);
				g1.CalculateDirection ();
				print ("Gesture Direction: " + g1.GetDirection ());
				g1.CalculateGesture ();

				print (g1.GetGesture ());
				break;
			}
		}
	}

	void SetLeftAction (int _action)
	{
		leftAction = _action;
	}

	public int GetLeftAction ()
	{
		return leftAction;
	}

	void SetRightAction (int _action)
	{
		rightAction = _action;
	}

	public int GetRightAction ()
	{
		return rightAction;
	}

	public float GetNoActionRadius() {
		return noActionRadius;
	}

	struct Pane
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

	struct Gesture
	{
		Vector2 start;
		Vector2 end;
		Vector2 direction; 
		string gesture;
		int noActionRadius;

		public Gesture (int i)
		{
			start = new Vector2(0,0);
			end = new Vector2 (0, 0);
			direction = new Vector2 (0, 0);
			gesture = "";
			noActionRadius = 30;
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

		public void CalculateGesture() {
			if (Mathf.Abs (direction.x) < noActionRadius && Mathf.Abs (direction.y) < noActionRadius)
				SetGesture ("running");
			
			else if (direction.x > noActionRadius && direction.y > noActionRadius)
				SetGesture ("upRightJump");
			else if (direction.x < -noActionRadius && direction.y > noActionRadius)
				SetGesture ("upLeftJump");
			else if (direction.x < -noActionRadius && direction.y < -noActionRadius)
				SetGesture ("btmLeftJump");
			else if (direction.x > noActionRadius && direction.y < -noActionRadius)
				SetGesture ("btmRightJump");
			else if ((direction.x > -noActionRadius || direction.x < noActionRadius) && direction.y > noActionRadius)
				SetGesture ("VertJump");
			else if ((direction.x > -noActionRadius || direction.x < noActionRadius) && direction.y < -noActionRadius)
				SetGesture ("Crouch");
			else if ((direction.y > -noActionRadius || direction.y < noActionRadius) && direction.x > noActionRadius)
				SetGesture ("RightSwipe");
			else if ((direction.y > -noActionRadius || direction.y < noActionRadius) && direction.x < -noActionRadius)
				SetGesture ("LeftSwipe");
		}

		public void SetGesture(string _gesture) {
			gesture = _gesture;
		}

		public string GetGesture() {
			return gesture;
		}
	}

}


