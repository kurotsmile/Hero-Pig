using Carrot;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Game : MonoBehaviour, Carrot_Gamepad_Event
{
    [Header("Main obj")]
    public Carrot.Carrot carrot;
    public GameObject panel_menu;
    public GameObject panel_play;
    public GameObject panel_play_one;
    public GameObject panel_play_two;
    public GameObject panel_game_over;
    private bool is_play = false;
    private bool is_play_one = false;

    [Header("Game obj")]
    public Background_Handle bk;
    public Shop_Manager shop;
    public SmoothCamera2D camSmoothCamera;
    public Pig pig;
    public Pig pig2;
    public Text txt_high_score_main;
    public Text txt_coin_main;
    public Text txt_over_your_score;
    public Text txt_over_high_score;
    private int score_high;

    [Header("GameOver Obj")]
    public GameObject panel_gameover_one_player;
    public GameObject panel_gameover_two_player;
    public Text txt_p1_one_gameover_score;
    public Text txt_p2_one_gameover_score;
    public Text txt_p1_one_gameover_hight_score;
    public Text txt_p2_one_gameover_hight_score;

    [Header("Game sound")]
    public AudioSource[] sound;

    [Header("Gamepad")]
    public List<GameObject> list_btn_main;
    public List<GameObject> list_btn_gameover;

    [Header("Effect")]
    public GameObject[] effect_weather;

    private KeyCode[] KeyCode_default2 = new KeyCode[10];

    void Start()
    {
        Screen.sleepTimeout = SleepTimeout.NeverSleep;

        this.carrot.Load_Carrot(this.check_exit_app);
        this.carrot.shop.onCarrotPaySuccess += this.carrot_by_success;
        this.carrot.ads.onRewardedSuccess+=this.on_Rewarded_Success;
        this.carrot.act_after_close_all_box = this.act_after_close_all_box;

        Carrot_Gamepad gamepad2 = this.carrot.game.create_gamepad("player_hero_2");
        KeyCode_default2[0] = KeyCode.O;
        KeyCode_default2[1] = KeyCode.J;
        KeyCode_default2[2] = KeyCode.L;
        KeyCode_default2[3] = KeyCode.K;
        KeyCode_default2[4] = KeyCode.N;
        KeyCode_default2[5] = KeyCode.M;
        KeyCode_default2[6] = KeyCode.Y;
        KeyCode_default2[7] = KeyCode.F;
        KeyCode_default2[8] = KeyCode.G;
        KeyCode_default2[9] = KeyCode.H;
        gamepad2.set_KeyCode_default(this.KeyCode_default2);
        gamepad2.set_gamepad_keydown_left(this.btn_pig2_left);
        gamepad2.set_gamepad_keydown_right(this.btn_pig2_right);


        Carrot_Gamepad gamepad1 = this.carrot.game.create_gamepad("player_hero_1");
        gamepad1.set_gamepad_keydown_left(this.gamepad_keydown_left);
        gamepad1.set_gamepad_keydown_right(this.gamepad_keydown_right);
        gamepad1.set_gamepad_keydown_up(this.gamepad_keydown_up);
        gamepad1.set_gamepad_keydown_down(this.gamepad_keydown_down);
        gamepad1.set_gamepad_keydown_x(this.gamepad_keydown_x);
        gamepad1.set_gamepad_keydown_b(this.gamepad_keydown_b);
        gamepad1.set_gamepad_keydown_a(this.gamepad_keydown_a);
        gamepad1.set_gamepad_keydown_y(this.gamepad_keydown_y);
        gamepad1.set_gamepad_keydown_select(this.gamepad_keydown_select);
        gamepad1.set_gamepad_keydown_start(this.gamepad_keydown_start);

        gamepad1.set_gamepad_Joystick_left(this.gamepad_keydown_left);
        gamepad1.set_gamepad_Joystick_right(this.gamepad_keydown_right);
        gamepad1.set_gamepad_Joystick_up(this.gamepad_keydown_up);
        gamepad1.set_gamepad_Joystick_down(this.gamepad_keydown_down);

        this.panel_menu.SetActive(true);
        this.panel_play.SetActive(false);
        this.panel_game_over.SetActive(false);
        this.bk.load_bk();
        this.score_high = PlayerPrefs.GetInt("score_high",0);
        this.txt_high_score_main.text = this.score_high.ToString();
        this.txt_coin_main.text = this.bk.get_your_coin().ToString();

        this.carrot.game.load_bk_music(this.sound[5]);

        this.carrot.game.set_list_button_gamepad_console(this.list_btn_main);
        this.pig2.gameObject.SetActive(false);
        this.shop.load_shop_game(this);
        this.pig.load(this);
        this.pig2.load(this);
        this.camSmoothCamera.set_cam_follow(true);
    }

    private void check_exit_app()
    {
        if (this.panel_play.activeInHierarchy)
        {
            this.btn_back_menu();
            this.carrot.set_no_check_exit_app();
        }else if (this.panel_game_over.activeInHierarchy)
        {
            this.btn_back_menu();
            this.carrot.set_no_check_exit_app();
        }
    }

    private void act_after_close_all_box()
    {
        this.carrot.game.set_list_button_gamepad_console(this.list_btn_main);
    }

    private void play_game()
    {
        this.is_play = true;
        this.show_ads_Interstitial();
        this.play_sound();
        this.panel_menu.SetActive(false);
        this.panel_play.SetActive(true);
        this.panel_game_over.SetActive(false);
        this.bk.set_status_play(true);
        this.bk.reset();
        this.shop.load_item_existence_in_game();
    }

    public void btn_play_one_player()
    {
        this.is_play_one = true;
        this.pig2.gameObject.SetActive(false);
        this.panel_play_one.SetActive(true);
        this.panel_play_two.SetActive(false);
        this.pig.Reset();
        this.pig.set_cam_follow(true);
        this.camSmoothCamera.set_model_one_play();
        this.camSmoothCamera.set_cam_follow(true);
        this.play_game();
    }

    public void btn_play_two_player()
    {
        this.is_play_one = false;
        this.pig2.gameObject.SetActive(true);
        this.panel_play_one.SetActive(false);
        this.panel_play_two.SetActive(true);
        this.pig.Reset();
        this.pig2.Reset();
        this.pig.set_cam_follow(false);
        this.pig2.set_cam_follow(false);
        this.camSmoothCamera.set_model_two_play();
        this.camSmoothCamera.set_cam_follow(false);
        this.play_game();
    }

    public void btn_play_agaim()
    {
        if (this.is_play_one)
            this.btn_play_one_player();
        else
            this.btn_play_two_player();
    }

    public void btn_back_menu()
    {
        this.is_play = false;
        this.show_ads_Interstitial();
        this.play_sound();
        this.panel_menu.SetActive(true);
        this.panel_play.SetActive(false);
        this.panel_game_over.SetActive(false);
        this.bk.set_status_play(false);
        this.bk.reset();
        this.pig.Reset();
        this.txt_coin_main.text = this.bk.get_your_coin().ToString();
        this.carrot.game.set_list_button_gamepad_console(this.list_btn_main);
    }

    public void btn_pig_left()
    {
        if (!this.is_play) return;
        this.pig.move_left();
        this.play_sound(3);
    }

    public void btn_pig_right()
    {
        if (!this.is_play) return;
        this.pig.move_right();
        this.play_sound(3);
    }

    public void btn_pig2_left()
    {
        if (!this.is_play_one)
        {
            if (!this.is_play) return;
            this.pig2.move_left();
            this.play_sound(3);
        }
    }

    public void btn_pig2_right()
    {
        if (!this.is_play_one)
        {
            if (!this.is_play) return;
            this.pig2.move_right();
            this.play_sound(3);
        }
    }

    public void play_sound(int index_sound=0)
    {
        if(this.carrot.get_status_sound()) this.sound[index_sound].Play();
    }

    public void random_weather()
    {
        for (int i = 0; i < this.effect_weather.Length; i++) this.effect_weather[i].SetActive(false);
        int random_weather = Random.Range(0, this.effect_weather.Length);
        this.effect_weather[random_weather].SetActive(true);
    }

    public void show_game_over()
    {
        int score = this.bk.get_your_score();
        this.is_play = false;
        this.GetComponent<History_Play>().add_history(score);
        if (score > this.score_high)
        {
            this.score_high = score; 
            PlayerPrefs.SetInt("score_high", this.score_high);
            this.txt_high_score_main.text = this.score_high.ToString();
        }
        this.carrot.game.update_scores_player(score);
        this.play_sound(2);
        this.panel_play.SetActive(false);
        this.panel_game_over.SetActive(true);
        this.bk.set_status_play(false);
        this.txt_over_high_score.text = this.score_high.ToString();
        this.txt_over_your_score.text = this.bk.get_your_score().ToString();
        this.carrot.game.set_list_button_gamepad_console(this.list_btn_gameover);

        this.panel_gameover_one_player.SetActive(false);
        this.panel_gameover_two_player.SetActive(false);
        if (this.is_play_one)
        {
            this.panel_gameover_one_player.SetActive(true);
        }
        else
        {
            this.txt_p1_one_gameover_score.text = this.pig.get_scores().ToString();
            this.txt_p1_one_gameover_hight_score.text = this.pig.get_highst_scores().ToString();
            this.txt_p2_one_gameover_score.text = this.pig2.get_scores().ToString();
            this.txt_p2_one_gameover_hight_score.text = this.pig2.get_highst_scores().ToString();
            this.panel_gameover_two_player.SetActive(true);
        }
    }

    public void show_game_setting()
    {
        this.carrot.Create_Setting();
    }

    public void act_remove_ads()
    {
        this.play_sound();
        this.carrot.buy_product(0);
    }

    public void btn_buy_coin()
    {
        this.play_sound();
        this.carrot.buy_product(3);
    }

    public void show_ads_Interstitial()
    {
        this.carrot.ads.show_ads_Interstitial();
    }

    public void carrot_by_success(string s_id_product)
    {

        if (s_id_product == this.carrot.shop.get_id_by_index(2))
        {
            this.shop.on_buy_success_item_existence();
        }

        if (s_id_product == this.carrot.shop.get_id_by_index(3))
        {
            this.bk.add_coin(1000);
            this.carrot.show_msg("Shop", "You have received 1000 more coins!!!",Msg_Icon.Success);
            this.txt_coin_main.text = this.bk.get_your_coin().ToString();
        }
    }


    public void act_vibrates()
    {
        carrot.play_vibrate();
    }

    public void btn_rate()
    {
        this.play_sound();
        this.carrot.show_rate();
    }

    public void gamepad_keydown_down()
    {
        this.carrot.game.gamepad_keydown_down_console();
    }

    public void gamepad_keydown_up()
    {
        this.carrot.game.gamepad_keydown_up_console();
    }

    public void gamepad_keydown_right()
    {
        this.btn_pig_right();
        this.carrot.game.gamepad_keydown_down_console();
    }

    public void gamepad_keydown_left()
    {
        this.btn_pig_left();
        this.carrot.game.gamepad_keydown_up_console();
    }

    public void gamepad_keydown_select()
    {
        this.carrot.game.gamepad_keydown_enter_console();
    }

    public void gamepad_keydown_start()
    {
        this.carrot.game.gamepad_keydown_enter_console();
    }

    public void gamepad_keydown_y()
    {

    }

    public void gamepad_keydown_x()
    {

    }

    public void gamepad_keydown_a()
    {

    }

    public void gamepad_keydown_b()
    {

    }

    public void btn_show_list_top_player()
    {
        this.carrot.game.Show_List_Top_player();
        this.play_sound();
    }

    public void btn_show_carrot_user()
    {
        this.carrot.show_login();
        this.play_sound();
    }

    public void btn_show_shop_game()
    {
        this.shop.show_shop();
        this.play_sound();
    }

    private void on_Rewarded_Success()
    {
        this.bk.add_coin(5);
        this.carrot.show_msg("Shop", "You got 5 extra coins from watching ads",Msg_Icon.Success);
        this.txt_coin_main.text = this.bk.get_your_coin().ToString();
    }

}
