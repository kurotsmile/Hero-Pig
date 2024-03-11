using System;
using UnityEngine;

public class Pig : MonoBehaviour
{
    public string name_hero;
    public float speed = 1f;
    public Rigidbody2D rigidbody_pig;
    public Transform tr_pig_pic;
    public Animator anim;
    public GameObject obj_undying;
    public GameObject obj_angry;
    public Camera cam;
    public Transform tr_pos_start;
    private Pedestal Pedestal_cur = null;
    private float distance_move = 200f;
    private float timer_food = 0f;
    private int scores = 0;
    private int hight_scores = 0;

    private bool is_big = false;
    private bool is_undying = false;
    private bool is_angry = false;
    private bool is_die = false;
    private bool is_cam_follow = false;

    public void load()
    {
        this.hight_scores = PlayerPrefs.GetInt("hight_scores_"+this.name_hero,0);
        this.is_cam_follow = false;
    }

    public void move_left()
    {
        if (this.is_die) return;
        this.anim.Play("Move");
        this.tr_pig_pic.localScale = new Vector3(1f, 1f, 1f);
        rigidbody_pig.AddForce(-transform.right * distance_move,ForceMode2D.Force); ;
    }

    public void move_right()
    {
        if (this.is_die) return;
        this.anim.Play("Move");
        this.tr_pig_pic.localScale = new Vector3(-1f, 1f, 1f);
        rigidbody_pig.AddForce(transform.right * distance_move, ForceMode2D.Force);
    }

    private void move_up()
    {
        GameObject.Find("Game").GetComponent<Game>().play_sound(3);
        this.anim.Play("Move");
        rigidbody_pig.AddForce(transform.up * (distance_move+600f), ForceMode2D.Force);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.name == "limit_bottom")
        {
            if (this.is_undying)
            {
                this.move_up();
            }
            else
            {
                GameObject.Find("Game").GetComponent<Game>().play_sound(6);
                this.die();
            }
        }

        if (collision.gameObject.name == "Coin")
        {
            GameObject.Find("Game").GetComponent<Game>().play_sound(1);
            GameObject.Find("Game").GetComponent<Game>().bk.add_coin();
            GameObject.Find("Game").GetComponent<Game>().bk.add_scores(1);
            this.scores++;
            Destroy(collision.gameObject);
        }

        if (collision.gameObject.name == "Snail")
        {
            collision.gameObject.GetComponent<Animator>().Play("Snail_die");
            this.catch_animals(collision.gameObject);
        }

        if (collision.gameObject.name == "Snake")
        {
            collision.gameObject.GetComponent<Animator>().Play("Snake_die");
            this.catch_animals(collision.gameObject);
        }

        if (collision.gameObject.name == "Pedestal")
        {
            this.Pedestal_cur = collision.gameObject.GetComponent<Pedestal>();
            if (this.is_big) this.Pedestal_cur.set_speed(0.4f);
            if (this.Pedestal_cur.type == 2) this.Pedestal_cur.active_boom();
        }

        if (collision.gameObject.name == "Food")
        {
            
            GameObject.Find("Game").GetComponent<Game>().bk.create_effect(this.transform.position,1);
            Food food= collision.gameObject.GetComponent<Food>();
            this.eat_food(food.type);
            GameObject.Find("Game").GetComponent<Game>().bk.show_item_use(food.sp_render.sprite);
            Destroy(collision.gameObject);
        }

        if (collision.gameObject.name == "limit_right")
        {
            this.move_left();
        }

