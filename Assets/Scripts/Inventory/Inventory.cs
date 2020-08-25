﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class Inventory : MonoBehaviour
{
    [SerializeField] private List<Item> Items;
    [SerializeField] private List<Item> Equips;
    [SerializeField] private InventoryCell _inventoryCellTemplate;
    [SerializeField] private Transform _container;
    [SerializeField] private Transform _originalParent;
    [SerializeField] private Transform _draggingParent;
    [SerializeField] private Transform _equipParent;
    [SerializeField] private Transform _infoPanel;
    public int _capacity = 40;

    ///public Button InvButton;


    private void Start()
    {
        //InvButton.onClick.AddListener(ClickBtn);
        //this._draggingParent.gameObject.SetActive(false);
        Items = new List<Item>();

        (_container as RectTransform).sizeDelta =  new Vector2 (0, 110 + (_capacity / 10 - 1) * 105);

        for (int i = 0; i < /*UnityEngine.Random.Range(5, */_capacity-5/* + 1)*/; i++)
        {
            Items.Add(ScriptableObject.CreateInstance("Item") as Item);
            Items[i].Data = new ItemData() { type = ItemType.Disposable };
            Items[i].Name = "PFNSDHKFHDS";
        }

        Items.Add(new Item() { Data = new ItemData() { type = ItemType.Boots }, Name = "PFNSDHKFHDS" });
        Items.Add(new Item() { Data = new ItemData() { type = ItemType.Bracers }, Name = "PFNSDHKFHDS" });
        Items.Add(new Item() { Data = new ItemData() { type = ItemType.Armor }, Name = "PFNSDHKFHDS" });
        Items.Add(new Item() { Data = new ItemData() { type = ItemType.Backpack }, Name = "PFNSDHKFHDS" });
        Items.Add(new Item() { Data = new ItemData() { type = ItemType.Pants }, Name = "PFNSDHKFHDS" });

        foreach (var item in Items)
        {
            var cell = Instantiate(_inventoryCellTemplate, _container);
            cell.Init(_originalParent,_draggingParent, _equipParent);
            cell.Render(item);

            cell.Ejection += Destroyer;
            cell.Deselection += InfoHide;
            cell.Selection += InfoSet;
            cell.Using += UseItem;
        }
    }

    //void ClickBtn()
    //{
    //    this._draggingParent.gameObject.active = !this._draggingParent.gameObject.active;
    //    Debug.Log("Inventory");
    //}

    private void OnEnable()
    {
        Render(Items);
    }

    public void Render(List <Item> items)
    {
        foreach (Transform child in _container)
            Destroy(child.gameObject);


        foreach (var item in items) 
        {
            var cell = Instantiate(_inventoryCellTemplate, _container);
            cell.Init(_originalParent, _draggingParent, _equipParent);
            cell.Render(item);

            cell.Ejection += Destroyer;
            cell.Deselection += InfoHide;
            cell.Selection += InfoSet;
            cell.Using += UseItem;
        }
    }

    private void Destroyer(object sender, EventArgs e)
    {
            Destroy(((InventoryCell)sender).gameObject);
            //Items.Remove();
    }

    private static readonly object syncLock = new object();

    private void InfoSet(object sender, EventArgs e)
    {
        _infoPanel.gameObject.SetActive(true);
        _infoPanel.transform.position = (sender as InventoryCell).transform.position;
        if (_infoPanel.transform.localPosition.y < -100)
        {
            _infoPanel.transform.localPosition = new Vector2(_infoPanel.transform.localPosition.x, -100);
        }
        GameObject.Find("NameItem").GetComponent<Text>().text = (sender as InventoryCell)._item.Name;
        GameObject.Find("ImageItem").GetComponent<Image>().sprite = (sender as InventoryCell)._item.UIIcon;
        GameObject.Find("TypeItem").GetComponent<Text>().text = (sender as InventoryCell)._item.Data.type.ToString();
        GameObject.Find("DescrItem").GetComponent<Text>().text = (sender as InventoryCell)._item.Description;

        Debug.LogWarning("//TODO: Effects inventory");

    }

    private void InfoHide(object sender, EventArgs e)
    {
            _infoPanel.gameObject.SetActive(false);
    }

    private void UseItem(object sender, EventArgs e)
    {
        if ((sender as InventoryCell)._item.Use())
        {
            if ((sender as InventoryCell)._item.Data.type > (ItemType)1
                && (sender as InventoryCell).transform.parent != _equipParent
                .Find((sender as InventoryCell)._item.Data.type.ToString() + "BackCell"))
            {
                (sender as InventoryCell).transform.SetParent(_equipParent.Find((sender as InventoryCell)
                    ._item.Data.type.ToString() + "BackCell"));
                (sender as InventoryCell).transform.localPosition = new Vector3(0, 0, 0);
                Equips.Add((sender as InventoryCell)._item);
                Items.Remove((sender as InventoryCell)._item);
            }
            else if ((sender as InventoryCell).transform.parent == _equipParent
                .Find((sender as InventoryCell)._item.Data.type.ToString() + "BackCell"))
            {
                (sender as InventoryCell).transform.SetParent(_originalParent);
                (sender as InventoryCell).transform.localPosition = new Vector3(0, 0, 0);
                Items.Add((sender as InventoryCell)._item);
                Equips.Remove((sender as InventoryCell)._item);
            }
            else if ((sender as InventoryCell)._item.Data.type == ItemType.Disposable)
            {
                Items.Remove((sender as InventoryCell)._item);
                Destroyer(sender, e);
            }
            
        }
        
    }

}