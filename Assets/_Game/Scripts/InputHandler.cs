using UnityEngine;

public class InputHandler : MonoBehaviour
{
    [SerializeField] private float _swapThreshold = 0.5f;

    private GridManager _gridManager;
    private Vector2 _initialTouchPosition;
    private bool _isSwapping;

    private void Awake() => _gridManager = GetComponent<GridManager>();

    private void Update()
    {
        HandleInput();
    }

    private void HandleInput()
    {
        if (Input.GetMouseButtonDown(0))
        {
            _initialTouchPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            _gridManager.SelectTile(_initialTouchPosition);
            _isSwapping = false;
        }

        if (Input.GetMouseButton(0) && !_isSwapping)
        {
            Vector2 currentPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            if (Vector2.Distance(currentPos, _initialTouchPosition) > _swapThreshold)
            {
                _isSwapping = _gridManager.AttemptSwap(_initialTouchPosition, currentPos);
            }
        }

        if (Input.GetMouseButtonUp(0))
        {
            _isSwapping = false;
        }
    }
}