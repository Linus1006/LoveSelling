using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using LoveSelling.Models;
using Dapper;
using System.Transactions;

namespace LoveSelling.Service
{
    public static class AccountHelper
    {
        /// <summary>
        /// 取的資料庫登錄人數(已改用Application level)
        /// </summary>
        /// <returns>人數</returns>
        public static int GetLogonNumber()
        {
            using (var conn = DbHelper.OpenConnection())
            {
                return conn.Query<int>(@"SELECT COUNT(*) FROM [HeartDrop2017].[dbo].[Account] WHERE IsLogon = 1").FirstOrDefault();

            }
        }

        /// <summary>
        /// 登錄作業(含資料庫更新)
        /// </summary>
        /// <param name="account">準備登錄的資料</param>
        /// <returns>帳號資訊</returns>
        public static Account Logon(Account account)
        {

            using (var conn = DbHelper.OpenConnection())
            {
                var logonInfo = conn.Query<Account>(@"SELECT * FROM [HeartDrop2017].[dbo].[Account] WHERE EmployeeID = @EmployeeID", account).FirstOrDefault();
                if (logonInfo == null)
                {
                    logonInfo = account;
                    logonInfo.SignInStatus = SignInStatus.Failure;
                }
                else
                {
                    if (logonInfo.ID != account.ID)
                    {
                        logonInfo.SignInStatus = SignInStatus.RequiresVerification;
                    }
                    else
                    {
                        logonInfo.SignInStatus = SignInStatus.Success;
                        logonInfo.IsLogon = true;
                    }
                }
                logonInfo.LastLogonTime = DateTime.Now;
                //更新登陸狀態
                updateLogonInfo(logonInfo);
                return logonInfo;
            }
        }

        /// <summary>
        /// 更新登陸紀錄
        /// </summary>
        /// <param name="account"></param>
        /// <returns>更新資料數量</returns>
        private static int updateLogonInfo(Account account)
        {

            using (var scope = new TransactionScope())
            using (var conn = DbHelper.OpenConnection())
            {
                //Log
                var logNum = conn.Execute(@"INSERT INTO [dbo].[AccountLog]([EmployeeID],[SignInStatus],[LastLogonTime]) VALUES(@EmployeeID, @SignInStatus, @LastLogonTime)", account);
                //update LogonInfo
                if (account.SignInStatus != SignInStatus.Failure)
                {
                    logNum += conn.Execute(@"UPDATE [HeartDrop2017].[dbo].[Account] SET [IsLogon] = @IsLogon ,[LastLogonTime] = @LastLogonTime WHERE [EmployeeID] = @EmployeeID", account);
                }
                scope.Complete();
                return logNum;
            }
        }

        /// <summary>
        /// 更新登陸紀錄(登出)
        /// </summary>
        /// <param name="employeeID"></param>
        /// <returns></returns>
        public static int Logout(string employeeID)
        {
            var account = new Account() {EmployeeID = employeeID,SignInStatus = SignInStatus.Logout, LastLogonTime = DateTime.Now, IsLogon = false };
            using (var scope = new TransactionScope())
            using (var conn = DbHelper.OpenConnection())
            {
                //Log
                var logNum = conn.Execute(@"INSERT INTO [dbo].[AccountLog]([EmployeeID],[SignInStatus],[LastLogonTime]) VALUES(@EmployeeID, @SignInStatus, @LastLogonTime)", account);
                //update LogonInfo
                logNum += conn.Execute(@"UPDATE [HeartDrop2017].[dbo].[Account] SET [IsLogon] = @IsLogon WHERE [EmployeeID] = @EmployeeID", account);
                scope.Complete();
                return logNum;
            }
        }

    }
}