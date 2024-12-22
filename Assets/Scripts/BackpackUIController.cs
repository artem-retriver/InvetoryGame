using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class BackpackUIController : MonoBehaviour
{
    [SerializeField] private GameObject inventoryUI;
    [SerializeField] private GameObject poolItemUIGameObject;
    [SerializeField] private ItemUI itemUI;
    [SerializeField] private Text inventoryText;
    [SerializeField] private List<ItemUI> poolItemUI = new List<ItemUI>();
    
    private Outline _outline;
    
    private bool _isMouseOverBackpack = false;

    private void Start()
    {
        _outline = GetComponent<Outline>();
        
        _outline.enabled = false;
        inventoryUI.SetActive(false);
    }

    private void Update()
    {
        if (_isMouseOverBackpack && Input.GetMouseButton(0))
        {
            _outline.enabled = true;
            inventoryUI.SetActive(true);
        }
        else
        {
            _outline.enabled = false;
            inventoryUI.SetActive(false);
        }
    }
    
    private void OnMouseEnter()
    {
        _isMouseOverBackpack = true;
    }

    private void OnMouseExit()
    {
        _isMouseOverBackpack = false;
        _outline.enabled = false;
        inventoryUI.SetActive(false);
    }

    public void InstantiateItemUI(ItemConfig item)
    {
        inventoryText.text = $"{item.itemName} ({item.itemType})\n";
        itemUI.itemName = item.itemName;
        
        poolItemUI.Add(Instantiate(itemUI, poolItemUIGameObject.transform));
        poolItemUI[^1].gameObject.SetActive(true);
    }
    
    public void RemoveItemUI(ItemConfig removeItem)
    {
        foreach (var item in poolItemUI.ToList().Where(item => item.itemName == removeItem.itemName))
        {
            Destroy(item.gameObject, 0.1f);
            poolItemUI.Remove(item);
        }
    }
}