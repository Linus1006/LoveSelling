using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;

namespace LoveSelling.Models
{
    public enum ProductType
    {
        Fine = 1,       //皮件/皮包/各式精品類 
        Toy = 2,        //嬰/幼兒衣物、玩具及相關物品
        Electronic = 3, //3C家電用品類
        Book = 4,       //書籍及視聽用品類
        Life = 5,        //居家生活及休閒用品類
        Bid = 6         //競標
    }
}