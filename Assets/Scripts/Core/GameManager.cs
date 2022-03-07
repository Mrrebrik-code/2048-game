using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class GameManager : MonoBehaviour
{
	public static GameManager Instance;
	[SerializeField] private int _countStartCubes;
	[SerializeField] private Cube _cubePrefab;
	[SerializeField] private List<Cell> _cells = new List<Cell>();
	[SerializeField] private List<Cell> _cellsSpawn = new List<Cell>();
	[SerializeField] private List<Cube> _cubes = new List<Cube>();
	[SerializeField] private Transform _content;

	private void Awake()
	{
		Instance = this;
		CreateMap();
		CreateCubes(_countStartCubes);
	}

	[SerializeField] private int countCellHorizontal;
	[SerializeField] private int cointCellVertical;
	public CellCore[,] Map;
	private Dictionary<int, CellCore> _mapCells = new Dictionary<int, CellCore>();
	private void CreateMap()
	{
		_cells.Reverse();
		Map = new CellCore[cointCellVertical, countCellHorizontal];
		int index = 0;
		for (int vertical = 0; vertical < cointCellVertical; vertical++)
		{
			for (int horizontal = 0; horizontal < countCellHorizontal; horizontal++)
			{
				var cell = Map[vertical, horizontal] = new CellCore(index, _cells[index]);
				_mapCells.Add(index, cell);

				cell.X = horizontal;
				cell.Y = vertical;

				index++;
			}
		}

		for (int vertical = 0; vertical < cointCellVertical; vertical++)
		{
			for (int horizontal = 0; horizontal < countCellHorizontal; horizontal++)
			{
				var cell = Map[vertical, horizontal];
				cell.SetDependencies(
					down: GetCell(cell, cell.Index - 6),
					up: GetCell(cell, cell.Index + 6),
					left: GetCell(cell, cell.Index + 1),
					right: GetCell(cell, cell.Index - 1));
			}
		}

		CellCore GetCell(CellCore cell, int id)
		{
			if (_mapCells.ContainsKey(id))
			{
				var cellTemp = _mapCells[id];

				if (Vector3.Distance(cell.Cell.transform.position, cellTemp.Cell.transform.position) < 2f) return cellTemp;
			}

			return null;
		}
	}

	private CellCore HasFreeCellCore()
	{
		foreach (var cell in Map)
		{
			if (cell.IsFree) return cell;
		}

		return null;
	}
	private void CreateCubes(int count)
	{
		for (int i = 1; i <= count; i++)
		{
			var cube = Instantiate(_cubePrefab, _content);
			cube.name = $"cube_{i}";
			var cell = HasFreeCellCore();
			cell.SetCube(cube);
			cube.Init(cell);
			_cubes.Add(cube);
		}
	}
	public bool IsStop = false;
	public void AddRow()
	{
		IsStop = true;
		StartCoroutine(Delay());
	}
	private IEnumerator Delay()
	{
		
		List<CellCore> _cells = new List<CellCore>();
		foreach (var cell in Map)
		{
			_cells.Add(cell);
		}
		_cells.Reverse();
		foreach (var cell in _cells)
		{
			if(cell.Cube != null)
			{
				cell.Cube.UpMove();
				cell.Cell.GetComponent<DebugCell>().Test();
			}
		}
		var cubes = new List<Cube>();
		var cells = new List<CellCore>();
		for (int i = 1; i <= 6; i++)
		{
			var cube = Instantiate(_cubePrefab, _content);
			cube.RectTransformCube.localPosition = _cellsSpawn[i - 1].RectTransform.localPosition;
			cube.name = $"cube_{i}";
			cubes.Add(cube);
			var cell = HasFreeCellCore();
			cells.Add(cell);

			cell.SetCube(cube, false);
			cube.Init(cell);
			_cubes.Add(cube);
		}
		for (int i = 0; i < cubes.Count; i++)
		{
			cubes[i].RectTransformCube.DOLocalMove(cells[i].Cell.RectTransform.localPosition, 0.2f);
		}

		yield return null;
		IsStop = false;
	}

	public void CreateGroup()
	{
	}

	public void DeleteCube(Cube cube)
	{
		if (_cubes.Contains(cube))
		{
			_cubes.Remove(cube);
			Destroy(cube.gameObject);
		}
	}
}
