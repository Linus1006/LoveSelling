using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace LoveSelling.Models
{
    public class Product
    {
        private string _staff;

        [DisplayName("產品編號")]
        [Required(ErrorMessage = "產品編號為必填欄位")]
        [RegularExpression(@"\d{1}-\d{4}", ErrorMessage = "產品編號格式有誤")]
        public string ID { get; set; }

        [DisplayName("單位")]
        [Required(ErrorMessage = "單位為必填欄位")]
        public string Unit { get; set; }

        [DisplayName("類別")]
        [Required(ErrorMessage = "類別為必填欄位")]
        public ProductType Type { get; set; }

        [DisplayName("品項")]
        [Required(ErrorMessage = "品項為必填欄位")]
        public string Item { get; set; }

        [DisplayName("提供同仁員編")]
        [Required(ErrorMessage = "員工編號為必填欄位")]
        [RegularExpression(@"\d{1,5}", ErrorMessage = "員工編號格式有誤")]
        public string Staff {
            get { return _staff; }
            set { _staff = value.PadLeft(5, '0'); }
        }

        [DisplayName("金額"), DisplayFormat(DataFormatString = "{0:G} ")]
        [Required(ErrorMessage = "金額為必填欄位")]
        [RegularExpression(@"\d+", ErrorMessage = "金額格式有誤")]
        public Decimal Amount { get; set; }

        [DisplayName("賣出")]
        public bool? isSell { get; set; }

    }
}