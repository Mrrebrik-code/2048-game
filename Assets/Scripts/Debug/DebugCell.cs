using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugCell : MonoBehaviour
{
	private Vector3[] _positionCells;
	private bool _isInit = false;
	public void Init(Vector3[] positions)
	{
		_positionCells = positions;
		_isInit = true;
		StartCoroutine(Delay());
	}
	private bool color = false;
	public void SetColorRed(bool acrive)
	{
		color = acrive;
	}
	private bool colordrag = false;
	public bool IsTest = false;
	public void SetDragColor()
	{
		colordrag = true;
	}

	public void Test()
	{
		IsTest = true;
	}

	private IEnumerator Delay()
	{
		while (true)
		{
			yield return new WaitForSecondsRealtime(2f);
			colordrag = false;
			IsTest = false;
		}
	}
	private void OnDrawGizmos()
	{
		if (IsTest)
		{
			if (color)
			{
				Gizmos.color = Color.red;
			}
			else
			{
				Gizmos.color = Color.green;
			}

			Gizmos.DrawSphere(transform.position, 0.1f);
			if (_isInit)
			{
				Gizmos.color = Color.white;
				Gizmos.DrawLine(transform.position, _positionCells[0]);

				Gizmos.DrawLine(transform.position, _positionCells[1]);
				Gizmos.DrawLine(transform.position, _positionCells[2]);
				Gizmos.DrawLine(transform.position, _positionCells[3]);
			}

		}
		else
		{
			if (colordrag == false)
			{
				if (color)
				{
					Gizmos.color = Color.red;
				}
				else
				{
					Gizmos.color = Color.green;
				}

				Gizmos.DrawSphere(transform.position, 0.1f);
				if (_isInit)
				{
					Gizmos.color = Color.blue;
					Gizmos.DrawLine(transform.position, _positionCells[0]);

					Gizmos.DrawLine(transform.position, _positionCells[1]);
					Gizmos.DrawLine(transform.position, _positionCells[2]);
					Gizmos.DrawLine(transform.position, _positionCells[3]);
				}
			}
			else
			{
				Gizmos.color = Color.black;
				Gizmos.DrawSphere(transform.position, 0.1f);

				if (_isInit)
				{
					Gizmos.color = Color.white;
					Gizmos.DrawLine(transform.position, _positionCells[0]);

					Gizmos.DrawLine(transform.position, _positionCells[1]);
					Gizmos.DrawLine(transform.position, _positionCells[2]);
					Gizmos.DrawLine(transform.position, _positionCells[3]);
				}
			}
		}
		
		
	}
}
