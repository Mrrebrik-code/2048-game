using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using DG.Tweening;

public class Cube : MonoBehaviour, IPointerUpHandler, IPointerDownHandler, IDragHandler, IBeginDragHandler, IEndDragHandler
{
	public RectTransform RectTransformCube;
	private Canvas _canvas;

	[Header("Vusualization to cube on board:")]
	[SerializeField] private TMP_Text _textIndex;
	[SerializeField] private Braces _brakes;

	private bool _isPresed = false;
	private bool _isDrag = false;
	private bool isDrop = false;

	private float xMinClamp, xMaxClamp = 0f;
	private float yMinClamp, yMaxClamp = 0f;

	private int _selectedProductIDY;
	private int _selectedProductIDX;

	private CellCore _cellCore;
	private CellCore _cellToDragCurrent;
	private CellCore _cellCoreOld;

	private int _numberCube;
	public int NumberCube
	{
		get { return _numberCube; }
		set
		{
			_numberCube = value;
			_textIndex.text = value.ToString();
			gameObject.name = $"Cube[{value}]";
		}
	}
	public void Init(CellCore core)
	{
		_cellCore = core;
		NumberCube = Random.Range(1, 6);
	}
	public void UpMove()
	{
		//_cellCore.ExitCube(this);
		RectTransformCube.DOLocalMove(_cellCore.DependenciesCells.Up.Cell.RectTransform.localPosition, 0.2f);
		_cellCore.DependenciesCells.Up.SetCube(this, false);
		
	}
	private void Update()
	{

		if(_isPresed == false && GameManager.Instance.IsStop == false)
		{
			if (_cellCore != null && _cellCore.DependenciesCells.Down != null && _cellCore.DependenciesCells.Down.IsFree)
			{
				//_cellCore.ExitCube(this);
				_cellCore.DependenciesCells.Down.SetCube(this);
			}
			else
			{
				if(_cellCore.Cube != this)
				{
					GameManager.Instance.Map[_selectedProductIDY, _selectedProductIDX].SetCube(this);
				}
			}
		}
	}

	public void SetCellCore(CellCore cell, bool isTemp = false)
	{
		if(isTemp == false)
		{
			if (_cellCore != null)
			{
				_cellCore.ExitCube(this);
			}

			cell.ExitCube(this);
			_cellCore = cell;
		}
		else
		{
			_cellToDragCurrent = cell;
		}
		
	}


	public void OnDrag(PointerEventData eventData)
	{
		if (_canvas == null) _canvas = GameObject.Find("UI").GetComponent<Canvas>();

		var positionDeltaDrag = eventData.delta / _canvas.scaleFactor;
		RectTransformCube.anchoredPosition += positionDeltaDrag;

		SetSelectedCellXY();

		GameManager.Instance.Map[_selectedProductIDY, _selectedProductIDX].Cell.GetComponent<DebugCell>().SetDragColor();
		SetCellCore(
			cell: GameManager.Instance.Map[_selectedProductIDY, _selectedProductIDX], 
			isTemp: true);

		if(_cellCore != null && _cellToDragCurrent != _cellCore)
		{
			_cellCore.ExitCube(this);
			_cellCore = null;
		}

		ClampedCubeMoveToFreeCells();
	}

	private void SetSelectedCellXY()
	{
		var nearestPositionX = float.MaxValue;
		foreach (var cell in GameManager.Instance.Map)
		{
			float distance = Mathf.Abs(RectTransformCube.localPosition.x - cell.Position.x);
			if (distance < nearestPositionX)
			{
				nearestPositionX = distance;
				_selectedProductIDX = cell.X;
			}
		}

		var nearestPositionY = float.MaxValue;
		foreach (var cell in GameManager.Instance.Map)
		{
			float distance = Mathf.Abs(RectTransformCube.localPosition.y - cell.Position.y);
			if (distance < nearestPositionY)
			{
				nearestPositionY = distance;
				_selectedProductIDY = cell.Y;
			}
		}
	}

	public void OnBeginDrag(PointerEventData eventData)
	{
		_isDrag = true;
	}