        if (collision.gameObject.name == "limit_left")
        {
            this.move_right();
        }
    }

    public void eat_food(int type_food)
    {
        GameObject.Find("Game").GetComponent<Game>().play_sound(4);
        if (type_food == 5)
            this.act_undying();
        else if (type_food == 6)
            GameObject.Find("Game").GetComponent<Game>().bk.freeze_all_Pedestal();
        else if (type_food == 7)
            GameObject.Find("Game").GetComponent<Game>().bk.additional_stone_pedestal();
        else if (type_food == 8)
            this.act_angry();
        else if (type_food == 9)
            GameObject.Find("Game").GetComponent<Game>().bk.kill_all_animals();
        else
        {
            GameObject.Find("Game").GetComponent<Game>().bk.add_scores(type_food);
            this.act_pig();
        }
            
    }

    private void catch_animals(GameObject animals)
    {
        GameObject.Find("Game").GetComponent<Game>().play_sound(6);
        if (this.is_big||this.is_undying||this.is_angry)
        {
            animals.GetComponent<BoxCollider2D>().enabled = false;
            animals.GetComponent<Rigidbody2D>().freezeRotation = false;
            GameObject.Find("Game").GetComponent<Game>().bk.add_scores(1);
            this.scores++;
        }
        else
        {
            this.die();
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.name == "Pedestal")
        {
            if (this.is_big || this.Pedestal_cur != null) GameObject.Find("Game").GetComponent<Game>().bk.reset_all_speed_Pedestal();
            this.Pedestal_cur = null;
        }
    }

    public void Reset()
    {
        this.scores = 0;
        this.is_die = false;
        this.reset_act_food();
        this.transform.position = this.tr_pos_start.position;
        this.obj_undying.SetActive(false);
        this.obj_angry.SetActive(false);
        this.gameObject.SetActive(true);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        GameObject.Find("Game").GetComponent<Game>().play_sound(6);
        this.die();
    }

    private void Update()
    {
        if (this.is_big || this.is_undying||this.is_angry)
        {
            this.timer_food += 0.1f * Time.deltaTime;
            if (this.timer_food > 1f)
            {
                GameObject.Find("Game").GetComponent<Game>().play_sound(6);
                if (this.is_big) GameObject.Find("Game").GetComponent<Game>().bk.reset_all_speed_Pedestal();
                this.reset_act_food();
            }
        }
    }

    private void FixedUpdate()
    {
        if (this.is_cam_follow)
        {
            float zoom_z;
            if (this.rigidbody_pig.position.x < 0f) zoom_z = Math.Abs(this.rigidbody_pig.position.x);
            else zoom_z = this.rigidbody_pig.position.x;

            zoom_z = zoom_z * 0.2f;
            this.cam.orthographicSize = 5 + zoom_z;
        }
    }

    private void die()
    {
        this.is_die = true;
        GameObject.Find("Game").GetComponent<Game>().panel_play.SetActive(false);
        GameObject.Find("Game").GetComponent<Game>().act_vibrates();
        GameObject.Find("Game").GetComponent<Game>().bk.create_effect(this.transform.position);
        this.gameObject.SetActive(false);
        GameObject.Find("Game").GetComponent<Game>().carrot.delay_function(1.5f, GameObject.Find("Game").GetComponent<Game>().show_game_over);
    }

    private void reset_act_food()
    {
        this.is_undying = false;
        this.is_big = false;
        this.is_angry = false;
        this.timer_food = 0f;
        this.obj_undying.SetActive(false);
        this.obj_angry.SetActive(false);
        this.transform.localScale = new Vector3(1f, 1f, 1f);
    }

    private void act_undying()
    {
        this.reset_act_food();
        this.obj_undying.SetActive(true);
        this.is_undying = true;
    }

    private void act_pig()
    {
        this.reset_act_food();
        this.is_big = true;
        if (this.Pedestal_cur != null) this.Pedestal_cur.set_speed(0.3f);
        this.transform.localScale = new Vector3(1.5f, 1.5f, 1.5f);
    }

    private void act_angry()
    {
        this.reset_act_food();
        this.is_angry = true;
        this.obj_angry.SetActive(true);
    }

    public void add_scores()
    {
        this.scores++;
    }

    public int get_scores()
    {
        return this.scores;
    }

    public int get_highst_scores()
    {
        if (this.scores > this.hight_scores)
        {
            this.hight_scores = scores;
            PlayerPrefs.SetInt("hight_scores_" + this.name_hero, this.scores);
        }
        return this.hight_scores;
    }

    public void set_cam_follow(bool is_act)
    {
        this.is_cam_follow = is_act;
    }
}
