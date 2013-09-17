/******This is a Singleton created to emulate several common inputs from leap for unity3D******/
/**************************https://github.com/Dachang/LeapPlugins******************************/
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Leap;

public static class LDCLeapControl
{
	public static float ROTATE_MOD = 3.14F;
	public static float TOUCH_DISTANCE;
	public static float TOUCH_POSITION_X;
	public static float TOUCH_POSITION_Y;
	private static int WINDOW_HEIGHT = UnityEngine.Screen.height;
	private static int WINDOW_WIDTH = UnityEngine.Screen.width;	
	
	//member variables;
	static Leap.Controller _controller;
	static Leap.Frame _frame;
	static Leap.Hand _hand;
	static Leap.Gesture _gesture;
	static int _fingerNum = 0;
	//construct leap controller
	static LDCLeapControl()
	{
		_controller = new Leap.Controller();
	}
	
	//getters
	public static Leap.Frame Frame
	{
		get { return _frame; }
	}
	public static Leap.Hand Hand
	{
		get { return _hand; }
	}
	//get latest frame called each second
	public static void Update () 
	{
		if(_controller != null)
		{
			Frame lastFrame = _frame == null ? Frame.Invalid : _frame;
			_frame = _controller.Frame();

			if(_frame != null)
			{
				if(_frame.Hands.Count > 0)
				{
					_hand = _frame.Hands[0];
				}
				if(_frame.Fingers.Count > 0)
				{
					_fingerNum = _frame.Fingers.Count;
				}
			}
		}
	}
	//return hand infos retrieved from leap
	public static float getHandInput(string gesture)
	{
		float leapData = getLeapHand(gesture);
		return leapData;
	}
	//return gesture infos retrieved from leap
	public static float getGestureInput()
	{
		float leapGestureData = getLeapGesture();
		return leapGestureData;
	}
	public static void getTouchPoint()
	{
		touchEmulation();
	}
	//private methods
	private static float getLeapHand(string movement)
	{
		Update();
		float leapData = 0.0F;
		
		if(_hand != null)
		{
			Vector3 PalmPosition = new Vector3(0,0,0);
			Vector3 PalmNormal = new Vector3(0,0,0);
			Vector3 PalmDirection = new Vector3(0,0,0);
			PalmPosition = _hand.PalmPosition.ToUnityTranslated();
			PalmNormal = _hand.PalmNormal.ToUnity();
			PalmDirection = _hand.PalmPosition.ToUnity();
			
			switch(movement)
			{
				//movement on x axis (left and right)
			case "Horizontal":
				leapData = PalmPosition.x;
				break;
				//movement on y axis (up and down)
			case "Vertical":
				leapData = PalmPosition.y;
				break;
				//movement on z axis (depth)
			case "Depth":
				leapData = PalmPosition.z;
				break;
				//rotate the wrist (horizontal)
			case "Rotation":
				leapData = -ROTATE_MOD * PalmNormal.x;
				break;
				//tilt the wrist (vertical)
			case "Tilt":
				leapData = PalmNormal.z;
				break;
				//horizontal direction of palm
			case "HorizontalDirection":
				leapData = PalmDirection.x;
				break;
				//vertical direction of palm
			case "VerticalDirection":
				leapData = PalmDirection.y;
				break;
			default:
				break;
			}
		}
		else
		{
			//add code here to switch back to keyboard & mouse input
		}
		return leapData;
	}
	
	private static float getLeapGesture()
	{
		Update();
		float gestureData = 0.0F;
		//enabled gestures
		_controller.EnableGesture(Gesture.GestureType.TYPECIRCLE);
		_controller.EnableGesture(Gesture.GestureType.TYPESWIPE);
		_controller.EnableGesture(Gesture.GestureType.TYPEKEYTAP);
		_controller.EnableGesture(Gesture.GestureType.TYPESCREENTAP);
		//get gestures from frame
		GestureList gestureList = _frame.Gestures();
		for( int i=0; i<gestureList.Count; i++)
		{
			Gesture gesture = gestureList[i];
			switch(gesture.Type)
			{
				//CIRCLE
			case Gesture.GestureType.TYPECIRCLE:
				CircleGesture circle = new CircleGesture (gesture);
				gestureData = circle.Progress;
				string clockwiseness;
				//whether it is clockwise or not
				if (circle.Pointable.Direction.AngleTo (circle.Normal) <= Mathf.PI / 4) 
				{ 
					clockwiseness = "clockwise";
					Debug.Log("Clockwise Circle Gesture");
				}
				else 
				{ 
					clockwiseness = "counter clockwise"; 
					Debug.Log("Counter Clockwise Circle Gesture");
				}
				break;
				//SWIPE
			case Gesture.GestureType.TYPESWIPE:
				SwipeGesture swipe = new SwipeGesture (gesture);
				Vector3 swipeDirection = new Vector3(0,0,0);
				swipeDirection = swipe.Direction.ToUnity();
				gestureData = swipeDirection.x;
				//swipe direction left or right
				if (gestureData < 0) { Debug.Log ("Left Swipe Gesture"); }
				else if (gestureData > 0) { Debug.Log ("Right Swipe Gesture"); }
				break;
				//KEY TAP
			case Gesture.GestureType.TYPEKEYTAP:
				KeyTapGesture keyTap = new KeyTapGesture (gesture);
				Vector3 keyTapPosition = new Vector3(0,0,0);
				keyTapPosition = keyTap.Position.ToUnity();
				gestureData = keyTapPosition.x;
				Debug.Log("Key Type Gesture");
				break;
				//SCREEN TAP
			case Gesture.GestureType.TYPESCREENTAP:
				ScreenTapGesture screenTap = new ScreenTapGesture (gesture);
				Vector3 screenTapPostion = new Vector3(0,0,0);
				screenTapPostion = screenTap.Position.ToUnityTranslated();
				gestureData = screenTapPostion.x;
				Debug.Log("Screen Tap Gesture");
				break;
			default:
				break;
			}
		}
		return gestureData;
	}
	
	private static void touchEmulation()
	{
		Update();
		//retrieve the zone of the foward-most finger
		Pointable pointable = _frame.Pointables.Frontmost;
		Pointable.Zone zone = pointable.TouchZone;
		//The touch distance ranges from +1 to -1 as the finger moves to and through the touch surface
		TOUCH_DISTANCE = pointable.TouchDistance;
		//The stabilized Position (in the Leap Motion coordinate system) has a context-sensitive amount of filtering and stabilization
		Vector stabilizedPosition = pointable.StabilizedTipPosition;
		//Get the 2D pixel coordinates of a touch point within the window
		InteractionBox iBox = _frame.InteractionBox;
		Vector normalizedPosition = iBox.NormalizePoint(stabilizedPosition);
		TOUCH_POSITION_X = normalizedPosition.x * WINDOW_WIDTH;
		TOUCH_POSITION_Y = WINDOW_HEIGHT - normalizedPosition.y * WINDOW_HEIGHT;
	}
}