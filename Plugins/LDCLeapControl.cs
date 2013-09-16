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
	static int _fingerNum = 0;
	
	static LDCLeapControl()
	{
		_controller = new Leap.Controller();
	}
	
	//getters
	public static Leap.Frame Frame
	{
		get
		{
			return _frame;
		}
	}
	public static Leap.Hand Hand
	{
		get
		{
			return _hand;
		}
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
	
	//private interfaces
	private static float getLeapData(string gesture)
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
			
			switch(gesture)
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
			//switch to keyboard & mouse input
		}
		return leapData;
	}
}
