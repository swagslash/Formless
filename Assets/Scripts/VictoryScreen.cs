using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VictoryScreen : MonoBehaviour
{

    public Item leftItem;
    public Item rightItem;
    public GameManager gameManager;

    // Start is called before the first frame update
    void Start()
    {
        gameManager = FindObjectOfType<GameManager>();   
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ShowUI(){
        gameObject.SetActive(true);
    }

    public void HideUI() {
        gameObject.SetActive(false);
    }

    public void SetItems(Item leftItem, Item rightItem) {
        this.leftItem = leftItem;
        this.rightItem = rightItem;

        // TODO update UI elements (text, sprites etc.)
    }

    public void ItemSelected() {
        gameManager.StartNextLevel();
    }
}
