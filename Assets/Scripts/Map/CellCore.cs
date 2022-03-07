using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CellCore
{
	public Vector2 Position;
	public int Index { get; private set; }
	public int X;
	public int Y;
	public Cell Cell { get; private set; }
	public bool IsFree { get; private set; }
	public Cube Cube { get; private set; }
	public Dependencies DependenciesCells { get; private set; }
	public CellCore(int index, Cell cell)
	{
		Index = index;
		Cell = cell;
		IsFree = true;
		Position = Cell.RectTransform.localPosition;
	}

	public void SetDependencies(CellCore down, CellCore up, CellCore left, CellCore right)
	{
		DependenciesCells = new Dependencies(down, up, left, right);
		Cell.GetComponent<DebugCell>().Init(new Vector3[]
		{
			down?.Cell.transform.position ?? Cell.transform.position,
			up?.Cell.transform.position ?? Cell.transform.position,
			left?.Cell.transform.position ?? Cell.transform.position,
			right?.Cell.transform.position ?? Cell.transform.position,
		});
	}
	public void SetCube(Cube cube, bool isStartPosition = true)
	{
		cube.SetCellCore(this);
		if(isStartPosition) cube.transform.localPosition = Cell.RectTransform.localPosition;
		Cube = cube;
		IsFree = false;
		Cell?.GetComponent<DebugCell>().SetColorRed(true);
	}

	public void ExitCube(Cube cube)
	{
		Cube = null;
		IsFree = true;
		Cell?.GetComponent<DebugCell>().SetColorRed(false);
	}

	[System.Serializable]
	public class Dependencies
	{
		public CellCore Down;
		public CellCore Up;
		public CellCore Right;
		public CellCore Left;

		public Dependencies(CellCore down, CellCore up, CellCore left, CellCore right)
		{
			Down = down;
			Up = up;
			Left = left;
			Right = right;
		}
	}
}
