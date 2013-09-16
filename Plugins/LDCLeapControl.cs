using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Leap;

public static class LDCLeapControl
{
	public static float ROTATE_MOD = 3.14F;
	
	//member variables;
	static Leap.Controller _controller;
	static Leap.Frame _frame;
	static Leap.Hand _hand;
	static Leap.Gesture _gesture;
	static int _fingerNum = 0;
	
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
	
	public static float getHandInput(string gesture)
	{
		float leapData = getLeapData(gesture);
		return leapData;
	}
	
	public static float getGestureInput()
	{
		float leapGestureData = getLeapGesture();
		return leapGestureData;
	}
	
	//private interfaces
	private static float getLeapData(string movement)
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
			case "Horizontal":
				leapData = PalmPosition.x;
				break;
			case "Vertical":
				leapData = PalmPosition.y;
				break;
			case "Depth":
				leapData = PalmPosition.z;
				break;
			case "Rotation":
				leapData = -ROTATE_MOD * PalmNormal.x;
				break;
			case "Tilt":
				leapData = PalmNormal.z;
				break;
			case "HorizontalDirection":
				leapData = PalmDirection.x;
				break;
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
		//enabled gesture
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
			case Gesture.GestureType.TYPECIRCLE:
				CircleGesture circle = new CircleGesture (gesture);
				Debug.Log("Circle Gesture");
				break;
			case Gesture.GestureType.TYPESWIPE:
				SwipeGesture swipe = new SwipeGesture (gesture);
				Debug.Log("Swipe Gesture");
				break;
			case Gesture.GestureType.TYPEKEYTAP:
				KeyTapGesture keyTap = new KeyTapGesture (gesture);
				Debug.Log("Key Type Gesture");
				break;
			case Gesture.GestureType.TYPESCREENTAP:
				ScreenTapGesture screenTap = new ScreenTapGesture (gesture);
				Debug.Log("Screen Tap Gesture");
				break;
			default:
				break;
			}
		}
		return gestureData;
	}
}
