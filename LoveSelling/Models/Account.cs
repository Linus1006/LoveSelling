using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace LoveSelling.Models
{
    /// <summary>
    /// 
    /// </summary>
    public class Account
    {
        private string _employeeID;

        [DisplayName("員工編號")]
        [Required(ErrorMessage = "員工編號為必填欄位")]
        //[StringLength(5, ErrorMessage = "員工編號長度有誤")]
        [RegularExpression(@"\d{5}", ErrorMessage = "員工編號格式有誤")]
        public string EmployeeID {
            get { return _employeeID; }
            set { _employeeID = value?.PadLeft(5, '0'); }
        }

        /// <summary>
        /// 
        /// </summary>
        [DisplayName("身分證號碼")]
        [Required(ErrorMessage = "身分證號碼為必填欄位")]
        [RegularExpression(@"[A-Z,a-z]+\d{9}",ErrorMessage = "身分證號碼格式有誤")]
        public string ID { get; set; }

        [DisplayName("姓名")]
        public string Name { get; set; }

        [DisplayName("登陸狀態")]
        public bool? IsLogon { get; set; }

        [DisplayName("權限等級")]
        public int? Rank { get; set; }

        [DisplayName("最後登陸時間")]
        public DateTime? LastLogonTime { get; set; }

        [DisplayName("最後狀態")]
        public SignInStatus? SignInStatus { get; set; }

    }
}