	public void OnEndDrag(PointerEventData eventData)
	{
		SetCellCore(GameManager.Instance.Map[_selectedProductIDY, _selectedProductIDX]);
		_isDrag = false;
	}
	private void ClampedCubeMoveToFreeCells()
	{
		if (_cellToDragCurrent.DependenciesCells.Left?.IsFree == false || _cellToDragCurrent.DependenciesCells.Left?.Cube != null)
		{
			if (_cellToDragCurrent.DependenciesCells.Left?.Cube?.NumberCube == NumberCube) xMinClamp = _cellToDragCurrent.DependenciesCells.Left.Position.x;
			else xMinClamp = _cellToDragCurrent.Position.x;
		}
		else xMinClamp = -413.33f;

		if (_cellToDragCurrent.DependenciesCells.Right?.IsFree == false || _cellToDragCurrent.DependenciesCells.Right?.Cube != null)
		{
			if (_cellToDragCurrent.DependenciesCells.Right?.Cube?.NumberCube == NumberCube) xMaxClamp = _cellToDragCurrent.DependenciesCells.Right.Position.x;
			else xMaxClamp = _cellToDragCurrent.Position.x;
		}
		else xMaxClamp = 416.87f;

		if (_cellToDragCurrent.DependenciesCells.Up?.IsFree == false || _cellToDragCurrent.DependenciesCells.Up?.Cube != null)
		{
			if (_cellToDragCurrent.DependenciesCells.Up?.Cube?.NumberCube == NumberCube) yMaxClamp = _cellToDragCurrent.DependenciesCells.Up.Position.y;
			else yMaxClamp = _cellToDragCurrent.Position.y;
		}
		else yMaxClamp = 657.855f;

		if (_cellToDragCurrent.DependenciesCells.Down?.IsFree == false || _cellToDragCurrent.DependenciesCells.Down?.Cube != null)
		{
			if (_cellToDragCurrent.DependenciesCells.Down?.Cube?.NumberCube == NumberCube) yMinClamp = _cellToDragCurrent.DependenciesCells.Down.Position.y;
			else yMinClamp = _cellToDragCurrent.Position.y;
		}
		else yMinClamp = -656.5563f;

		var position = RectTransformCube.localPosition;
		position = new Vector2(Mathf.Clamp(position.x, xMinClamp, xMaxClamp), Mathf.Clamp(position.y, yMinClamp, yMaxClamp));
		RectTransformCube.localPosition = position;
	}

	public void OnPointerDown(PointerEventData eventData)
	{
		_isPresed = true;
		
		RectTransformCube.SetAsLastSibling();
		GetComponent<Rigidbody2D>().mass = 0;
	}

	public void OnPointerUp(PointerEventData eventData)
	{
		_isPresed = false;
		GetComponent<Rigidbody2D>().mass = 1;

		RectTransformCube.localPosition = GameManager.Instance.Map[_selectedProductIDY, _selectedProductIDX].Position;
	}
	public void OnCollisionEnter2D(Collision2D collision)
	{
		var cube = collision.collider.GetComponent<Cube>();
		if (cube != null && cube.NumberCube == NumberCube)
		{
			if (_isPresed)
			{
				_cellCore?.ExitCube(this);
				GameManager.Instance.DeleteCube(this);
				cube.NumberCube++;
			}
		}
	}

	public void ShowBrakes(Braces.Type type)
	{
		_brakes.Show(type);
	}

	[System.Serializable]
	public class Braces
	{
		public GameObject Up;
		public GameObject Down;
		public GameObject Right;
		public GameObject Left;

		public void Show(Type type)
		{
			Up.SetActive(false);
			Down.SetActive(false);
			Right.SetActive(false);
			Left.SetActive(false);

			switch (type)
			{
				case Type.None:
					break;
				case Type.Up:
					Up.SetActive(true);
					break;
				case Type.Down:
					Down.SetActive(true);
					break;
				case Type.Right:
					Right.SetActive(true);
					break;
				case Type.Left:
					Left.SetActive(true);
					break;
			}
		}
		public enum Type
		{
			None,
			Up,
			Down,
			Right,
			Left
		}
	}
}
