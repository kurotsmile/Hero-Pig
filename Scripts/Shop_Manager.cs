using Carrot;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Shop_Manager : MonoBehaviour
{
    public Sprite icon_shop;
    public Sprite icon_shop_buy;
    public Sprite icon_shop_coin;
    public Sprite icon_shop_gift;
    public Sprite[] icon_item;
    public string[] s_name_item;
    public string[] s_tip_item;
    public int[] type_item;
    public int[] price_item;
    public GameObject item_existence_prefab;
    public GameObject btn_buy_coin_prefab;
    public Transform area_all_item_existence;

    private List<int> list_item_storehouse_icon;
    private List<int> list_item_storehouse_type;
    private Carrot_Box_Item[] box_shop_item;

    private int sel_index_icon_temp = -1;
    private int sel_index_type_temp = -1;

    private Game game;
    public void load_shop_game(Game game)
    {
        this.game = game;
        this.list_item_storehouse_icon = new List<int>();
        this.list_item_storehouse_type = new List<int>();
    }

    public void show_shop()
    {
        Carrot_Box box_shop = this.GetComponent<Game>().carrot.Create_Box();
        box_shop.set_icon(this.icon_shop);
        box_shop.set_title(PlayerPrefs.GetString("shop","Shop"));
        box_shop_item = new Carrot_Box_Item[this.icon_item.Length];
        for (int i = 0; i < this.icon_item.Length; i++)
        {
            var index_type = this.type_item[i];
            var index_icon = i;
            this.box_shop_item[i] = box_shop.create_item();
            this.box_shop_item[i].set_icon_white(this.icon_item[i]);
            this.box_shop_item[i].set_title(this.s_name_item[i]);
            this.box_shop_item[i].set_tip(this.s_tip_item[i]);
            this.box_shop_item[i].set_act(() =>this.buy_item_existence(index_icon, index_type));

            GameObject btn_coin = this.box_shop_item[i].add_item_extension(this.btn_buy_coin_prefab);
            btn_coin.GetComponent<Item_btn_shop_buy_coin>().txt_text.text = this.price_item[i].ToString();
            btn_coin.GetComponent<Button>().onClick.RemoveAllListeners();
            btn_coin.GetComponent<Button>().onClick.AddListener(() => buy_item_by_coin( index_icon, index_type));

            Carrot_Box_Btn_Item btn_buy = this.box_shop_item[i].create_item();
            btn_buy.icon.sprite = this.icon_shop_buy;
            btn_buy.GetComponent<Image>().color=this.GetComponent<Game>().carrot.color_highlight;
            Destroy(btn_buy.GetComponent<Button>());
        }

        if (PlayerPrefs.GetInt("is_ads", 0) == 0)
        {
            Carrot_Box_Item setting_item_removeads = box_shop.create_item();
            setting_item_removeads.set_icon(this.game.carrot.sp_icon_removeads);
            setting_item_removeads.set_title("Remove Ads");
            setting_item_removeads.set_tip("Not show ads in game");
            setting_item_removeads.set_act(this.game.act_remove_ads);

            Carrot_Box_Btn_Item btn_buy_ads = setting_item_removeads.create_item();
            btn_buy_ads.icon.sprite = this.icon_shop_buy;
            btn_buy_ads.GetComponent<Image>().color = this.GetComponent<Game>().carrot.color_highlight;
            Destroy(btn_buy_ads.GetComponent<Button>());
        }

        Carrot_Box_Item shop_item_coin = box_shop.create_item();
        shop_item_coin.set_icon_white(this.icon_shop_coin);
        shop_item_coin.set_title("Buy game coins");
        shop_item_coin.set_tip("You will be added 1000 coins to shop freely");
        shop_item_coin.set_act(this.game.btn_buy_coin);

        Carrot_Box_Btn_Item btn_buy_coin = shop_item_coin.create_item();
        btn_buy_coin.icon.sprite = this.icon_shop_buy;
        btn_buy_coin.GetComponent<Image>().color = this.GetComponent<Game>().carrot.color_highlight;
        Destroy(btn_buy_coin.GetComponent<Button>());


        Carrot_Box_Item shop_item_gift_ads = box_shop.create_item();
        shop_item_gift_ads.set_icon(this.icon_shop_gift);
        shop_item_gift_ads.set_title("Watch ads to get rewards");
        shop_item_gift_ads.set_tip("Finish watching ads to get +5 coins");
        shop_item_gift_ads.set_act(this.view_ads_rewards);
        box_shop.update_gamepad_cosonle_control();
    }

    public void buy_item_by_coin(int index_icon, int index_type_food)
    {
        int price= price_item[index_icon];
        if (price <= this.game.bk.get_your_coin())
        {
            this.sel_index_icon_temp = index_icon;
            this.sel_index_type_temp = index_type_food;
            this.on_buy_success_item_existence();
            this.game.bk.subtraction_coin(price);
        }
        else
        {
            this.game.carrot.show_msg("Shop","You don't have enough coins, Let's play games to earn coins",Msg_Icon.Error);
        }
    }

    public void buy_item_existence(int index_icon, int index_type_food)
    {
        this.sel_index_icon_temp = index_icon;
        this.sel_index_type_temp = index_type_food;
        this.game.carrot.shop.buy_product(2);
    }

    public void on_buy_success_item_existence()
    {
        this.game.carrot.show_msg("Shop", "Successful item purchase!",Msg_Icon.Success);
        this.list_item_storehouse_icon.Add(sel_index_icon_temp);
        this.list_item_storehouse_type.Add(sel_index_type_temp);
    }

    public void load_item_existence_in_game()
    {
        if(this.list_item_storehouse_type.Count>0) this.GetComponent<Game>().carrot.clear_contain(this.area_all_item_existence);

        for(int i = 0; i < this.list_item_storehouse_type.Count; i++)
        {
            GameObject item_existence = Instantiate(this.item_existence_prefab);
            item_existence.transform.SetParent(this.area_all_item_existence);
            item_existence.transform.localScale = new Vector3(1f, 1f, 1f);
            int index_icon= this.list_item_storehouse_icon[i];
            item_existence.GetComponent<Item_existence>().icon.sprite = this.icon_item[index_icon];
            item_existence.GetComponent<Item_existence>().type_food = this.list_item_storehouse_type[i];
            item_existence.GetComponent<Item_existence>().index = i;
        }
    }

    public void remove_item_existence(int index)
    {
        this.list_item_storehouse_type.RemoveAt(index);
        this.list_item_storehouse_icon.RemoveAt(index);
        if (this.list_item_storehouse_icon.Count == 0)
        {
            this.GetComponent<Game>().carrot.clear_contain(this.area_all_item_existence);
            this.list_item_storehouse_type = new List<int>();
            this.list_item_storehouse_icon = new List<int>();
        }
        this.load_item_existence_in_game();
    }

    public void view_ads_rewards()
    {
        this.game.carrot.ads.show_ads_Rewarded();
    }
}
