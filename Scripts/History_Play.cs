using Carrot;
using System;
using System.Collections.Generic;
using UnityEngine;

class History_Data
{
    public int score;
    public DateTime datime;
}

public class History_Play : MonoBehaviour
{
    public Sprite sp_icon_history;
    public GameObject history_item_prefab;
    private List<History_Data> list_history = new List<History_Data>();
    
    public void show_history()
    {
        this.GetComponent<Game>().play_sound();
        if (this.list_history.Count > 0)
        {
            Carrot_Box box_history = this.GetComponent<Game>().carrot.Create_Box();
            box_history.set_icon(sp_icon_history);
            box_history.set_title("Play History");

            for(int i = this.list_history.Count-1; i >= 0; i--)
            {
                GameObject obj_h = Instantiate(this.history_item_prefab);
                obj_h.transform.SetParent(box_history.area_all_item);
                obj_h.transform.localPosition = new Vector3(obj_h.transform.localPosition.x, obj_h.transform.localPosition.y,0f);
                obj_h.transform.localScale = new Vector3(1f,1f,1f);
                obj_h.GetComponent<History_Item>().txt_score.text = this.list_history[i].score.ToString();
                obj_h.GetComponent<History_Item>().txt_time.text = this.list_history[i].datime.ToString("g");
            }
            box_history.update_gamepad_cosonle_control();
        }
        else
        {
            this.GetComponent<Game>().carrot.show_msg("Play History", "No play history!",Msg_Icon.Alert);
        }
    }

    public void add_history(int score)
    {
        if (score > 0)
        {
            History_Data h = new History_Data();
            h.score = score;
            h.datime = DateTime.Now;
            this.list_history.Add(h);
        }
    }
}
