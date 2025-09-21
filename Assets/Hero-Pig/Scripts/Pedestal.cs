using UnityEngine;

public class Pedestal : MonoBehaviour
{
    private float speed = 0.8f;
    private float speed_default = 0.8f;
    public bool is_play = false;
    public int type = 0;
    public SpriteRenderer pic_pedestal;
    public GameObject obj_boom;
    public GameObject obj_boom_chat;

    public void on_load()
    {
        if (this.type == 2)
        {
            this.obj_boom.SetActive(true);
            this.obj_boom_chat.SetActive(false);
        }
        else if (this.type == 0) this.obj_boom.SetActive(false);
    }

    void Update()
    {
        if (this.is_play)
        {
            this.transform.Translate(Vector3.up * speed * Time.deltaTime);
            if (this.transform.position.y > 6f) Destroy(this.gameObject);
        }
    }

    public void set_speed(float f)
    {
        this.speed = f;
    }

    public void reset_speed()
    {
        this.speed = this.speed_default;
    }

    public void active_boom()
    {
        this.GetComponent<Animator>().enabled = true;
        this.obj_boom_chat.SetActive(true);
    }

    public void act_freeze(bool is_act)
    {
        if (is_act)
        {
            this.is_play = false;
            this.pic_pedestal.color = new Color32(0, 200, 225, 255);
            GameObject.Find("Game").GetComponent<Game>().bk.create_effect(this.transform.position, 3, 3f, 0.02f);
        }
        else
        {
            this.is_play = true;
            this.pic_pedestal.color = Color.white;
        }
    }

    public void bomb_explosion()
    {
        GameObject.Find("Game").GetComponent<Game>().play_sound(8);
        GameObject.Find("Game").GetComponent<Game>().bk.create_effect(this.transform.position,2,2f,0.02f);
        Destroy(this.gameObject);
    }
}
