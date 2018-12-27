using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using LoveSelling.Models;
using Dapper;
using System.Transactions;

namespace LoveSelling.Service
{
    public class ProductHelper
    {
        /// <summary>
        /// 讀取商品資料
        /// </summary>
        /// <param name="id">產品編號</param>
        /// <returns>產品清單的List</returns>
        public static List<Product> Load(string id = null)
        {
            var sqlStr = @"SELECT * FROM [HeartDrop2017].[dbo].[HeartProduct] ";
            if (id != null) sqlStr += "WHERE ID = @id";
            using (var conn = DbHelper.OpenConnection())
            {
                return conn.Query<Product>(sqlStr, new { id }).OrderBy(m => m.ID).ToList();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="keyword"></param>
        /// <returns></returns>
        public static List<Product> Find(string keyword = null)
        {
            var sqlStr = @"SELECT * from [HeartDrop2017].[dbo].[HeartProduct]";
            if (!string.IsNullOrWhiteSpace(keyword))
                sqlStr += @" where [ID] like @keyword or [Unit] like @keyword or [Item] like @keyword or [Staff] like @keyword";

            using (var conn = DbHelper.OpenConnection())
            {
                return conn.Query<Product>(sqlStr, new { keyword = $@"%{keyword}%" }).ToList();
            }

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="product"></param>
        /// <returns></returns>
        public static int AddProduct(Product product, string employeeID = "11964")
        {
            product.isSell = false;
            using (var scope = new TransactionScope())
            using (var conn = DbHelper.OpenConnection())
            {
                var result = conn.Execute(@"INSERT INTO [dbo].[HeartProduct] ([ID],[Unit],[Type],[Item],[Staff],[Amount],[isSell])
                                                                VALUES (@ID ,@Unit ,@Type ,@Item ,@Staff ,@Amount ,@isSell)", product);
                result += conn.Execute(@"INSERT INTO [dbo].[TransactionLog] ([EmployeeID] ,[Action] ,[CreateTime] ,[ProductID]) VALUES(@employeeID, @action ,@createTime ,@id)",
                                    new { employeeID = employeeID, action = TansactionAction.Create, createTime = DateTime.Now, id = product.ID });
                scope.Complete();
                return result;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="product"></param>
        /// <returns></returns>
        public static int UpdateProduct(Product product, string employeeID = "11964")
        {
            using (var scope = new TransactionScope())
            using (var conn = DbHelper.OpenConnection())
            {
                var result = conn.Execute(@"UPDATE [dbo].[HeartProduct]  SET [Unit] = @Unit ,[Type] = @Type ,[Item] = @Item ,[Staff] = @Staff, [Amount] = @Amount ,[isSell] = @isSell WHERE [ID] = @ID ", product);
                result += conn.Execute(@"INSERT INTO [dbo].[TransactionLog] ([EmployeeID] ,[Action] ,[CreateTime] ,[ProductID]) VALUES(@employeeID, @action ,@createTime ,@id)",
                                    new { employeeID = employeeID, action = TansactionAction.Update, createTime = DateTime.Now, id = product.ID });
                scope.Complete();
                return result;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static int DeleteProduct(string id, string employeeID = "11964")
        {
            using (var scope = new TransactionScope())
            using (var conn = DbHelper.OpenConnection())
            {
                var result = conn.Execute(@"DELETE FROM [dbo].[HeartProduct] WHERE ID = @id", new { id });

                result += conn.Execute(@"INSERT INTO [dbo].[TransactionLog] ([EmployeeID] ,[Action] ,[CreateTime] ,[ProductID]) VALUES(@employeeID, @action ,@createTime ,@id)"
                                    , new { employeeID = employeeID, action = TansactionAction.Delete, createTime = DateTime.Now, id = id });
                scope.Complete();
                return result;
            }
        }

        /// <summary>
        /// 進行交易
        /// </summary>
        /// <returns></returns>
        public static int Transaction(Product product, string employeeID)
        {

            if (employeeID == null) employeeID = "11964";
            using (var scope = new TransactionScope())
            using (var conn = DbHelper.OpenConnection())
            {
                var result = conn.Execute(@"UPDATE [dbo].[HeartProduct]  SET [isSell] = @isSell WHERE [ID] = @id ", new { isSell = true, id = product.ID });
                //Log
                result += conn.Execute(@"INSERT INTO [dbo].[TransactionLog] ([EmployeeID] ,[Action] ,[CreateTime] ,[ProductID]) VALUES(@employeeID ,@action ,@createTime ,@id)"
                                    , new { employeeID = employeeID, action = TansactionAction.Sell, createTime = DateTime.Now, id = product.ID });
                scope.Complete();
                return result;
            }

        }


        /// <summary>
        /// 獲得特定地點販賣的總金額
        /// </summary>
        /// <param name="place">地點 Place</param>
        /// <returns></returns>
        public static Decimal GetTotalAmount(Place? place = null)
        {
            var sqlAmount = @"SELECT SUM(Amount) FROM [HeartDrop2017].[dbo].[HeartProduct] WHERE isSell = 1 ";
            switch (place)
            {
                case Place.C004:
                case Place.C105:
                case Place.C810:
                    sqlAmount += $@" AND [ID] LIKE '{(int)place}-%'";
                    break;
                default:
                    break;
            }


            using (var conn = DbHelper.OpenConnection())
            {
                return conn.Query<Decimal?>(sqlAmount).FirstOrDefault() ?? 0;
            }
        }

        public static Dictionary<string, string> GetUnits()
        {
            Dictionary<string, string> unitItems = new Dictionary<string, string>();
            var sqlStr = @"SELECT [Unit],[Name] FROM [dbo].[Unit] ORDER BY [Unit] ASC";
            using (var conn = DbHelper.OpenConnection())
            {
                conn.Query(sqlStr).ForEach(n =>
                {
                    unitItems.Add(
                        key: n.Unit,
                        value: n.Name
                        );
                });
            }

            return unitItems;
        }

    }
}