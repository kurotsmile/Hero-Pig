using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Item_existence : MonoBehaviour
{
    public Image icon;
    public int type_food;
    public int index;

    public void click()
    {
        GameObject.Find("Game").GetComponent<Game>().pig.eat_food(type_food);
        GameObject.Find("Game").GetComponent<Game>().bk.show_item_use(this.icon.sprite);
        GameObject.Find("Game").GetComponent<Game>().shop.remove_item_existence(this.index);
    }
}
