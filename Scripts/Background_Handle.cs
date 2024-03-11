using UnityEngine;
using UnityEngine.UI;

public class Background_Handle : MonoBehaviour
{
    public GameObject Pedestal_prefab;
    public GameObject Pedestal_mini_prefab;
    public GameObject Coin_prefab;
    public GameObject Snail_prefab;
    public GameObject Snake_prefab;
    public GameObject food_prefab;
    public Text txt_coin;
    public Text txt_score;
    public Sprite[] sp_pedestal;
    public Sprite[] sp_pedestal_mini;
    public Sprite[] sp_food;
    private float timer_create_pedestal = 0f;
    private float timer_change_random_pedestal = 0f;
    private float[] pos_start_pedestal= new float[] { -0.6f, -2.67f, -4.32f };
    private bool is_play = false;
    private int coin = 0;
    private int score = 0;
    private int index_sp_pedestal;
    private bool is_freeze_all_Pedestal = false;
    private float timer_freeze_all_Pedestal = 0f;
    private bool is_item_show_user = false;
    private float timer_item_show_user = 2f;

    [Header("Effect")]
    public GameObject[] obj_effect_prefab;

    [Header("Item use")]
    public Image img_icon_item_use;
    public Text txt_timer_item_use;
    public GameObject panel_item_use;

    public void load_bk()
    {
        this.coin = PlayerPrefs.GetInt("game_coin",0);
        this.reset();
    }

    private Pedestal create_pedestal()
    {
        float rand_x = Random.Range(-2.5f, 2.5f);
        return this.create_pedestal(new Vector2(rand_x, -5.3f));
    }

    private void create_pedestal(float pos_y)
    {
        float rand_x = Random.Range(-2.04f, 2.04f);
        this.create_pedestal(new Vector2(rand_x, pos_y));
    }

    private Pedestal create_pedestal(Vector2 pos,int type_p=-1)
    {
        GameObject p;
        int r_p;
        if (type_p == -1)
            r_p = Random.Range(0, 5);
        else
            r_p = type_p;

        if (r_p == 1)
        {
            p = Instantiate(this.Pedestal_mini_prefab);
            p.GetComponent<Pedestal>().pic_pedestal.sprite = this.sp_pedestal_mini[this.index_sp_pedestal];
            p.GetComponent<Pedestal>().type = 1;
        }
        else if (r_p == 2)
        {
            p = Instantiate(this.Pedestal_prefab);
            p.GetComponent<Pedestal>().pic_pedestal.sprite = this.sp_pedestal[this.index_sp_pedestal];
            p.GetComponent<Pedestal>().type = 2;
        }
        else
        {
            p = Instantiate(this.Pedestal_prefab);
            p.GetComponent<Pedestal>().pic_pedestal.sprite = this.sp_pedestal[this.index_sp_pedestal];
            p.GetComponent<Pedestal>().type = 0;
        }
        p.GetComponent<Pedestal>().on_load();
        p.name = "Pedestal";
        p.transform.SetParent(transform);
        p.transform.position = new Vector3(pos.x, pos.y, 0f);
        p.transform.localScale = new Vector3(1f, 1f, 0f);
        p.GetComponent<Pedestal>().is_play = this.is_play;
        
        return p.GetComponent<Pedestal>();
    }

    private GameObject create_coin(Vector2 pos)
    {
        GameObject c = Instantiate(this.Coin_prefab);
        c.name = "Coin";
        c.transform.SetParent(transform);
        c.transform.position = new Vector3(pos.x, pos.y+1f, 0f);
        c.transform.localScale = new Vector3(1f, 1f, 0f);
        return c;
    }

    private GameObject create_food(Vector2 pos)
    {
        GameObject f = Instantiate(this.food_prefab);
        f.name = "Food";
        f.transform.SetParent(transform);
        f.transform.position = new Vector3(pos.x, pos.y + 1f, 0f);
        f.transform.localScale = new Vector3(1f, 1f, 0f);
        int r_index_sp_foord = Random.Range(0, this.sp_food.Length);
        f.GetComponent<Food>().sp_render.sprite = this.sp_food[r_index_sp_foord];
        f.GetComponent<Food>().type = r_index_sp_foord;
        return f;
    }

    private GameObject create_snail(Vector2 pos)
    {
        GameObject s = Instantiate(this.Snail_prefab);
        s.name = "Snail";
        s.transform.SetParent(transform);
        s.transform.position = new Vector3(pos.x, pos.y + 0.8f, 0f);
        s.transform.localScale = new Vector3(0.35f, 0.35f, 0f);
        return s;
    }

    private GameObject create_snake(Vector2 pos)
    {
        GameObject s = Instantiate(this.Snake_prefab);
        s.name = "Snake";
        s.transform.SetParent(transform);
        s.transform.position = new Vector3(pos.x, pos.y + 0.8f, 0f);
        s.transform.localScale = new Vector3(1.3f, 1.3f, 1.3f);
        return s;
    }

    public void create_effect(Vector2 pos,int index_type=0,float timer_out=2f,float size= 0.1f)
    {
        GameObject effect = Instantiate(this.obj_effect_prefab[index_type]);
        effect.name = "Effect";
        effect.transform.SetParent(transform);
        effect.transform.position = new Vector3(pos.x, pos.y, 0f);
        effect.transform.localScale = new Vector3(size, size, size);
        Destroy(effect, timer_out);
    }

