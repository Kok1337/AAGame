using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GridSystem;

public class Player : MonoBehaviour
{
	[Header("—корость")]
	public float Speed;
	private Transform _transform;
	private Rigidbody2D _rigidbody;
	private Animator _animationController;

	private EventState _eventState = EventState.Idle;
	private DirectionState _directionState = DirectionState.Down;

	private bool _pathFollowCoroutineIsStarted;
	private Coroutine _pathFollowCoroutine;

	private BackgroundManager _backgroundManager;

	private void Start()
	{
		_transform = GetComponent<Transform>();
		_rigidbody = GetComponent<Rigidbody2D>();
		_animationController = GetComponent<Animator>();

		_pathFollowCoroutineIsStarted = false;
	}

	private void Update()
	{
		string animationName = GetAnimationName();
		if (animationName.Length != 0)
		{
			_animationController.Play(animationName);
		}

		MovementByDirection(Vector2.zero);
	}

	public void MovementByDirectionFromKeyBoard(Vector2 direction)
	{
		if (_pathFollowCoroutineIsStarted)
		{
			StopCoroutine(_pathFollowCoroutine);
		}
		MovementByDirection(direction);
	}

	/*
     * ѕереместить персонажа согластно вектору direction
     */
	private void MovementByDirection(Vector2 direction)
	{
		if (direction != null)
		{
			Vector2 velocity = direction * Speed;
			if (_backgroundManager != null)
			{
				velocity *= _backgroundManager.GetWalkingSpeed(transform.position);
			}
			_rigidbody.velocity = velocity;
			SetCurrentStates(direction);
		}
	}

	/*
     * «адать новые состо€ни€ игрока по его направлению. 
     */
	private void SetCurrentStates(Vector2 direction)
	{
		float x = direction.x;
		float y = direction.y;

		_eventState = GetEventState(x, y);
		_directionState = GetDirectionState(x, y);
	}

	/*
     * ѕолучение состо€ние Event по смещению игрока 
     */
	private EventState GetEventState(float x, float y)
	{
		if (x == 0 && y == 0)
		{
			return EventState.Idle;
		}
		else
		{
			return EventState.Walk;
		}

		// return _eventState;
	}

	public void FollowPath(List<PathNode> path)
	{
		if (_pathFollowCoroutineIsStarted)
		{
			StopCoroutine(_pathFollowCoroutine);
		}
		_pathFollowCoroutine = StartCoroutine(PathFollowCoroutine(path));
	}

	private IEnumerator PathFollowCoroutine(List<PathNode> path)
	{
		_pathFollowCoroutineIsStarted = true;

		for (int i = 1; i < path.Count; i++)
		{
			PathNode currentNode = path[i];
			Vector3 nodeCenter = currentNode.GetNodeCenterPosition();
			nodeCenter.z = transform.position.z;

			while ((transform.position - nodeCenter).magnitude >= 0.2f)
			{
				Vector2 direction = nodeCenter - transform.position;
				MovementByDirection(direction.normalized);
				yield return null;
			}
		}

		_pathFollowCoroutineIsStarted = false;
	}

	/*
     * ѕолучение состо€ние Direction по смещению игрока 
     */
	private DirectionState GetDirectionState(float x, float y)
	{
		if (CloseToZero(y))
		{
			if (x > 0)
			{
				return DirectionState.Right;
			}
			if (x < 0)
			{
				return DirectionState.Left;
			}
			return _directionState;
		}

		if (CloseToZero(x))
		{
			if (y > 0)
			{
				return DirectionState.Up;
			}
			if (y < 0)
			{
				return DirectionState.Down;
			}
			return _directionState;
		}

		if (y > 0)
		{
			if (x > 0)
			{
				return DirectionState.UpRight;
			}
			else if (x < 0)
			{
				return DirectionState.UpLeft;
			}
			else
			{
				return DirectionState.Up;
			}
		}

		if (y < 0)
		{
			if (x > 0)
			{
				return DirectionState.DownRight;
			}
			else if (x < 0)
			{
				return DirectionState.DownLeft;
			}
			else
			{
				return DirectionState.Down;
			}
		}

		return _directionState;
	}

	/*
     * ѕровер€ет, близка ли координата к нолю. Ќужно дл€ определени€ направлени€.
     */
	private bool CloseToZero(float value)
	{
		return Mathf.Abs(value) < 0.1;
	}

	/*
     * ѕолучить название проигрываемой анимации по текущим состо€ни€м
     */
	private string GetAnimationName()
	{
		string eventString = "";
		switch (_eventState)
		{
			case EventState.Idle:
				eventString = "idle";
				break;
			case EventState.Walk:
				eventString = "walk";
				break;
		}

		string directionString = "";
		switch (_directionState)
		{
			case DirectionState.Down:
				directionString = "down";
				break;
			case DirectionState.Up:
				directionString = "up";
				break;
			case DirectionState.Right:
				directionString = "right";
				break;
			case DirectionState.Left:
				directionString = "left";
				break;
			case DirectionState.UpLeft:
				directionString = "up_left";
				break;
			case DirectionState.UpRight:
				directionString = "up_right";
				break;
			case DirectionState.DownLeft:
				directionString = "down_left";
				break;
			case DirectionState.DownRight:
				directionString = "down_right";
				break;
		}

		return eventString + '_' + directionString;
	}

	private enum EventState
	{
		Idle,
		Walk
	}

	private enum DirectionState
	{
		Up,
		Down,
		Left,
		Right,
		UpLeft,
		UpRight,
		DownLeft,
		DownRight
	}

	public BackgroundManager BackgroundManager
	{
		set => _backgroundManager = value;
	}
}
