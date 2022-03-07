using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
public class Cell : MonoBehaviour
{
	public RectTransform RectTransform;
	public DragClamp Clamp;
	private Cube _cubeStay = null;
	private Cube _currentCube = null;
	public Cube Cube
	{
		get { return _currentCube; }
		private set { _currentCube = value; }
	}
	private void Start()
	{
		SetClampedCell();
	}

	private void SetClampedCell()
	{
		var delta = 100;
		var left = RectTransform.localPosition.x - delta;
		var right = RectTransform.localPosition.x + delta;
		var up = RectTransform.localPosition.y - delta;
		var down = RectTransform.localPosition.y + delta;
		Clamp = new DragClamp(right, left, up, down);
	}

	[System.Serializable]
	public class DragClamp
	{
		public float Left;
		public float Right;
		public float Up;
		public float Down;

		public DragClamp(float right, float left, float up, float down)
		{
			Left = left;
			Right = right;
			Up = up;
			Down = down;
		}
	}
}
