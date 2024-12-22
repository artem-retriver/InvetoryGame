using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Networking;

public class BackpackManager : MonoBehaviour
{
    [SerializeField] private Transform[] slots;
    [SerializeField] private List<ItemConfig> items = new List<ItemConfig>();
    
    public static BackpackManager Instance;
    
    private BackpackUIController _backpackUIController;
    private UnityEvent<ItemConfig> OnItemAdded;
    private UnityEvent<ItemConfig> OnItemRemoved;
   
    private void Awake()
    {
        Instance = GetComponent<BackpackManager>();
    }

    private void Start()
    {
        _backpackUIController = GetComponent<BackpackUIController>();
    }

    public void AddItem(ItemConfig item, bool hasThisItemInBackpack)
    {
        if (hasThisItemInBackpack == false)
        {
            items.Add(item);
            OnItemAdded?.Invoke(item);
            _backpackUIController.InstantiateItemUI(item);
        }
        
        StartCoroutine(SnapToSlot(item));
        SendEventToServer(item.itemIdentifier, "Added");
    }

    public void RemoveItem(ItemConfig item)
    {
        items.Remove(item);
        OnItemRemoved?.Invoke(item);
        _backpackUIController.RemoveItemUI(item);
        
        StartCoroutine(SnapToWorld(item));
        SendEventToServer(item.itemIdentifier, "Removed");
    }

    private Transform GetSlotForItemType(string itemType)
    {
        return itemType switch
        {
            "Sleep" => slots[0],
            "Drink" => slots[1],
            "Tool" => slots[2],
            _ => null
        };
    }

    private IEnumerator SnapToSlot(ItemConfig item)
    {
        Transform slot = GetSlotForItemType(item.itemType);
        Vector3 startPosition = item.transform.position;
        Quaternion startRotation = item.transform.rotation;
        
        float time = 0;
        
        while (time < 1)
        {
            item.transform.position = Vector3.Lerp(startPosition, slot.position, time);
            item.transform.rotation = Quaternion.Lerp(startRotation, slot.rotation, time);
            time += Time.deltaTime;
            
            yield return null;
        }

        item.transform.position = slot.position;
        item.transform.rotation = slot.rotation;
    }

    private IEnumerator SnapToWorld(ItemConfig item)
    {
        Vector3 targetPosition = item.transform.position + Vector3.down * 2;
        targetPosition.y = Mathf.Max(targetPosition.y, 0);
        Vector3 startPosition = item.transform.position;

        float time = 0;
        
        while (time < 1)
        {
            item.transform.position = Vector3.Lerp(startPosition, targetPosition, time);
            time += Time.deltaTime;
            
            yield return null;
        }

        item.transform.position = targetPosition;
    }

    private void SendEventToServer(string id, string action)
    {
        StartCoroutine(SendPostRequest(id, action));
    }

    private IEnumerator SendPostRequest(string id, string action)
    {
        var jsonData = JsonUtility.ToJson(new { identifier = id, @event = action });
        
        UnityWebRequest request = new UnityWebRequest("https://wadahub.manerai.com/api/inventory/status", "POST");
        request.SetRequestHeader("Authorization", "Bearer kPERnYcWAY46xaSy8CEzanosAgsWM84Nx7SKM4QBSqPq6c7StWfGxzhxPfDh8MaP");
        request.SetRequestHeader("Content-Type", "application/json");
        
        byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonData);
        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            Debug.Log($"Server Response: {request.downloadHandler.text}");
        }
        else
        {
            Debug.LogError($"Error: {request.error}");
        }
    }
}