using UnityEngine;
using UnityEngine.UI;

public class DragAndDrop : MonoBehaviour
{
    private Camera _mainCamera;
    private Rigidbody _rigidbody;
    private BoxCollider _boxCollider;
    private Outline _outline;
    
    private float _initialDistanceToCamera;
    private bool _isTakeItem;

    private void Start()
    {
        _mainCamera = Camera.main;
        _rigidbody = GetComponent<Rigidbody>();
        _boxCollider = GetComponent<BoxCollider>();
        _outline = GetComponent<Outline>();
        
        _outline.enabled = false;
    }

    private void OnMouseDown()
    {
        _outline.enabled = true;
        _boxCollider.enabled = false;
        _initialDistanceToCamera = Vector3.Distance(transform.position, _mainCamera.transform.position);
    }

    private void OnMouseDrag()
    {
        Vector3 mouseWorldPosition = GetMouseWorldPosition();
        transform.position = new Vector3(mouseWorldPosition.x, mouseWorldPosition.y, transform.position.z);
        transform.position = new Vector3(transform.position.x, Mathf.Max(transform.position.y, 0), transform.position.z);
    }

    private void OnMouseUp()
    {
        _outline.enabled = false;
        _boxCollider.enabled = true;
        
        RaycastHit hit;
        
        if (Physics.Raycast(_mainCamera.ScreenPointToRay(Input.mousePosition), out hit))
        {
            var backpack = hit.collider.GetComponent<BackpackManager>();
            
            if (backpack)
            {
                BackpackManager.Instance.AddItem(GetComponent<ItemConfig>(), _isTakeItem);
                
                _isTakeItem = true;
                _rigidbody.isKinematic = true;
            }
            else
            {
                if (_isTakeItem)
                {
                    BackpackManager.Instance.RemoveItem(GetComponent<ItemConfig>());
                    _isTakeItem = false;
                }
                
                _rigidbody.isKinematic = false;
            }
        }
    }

    private Vector3 GetMouseWorldPosition()
    {
        Vector3 mousePosition = Input.mousePosition;
        mousePosition.z = _initialDistanceToCamera;
        return _mainCamera.ScreenToWorldPoint(mousePosition);
    }
}