    private void Update()
    {
        if (this.is_play)
        {
            this.timer_create_pedestal += 1f * Time.deltaTime;
            if (this.timer_create_pedestal > 2f)
            {
                if (Random.Range(0, 5) < 4)
                {
                    Pedestal p_new = this.create_pedestal();
                    int rand_create = Random.Range(0, 8);
                    if (rand_create == 0) Destroy(this.create_coin(p_new.transform.position), 6f);
                    if (rand_create == 5) Destroy(this.create_food(p_new.transform.position), 6f);
                    if (p_new.type == 0)
                    {
                        if (rand_create == 1) Destroy(this.create_snail(p_new.transform.position), 9f);
                        if (rand_create == 3) Destroy(this.create_snake(p_new.transform.position), 9f);
                    }
                }

                this.timer_create_pedestal = 0;
            }

            this.timer_change_random_pedestal += 0.5f * Time.deltaTime;
            if (this.timer_change_random_pedestal > 20f)
            {
                GameObject.Find("Game").GetComponent<Game>().random_weather();
                this.random_skin_pedastal();
                this.timer_change_random_pedestal = 0;
            }

            if (this.is_item_show_user)
            {
                this.timer_item_show_user -= 0.1f * Time.deltaTime;
                this.txt_timer_item_use.text = this.timer_item_show_user.ToString("N2");
                if (this.timer_item_show_user <= 0)
                {
                    this.is_item_show_user = false;
                    this.panel_item_use.SetActive(false);
                    this.timer_item_show_user = 1f;
                }
            }
        }

        if (this.is_freeze_all_Pedestal)
        {
            this.timer_freeze_all_Pedestal += 0.1f * Time.deltaTime;
            if (this.timer_freeze_all_Pedestal > 1f)
            {
                foreach (Transform tr in this.transform) if (tr.gameObject.name == "Pedestal") tr.gameObject.GetComponent<Pedestal>().act_freeze(false);
                this.timer_freeze_all_Pedestal = 0;
                this.is_freeze_all_Pedestal = false;
            }
        }
    }

    public void set_status_play(bool is_play)
    {
        this.is_play = is_play;
        foreach(Transform tr in this.transform)
        {
            if (tr.gameObject.name == "Pedestal") tr.gameObject.GetComponent<Pedestal>().is_play = is_play;
        }
    }

    public void reset()
    {
        this.timer_change_random_pedestal = 0f;
        this.timer_create_pedestal = 0f;
        this.coin = PlayerPrefs.GetInt("game_coin", 0);
        this.score = 0;
        this.txt_coin.text = this.coin.ToString();
        this.txt_score.text = "0";
        foreach (Transform tr in this.transform) if (tr.gameObject.name == "Pedestal"|| tr.gameObject.name == "Coin" || tr.gameObject.name == "Snail" || tr.gameObject.name == "Snake" || tr.gameObject.name == "Food") Destroy(tr.gameObject);

        this.random_skin_pedastal();
        this.create_pedestal(new Vector2(0, 1.31f),0);

        for (int i = 0; i < this.pos_start_pedestal.Length; i++) this.create_pedestal(this.pos_start_pedestal[i]);

        this.panel_item_use.SetActive(false);
    }

    public void reset_all_speed_Pedestal()
    {
        foreach (Transform tr in this.transform) if (tr.gameObject.name == "Pedestal") tr.gameObject.GetComponent<Pedestal>().reset_speed();
    }

    public void add_coin(int c=1)
    {
        this.coin += c;
        this.txt_coin.text = this.coin.ToString();
        PlayerPrefs.SetInt("game_coin", this.coin);
    }

    public void add_scores(int scores)
    {
        this.score += scores;
        this.txt_score.text = this.score.ToString();
    }

    public void subtraction_coin(int c)
    {
        this.coin -= c;
        this.txt_coin.text = this.coin.ToString();
        GameObject.Find("Game").GetComponent<Game>().txt_coin_main.text = this.coin.ToString();
        PlayerPrefs.SetInt("game_coin", this.coin);
    }

    private void random_skin_pedastal()
    {
        this.index_sp_pedestal = Random.Range(0, this.sp_pedestal.Length);
    }

    public int get_your_score()
    {
        return this.score;
    }

    public int get_your_coin()
    {
        return this.coin;
    }

    public void freeze_all_Pedestal()
    {
        this.is_freeze_all_Pedestal = true;
        GameObject.Find("Game").GetComponent<Game>().play_sound(9);
        foreach (Transform tr in this.transform) if (tr.gameObject.name == "Pedestal") tr.gameObject.GetComponent<Pedestal>().act_freeze(true);
    }

    public void kill_all_animals()
    {
        foreach (Transform tr in this.transform)
        {
            if (tr.gameObject.name == "Snail" || tr.gameObject.name == "Snake")
            {
                this.create_effect(tr.position,4);
                Destroy(tr.gameObject);
            }
        }
    }

    public void additional_stone_pedestal()
    {
        for(int i = 2; i <= Random.Range(2, 5); i++)
        {
            Vector2 pos_create = new Vector2(Random.Range(-4, 4),Random.Range(-2, -4.2f));
            this.create_pedestal(pos_create);
            this.create_effect(pos_create, 5);
        }
    }

    public void show_item_use(Sprite icon_item)
    {
        this.is_item_show_user = true;
        this.img_icon_item_use.sprite = icon_item;
        this.panel_item_use.SetActive(true);
        this.timer_item_show_user = 1f;
    }

}
