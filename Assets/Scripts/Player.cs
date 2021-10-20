using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [Header("—корость")]
    public float Speed;
    private Transform _transform;
    private Rigidbody2D _rigidbody;
    private Animator _animationController;

    private EventState _eventState = EventState.Idle;
    private DirectionState _directionState = DirectionState.Down;

    private void Start()
    {
        _transform = GetComponent<Transform>();
        _rigidbody = GetComponent<Rigidbody2D>();
        _animationController = GetComponent<Animator>();
    }

    private void Update()
    {
        string animationName = GetAnimationName();
        if (animationName.Length != 0)
        {
            _animationController.Play(animationName);
        }
    }

    /*
     * ѕереместить персонажа согластно вектору direction
     */
    public void MovementByDirection(Vector2 direction)
    {
        if (direction != null)
        {
            _rigidbody.velocity = direction * Speed;
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

    /*
     * ѕолучение состо€ние Direction по смещению игрока 
     */
    private DirectionState GetDirectionState(float x, float y)
    {
        if (y == 0)
        {
            if (x > 0)
            {
                return DirectionState.Right;
            }
            if (x < 0)
            {
                return DirectionState.Left;
            }
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
}
