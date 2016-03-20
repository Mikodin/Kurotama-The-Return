using UnityEngine;
using System.Collections;

public class MobileInput : MonoBehaviour {
	int screenWidth = Screen.width;
	int screenHeight = Screen.height;

	float dividerPos;
	Pane leftPane;
	Pane rightPane;

	Touch touches;

	int leftAction = 0;
	int rightAction = 0;

	void Start() {
		dividerPos = screenWidth / 3;
		leftPane = new Pane (0, dividerPos);
		rightPane = new Pane (dividerPos, screenWidth);

	}

	void Update() {
		int fingerCount = 0;
		foreach (Touch touch in Input.touches) {
			if (touch.phase != TouchPhase.Ended && touch.phase != TouchPhase.Canceled) {
				fingerCount++;
				print ("Touch X: " + touch.position.x + " Touch Y: " + touch.position.y + " In left pane? " + leftPane.InBounds (touch) + " In right pane? " + rightPane.InBounds (touch));
				if (leftPane.InBounds (touch))
					SetLeftAction (1);	
				else if (rightPane.InBounds (touch))
					SetRightAction (1);
				
			} else {
				SetRightAction (0);
				SetLeftAction (0);
				print ("Touch Direction: " + TouchPhase.Moved.ToString());
			}
		}
	}



	void SetLeftAction(int _action) {
		leftAction = _action;
	}

	public int GetLeftAction() {
		return leftAction;
	}

	void SetRightAction(int _action) {
		rightAction = _action;
	}

	public int GetRightAction() {
		return rightAction;
	}
			

	struct Pane {
		public float startPos;
		public float endPos;

		public Pane(float _startPos, float _endPos) {
			startPos = _startPos;
			endPos = _endPos;
		}

		public bool InBounds(Touch _touch) {
			if ((_touch.position.x > startPos && _touch.position.x < endPos) && (_touch.position.y > 0 && _touch.position.y < Screen.height))
				return true;
			else
				return false;
		}

	}

	struct Gesture {
		public Vector2 start;
		public Vector2 end;
		public Vector2 direction;

		public Gesture(Vector2 _start) {
			start = _start;
			end = new Vector2(0,0);
			direction = new Vector2(0,0);
		}

		public void SetStart(Vector2 _start) {
			start = _start;
		}

		public Vector2 GetStart() {
			return start;
		}

		public void SetEnd(Vector2 _end) {
			end = _end;
		}

		public Vector2 GetEnd() {
			return end;
		}

		public Vector2 GetDirection() {
			return direction;
		}
	}

}


