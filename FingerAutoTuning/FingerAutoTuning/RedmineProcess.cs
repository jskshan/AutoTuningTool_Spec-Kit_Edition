using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Redmine.Net.Api;
using Redmine.Net.Api.Types;
using System.Collections.Specialized;
//using System.Net.Http;
//using System.Net.Http.Headers;
using System.Net;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace FingerAutoTuning
{
    public class RedmineProcess
    {
        private string m_sRedmineUrl = "http://192.168.55.153/redmine"; // Redmine 伺服器地址

        private int m_nLoginType = 0;

        private string m_sUserName = "";
        private string m_sPassword = "";

        private string m_sAPIKey = "";  // API Key Token

        private string m_sServerUrl = "http://192.168.55.214:5010";     //"http://192.168.56.51:5010";    //"http://192.168.55.214:5010";
        // private string m_sTestServerUrl = "http://192.168.56.23:5000";

        private string m_sCreateIssueRoute = @"api/create_issue";

        private string m_sProjectID = "auto-tuning-service";
        private int m_nIssueID = 42924;

        private int m_nTrackerID = 3;   // 可選:追蹤標籤 ID (1:臭蟲  2:臭蟲  3:支援  4:臭蟲  5:會議紀錄  6:臭蟲  7:臭蟲  8:臭蟲  9:臭蟲  10:臭蟲  11:臭蟲  12:臭蟲)
        private int m_nPriorifyID = 2;   // 可選:優先級 ID (1:低  2:正常  3:高  4:速  5:急)

        private frmRedmineTask m_cfrmRedmineTask = null;

        public bool m_bErrorFlag = false;

        public List<frmRedmineTask.FileInforamtion> m_cUploadFileInforamtion_List = null;

        public RedmineProcess(frmRedmineTask cfrmRedmineTask)
        {
            m_cfrmRedmineTask = cfrmRedmineTask;
        }

        public void SetLoginType(int nLoginType)
        {
            m_nLoginType = nLoginType;
        }

        public void SetUserNameAndPassword(string sUserName, string sPassword)
        {
            m_sUserName = sUserName;
            m_sPassword = sPassword;
        }

        public void SetAPIKey(string sAPIKey)
        {
            m_sAPIKey = sAPIKey;
        }

        public void ResetErrorFlag()
        {
            m_bErrorFlag = false;
        }

        public void SetUploadFileInformationList(List<frmRedmineTask.FileInforamtion> cUploadFileInforamtion_List)
        {
            m_cUploadFileInforamtion_List = new List<frmRedmineTask.FileInforamtion>();
            m_cUploadFileInforamtion_List = cUploadFileInforamtion_List;
        }

        public void ConnectRedmineByRedmineNetApi()
        {
            if (m_bErrorFlag == true)
                return;

            OutputMessage("Test : Use Redmine.Net.Api to Connect Redmine");

            // 初始化 Redmine Manager
            var vManager = new RedmineManager(m_sRedmineUrl, m_sAPIKey);

            try
            {
                if (m_nLoginType == 0)
                {
                    vManager = null;

                    // 初始化 Redmine Manager
                    vManager = new RedmineManager(m_sRedmineUrl, m_sUserName, m_sPassword, MimeFormat.json);
                }
                else if (m_nLoginType == 1)
                {
                    vManager = null;

                    // 初始化 Redmine Manager
                    vManager = new RedmineManager(m_sRedmineUrl, m_sAPIKey);
                }

                // 獲取使用者名稱
                OutputMessage("");
                OutputMessage("Test : Connect Redmine and Get UserName");

                var vCurrentUser = vManager.GetObject<User>("current", null);
                string sUserName = string.Format("UserName : {0} {1}, EmployeeID : {2}", vCurrentUser.LastName, vCurrentUser.FirstName, vCurrentUser.Login);
                Console.WriteLine(sUserName);
                OutputMessage(string.Format(sUserName));
            }
            catch (Exception ex)
            {
                string sErrorMessage = string.Format("Error : {0}", ex.Message);
                Console.WriteLine(sErrorMessage);
                OutputMessage(string.Format(sErrorMessage));
                OutputMessage("");

                return;
            }

            OutputMessage("");
        }

        public void GetIssueJournalsByRedmineNetApi()
        {
            if (m_bErrorFlag == true)
                return;

            OutputMessage("Test : Use Redmine.Net.Api to Connect Redmine");

            // 初始化 Redmine Manager
            var vManager = new RedmineManager(m_sRedmineUrl, m_sAPIKey);

            // 獲取所有專案
            OutputMessage("");
            OutputMessage(string.Format("Test : Read All Issue in Project : {0}", m_sProjectID));

            try
            {
                if (m_nLoginType == 0)
                {
                    vManager = null;

                    // 初始化 Redmine Manager
                    vManager = new RedmineManager(m_sRedmineUrl, m_sUserName, m_sPassword, MimeFormat.json);
                }
                else if (m_nLoginType == 1)
                {
                    vManager = null;

                    // 初始化 Redmine Manager
                    vManager = new RedmineManager(m_sRedmineUrl, m_sAPIKey);
                }

                // 設定篩選條件：只獲取指派給當前使用者的 Issue
                NameValueCollection cParameter = new NameValueCollection
                {
                    //{ "assigned_to_id", "me" }, // 只獲取指派給自己的 Issue
                    //{ "status_id", "2" },
                    { "project_id", m_sProjectID }
                };

                // 獲取 Issue 列表
                var vIssue_List = vManager.GetTotalObjectList<Issue>(cParameter);

                // 輸出 Issue 資訊
                foreach (var vIssue in vIssue_List)
                {
                    string sIssueName = string.Format("Issue ID : {0}, Subject : {1}, Status : {2}, Author : {3}, AssignedTo : {4}", vIssue.Id, vIssue.Subject, vIssue.Status.Name, vIssue.Author, vIssue.AssignedTo);
                    Console.WriteLine(sIssueName);
                    OutputMessage(string.Format(sIssueName));
                }
            }
            catch (Exception ex)
            {
                string sErrorMessage = string.Format("Error Message : {0}", ex.Message);
                OutputMessage(string.Format(sErrorMessage));
                OutputMessage("");

                return;
            }

            OutputMessage("");
            OutputMessage(string.Format("Test : Read Issue : {0} Include Children/Journals/Watchers and Update Journal", m_nIssueID));

            try
            {
                // 創建 NameValueCollection 並加載 "journals" 屬性
                var vParameter = new NameValueCollection
                {
                    { "include", "children,journals,watchers" } // 包含 journals 屬性
                };

                var vIssue = vManager.GetObject<Issue>(m_nIssueID.ToString(), vParameter);
                string sIssueMessage = string.Format("Issue ID : {0}, Subject : {1}", vIssue.Id, vIssue.Subject);
                Console.WriteLine(sIssueMessage);
                OutputMessage(string.Format(sIssueMessage));

                // 顯示子任務（Children）
                if (vIssue.Children != null && vIssue.Children.Count > 0)
                {
                    Console.WriteLine("Child Issue : ");
                    OutputMessage("Child Issue : ");
                    foreach (var child in vIssue.Children)
                    {
                        Console.WriteLine(string.Format("  - Child Issue ID : {0}, Subject : {1}", child.Id, child.Subject));
                        OutputMessage(string.Format("  - Child Issue ID : {0}, Subject : {1}", child.Id, child.Subject));
                    }
                }

                // 顯示歷史紀錄（Journals）
                if (vIssue.Journals != null && vIssue.Journals.Count > 0)
                {
                    Console.WriteLine("Journals : ");
                    OutputMessage("Journals : ");
                    foreach (var journal in vIssue.Journals)
                    {
                        Console.WriteLine(string.Format("  - Time : {0}, User : {1}", journal.CreatedOn, journal.User.Name));
                        Console.WriteLine(string.Format("    Note : {0}", journal.Notes));
                        OutputMessage(string.Format("  - Time : {0}, User : {1}", journal.CreatedOn, journal.User.Name));
                        OutputMessage(string.Format("    Note : {0}", journal.Notes));
                    }
                }

                // 顯示監視者（Watchers）
                /*
                if (vIssue.Watchers != null && vIssue.Watchers.Count > 0)
                {
                    Console.WriteLine("Watchers : ");
                    OutputMessage("Watchers : ");
                    foreach (var watcher in vIssue.Watchers)
                    {
                        Console.WriteLine(string.Format("  - Watcher ID : {0}, Name : {0}", watcher.Id, watcher.Name));
                        OutputMessage(string.Format("  - Watcher ID : {0}, Name : {0}", watcher.Id, watcher.Name));
                    }
                }
                */

                // 顯示 Issue 的歷史紀錄（Journals）
                /*
                foreach (var journal in vIssue.Journals)
                {
                    Console.WriteLine(string.Format("Journal Create Time : {0}", journal.CreatedOn));
                    Console.WriteLine(string.Format("User : {0}", journal.User.Name));
                    OutputMessage(string.Format("Journal Create Time : {0}", journal.CreatedOn));
                    OutputMessage(string.Format("User : {0}", journal.User.Name));

                    // 顯示備註內容
                    if (!string.IsNullOrEmpty(journal.Notes))
                    {
                        Console.WriteLine(string.Format("Note : {0}", journal.Notes));
                        OutputMessage(string.Format("Note : {0}", journal.Notes));
                    }
                }
                */

                /*
                // 準備備註
                var vUpdateIssue = new Issue
                {
                    Notes = "This is New Note"
                };

                // 提交更新
                vManager.UpdateObject(m_nIssueID.ToString(), vUpdateIssue);

                string sUpdateMessage = "Issue Update Success";

                Console.WriteLine(sUpdateMessage);
                OutputMessage(sUpdateMessage);
                */
            }
            catch (RedmineException ex)
            {
                string sErrorMessage = string.Format("Redmine API Error : {0}", ex.Message);
                Console.WriteLine(sErrorMessage);
                OutputMessage(sErrorMessage);
                OutputMessage("");
                m_bErrorFlag = true;

                return;
            }
            catch (Exception ex)
            {
                string sErrorMessage = string.Format("Other Error : {0}", ex.Message);
                Console.WriteLine(sErrorMessage);
                OutputMessage(sErrorMessage);
                OutputMessage("");
                m_bErrorFlag = true;

                return;
            }

            OutputMessage("");
            OutputMessage(string.Format("Test : Read Journals in Issue : {0}", m_nIssueID));

            try
            {
                // 創建查詢參數，包含需要加載的屬性
                var parameters = new NameValueCollection
                {
                    { "include", "journals" } // 加載 journals
                };

                // 查詢 Issue 並包含 Journals
                var issue = vManager.GetObject<Issue>(m_nIssueID.ToString(), parameters);

                // 顯示 Journals
                if (issue.Journals != null && issue.Journals.Count > 0)
                {
                    Console.WriteLine("Journals : ");
                    OutputMessage("Journals : ");
                    foreach (var journal in issue.Journals)
                    {
                        Console.WriteLine(string.Format("- Time : {0}, User : {1}", journal.CreatedOn, journal.User.Name));
                        Console.WriteLine(string.Format("  Note : {0}", journal.Notes));
                        OutputMessage(string.Format("- Time : {0}, User : {1}", journal.CreatedOn, journal.User.Name));
                        OutputMessage(string.Format("  Note : {0}", journal.Notes));
                    }
                }
                else
                {
                    Console.WriteLine("No Journals");
                    OutputMessage("No Journals");
                }
            }
            catch (RedmineException ex)
            {
                Console.WriteLine(string.Format("Redmine API Error : {0}", ex.Message));
                OutputMessage(string.Format("Redmine API Error : {0}", ex.Message));
                OutputMessage("");
                m_bErrorFlag = true;

                return;
            }
            catch (Exception ex)
            {
                Console.WriteLine(string.Format("Other Error : {0}", ex.Message));
                OutputMessage(string.Format("Other Error : {0}", ex.Message));
                OutputMessage("");
                m_bErrorFlag = true;

                return;
            }

            OutputMessage("");
        }

        public void CreateRedmineIssueByRedmineNetApi(string sSubject, string sDescription)
        {
            // 初始化 Redmine Manager
            var vManager = new RedmineManager(m_sRedmineUrl, m_sAPIKey);

            OutputMessage("");
            OutputMessage(string.Format("Test : Create Issue in Project : {0}", m_sProjectID));

            try
            {
                if (m_nLoginType == 0)
                {
                    vManager = null;

                    // 初始化 Redmine Manager
                    vManager = new RedmineManager(m_sRedmineUrl, m_sUserName, m_sPassword, MimeFormat.json);
                }
                else if (m_nLoginType == 1)
                {
                    vManager = null;

                    // 初始化 Redmine Manager
                    vManager = new RedmineManager(m_sRedmineUrl, m_sAPIKey);
                }

                // 創建新的 Issue
                var vNewIssue = new Issue
                {
                    Project = new IdentifiableName { Name = m_sProjectID }, // 專案名稱
                    Subject = sSubject,      // 必須：Issue 標題
                    Description = sDescription,  // 可選：Issue 描述
                    //Priority = new IdentifiableName { Id = 4 }, // 可選：優先級 ID (例如 4 表示普通)
                    //Tracker = new IdentifiableName { Id = 1 }  // 可選：跟蹤器類型 ID (1 是缺陷)
                };

                // 呼叫 API 創建 Issue
                var vCreatedIssue = vManager.CreateObject(vNewIssue);

                string sCreateMessage = string.Format("Create Issue Success! ID : {0}, Subject : {1}", vCreatedIssue.Id, vCreatedIssue.Subject);

                // 顯示新增 Issue 的資訊
                Console.WriteLine(sCreateMessage);
                OutputMessage(string.Format(sCreateMessage));
            }
            catch (RedmineException ex)
            {
                string sErrorMessage = string.Format("Redmine API Error : {0}", ex.Message);
                Console.WriteLine(sErrorMessage);
                OutputMessage(string.Format(sErrorMessage));
                OutputMessage("");
                m_bErrorFlag = true;

                return;
            }
            catch (Exception ex)
            {
                string sErrorMessage = string.Format("Other Error : {0}", ex.Message);
                Console.WriteLine(sErrorMessage);
                OutputMessage(string.Format(sErrorMessage));
                OutputMessage("");
                m_bErrorFlag = true;

                return;
            }

            OutputMessage("");
            OutputMessage(string.Format("Test : Create Issue in Project : {0}", m_sProjectID));

            try
            {
                if (m_nLoginType == 0)
                {
                    // 初始化 Redmine Manager
                    vManager = new RedmineManager(m_sRedmineUrl, m_sUserName, m_sPassword, MimeFormat.json);
                }
                else if (m_nLoginType == 1)
                {
                    // 初始化 Redmine Manager
                    vManager = new RedmineManager(m_sRedmineUrl, m_sAPIKey);
                }

                // 創建新的 Issue
                var vNewIssue = new Issue
                {
                    Project = new IdentifiableName { Name = m_sProjectID }, // 專案名稱
                    Subject = sSubject,      // 必須：Issue 標題
                    Description = sDescription,  // 可選：Issue 描述
                    //Priority = new IdentifiableName { Id = 4 }, // 可選：優先級 ID (例如 4 表示普通)
                    //Tracker = new IdentifiableName { Id = 1 }  // 可選：跟蹤器類型 ID (1 是缺陷)
                };

                // 呼叫 API 創建 Issue
                var vCreatedIssue = vManager.CreateObject(vNewIssue);

                string sCreateMessage = string.Format("Create Issue Success! ID : {0}, Subject : {1}", vCreatedIssue.Id, vCreatedIssue.Subject);

                // 顯示新增 Issue 的資訊
                Console.WriteLine(sCreateMessage);
                OutputMessage(string.Format(sCreateMessage));
            }
            catch (RedmineException ex)
            {
                string sErrorMessage = string.Format("Redmine API Error : {0}", ex.Message);
                Console.WriteLine(sErrorMessage);
                OutputMessage(string.Format(sErrorMessage));
                OutputMessage("");
                m_bErrorFlag = true;

                return;
            }
            catch (Exception ex)
            {
                string sErrorMessage = string.Format("Other Error : {0}", ex.Message);
                Console.WriteLine(sErrorMessage);
                OutputMessage(string.Format(sErrorMessage));
                OutputMessage("");
                m_bErrorFlag = true;

                return;
            }

            OutputMessage("");
        }

        public void ConnectRedmineByNet()
        {
            if (m_bErrorFlag == true)
                return;

            OutputMessage("Test : Use System.Net to Connect Redmine");

            // 獲取使用者名稱
            OutputMessage("");
            OutputMessage("Test : Connect Redmine and Get UserName");

            try
            {
                // 建立 WebClient
                using (var client = new WebClient())
                {
                    // 設定編碼和標頭
                    client.Encoding = Encoding.UTF8;
                    client.Headers[HttpRequestHeader.ContentType] = "application/json";

                    if (m_nLoginType == 0)
                    {
                        // 設定基本驗證
                        string sAuth = Convert.ToBase64String(Encoding.UTF8.GetBytes(string.Format("{0}:{1}", m_sUserName, m_sPassword)));
                        client.Headers[HttpRequestHeader.Authorization] = string.Format("Basic {0}", sAuth);
                    }
                    else if (m_nLoginType == 1)
                    {
                        client.Headers.Add("X-Redmine-API-Key", m_sAPIKey);
                    }

                    string sJsonResponse = client.DownloadString(string.Format("{0}/users/current.json", m_sRedmineUrl));

                    // 解析 JSON
                    JObject cCurrentUser = JObject.Parse(sJsonResponse);

                    // 取得使用者資訊
                    string sLogin = (string)cCurrentUser["user"]["login"];
                    string sFirstName = (string)cCurrentUser["user"]["firstname"];
                    string sLastName = (string)cCurrentUser["user"]["lastname"];

                    string sUserName = string.Format("UserName : {0} {1}, EmployeeID : {2}", sLastName, sFirstName, sLogin);
                    Console.WriteLine(sUserName);
                    OutputMessage(string.Format(sUserName));
                }
            }
            catch (WebException ex)
            {
                // 處理 WebException
                if (ex.Response is HttpWebResponse)
                {
                    OutputMessage(string.Format("HTTP Error : {0}", ((HttpWebResponse)ex.Response).StatusCode));

                    // 讀取錯誤回應
                    using (var vReader = new System.IO.StreamReader(((HttpWebResponse)ex.Response).GetResponseStream()))
                    {
                        string sErrorText = vReader.ReadToEnd();
                        OutputMessage(string.Format("Error Message : {0}", sErrorText));
                    }
                }
                else
                {
                    OutputMessage(string.Format("Web Error : {0}", ex.Message));
                }

                m_bErrorFlag = true;
            }
            catch (Exception ex)
            {
                OutputMessage(string.Format("Error Message : {0}", ex.Message));

                m_bErrorFlag = true;
            }

            OutputMessage("");
        }

        public void GetIssueJournalsByNet()
        {
            if (m_bErrorFlag == true)
                return;

            OutputMessage("Test : Use System.Net to Connect Redmine");

            OutputMessage("");
            OutputMessage(string.Format("Test : Read Journals in Issue : {0}", m_nIssueID));

            try
            {
                // 建立 WebClient
                using (var client = new WebClient())
                {
                    // 設定編碼和標頭
                    client.Encoding = Encoding.UTF8;
                    client.Headers[HttpRequestHeader.ContentType] = "application/json";

                    if (m_nLoginType == 0)
                    {
                        // 設定基本驗證
                        string sAuth = Convert.ToBase64String(Encoding.UTF8.GetBytes(string.Format("{0}:{1}", m_sUserName, m_sPassword)));
                        client.Headers[HttpRequestHeader.Authorization] = string.Format("Basic {0}", sAuth);
                    }
                    else if (m_nLoginType == 1)
                    {
                        client.Headers.Add("X-Redmine-API-Key", m_sAPIKey);
                    }

                    // 建立完整的 URL
                    string sFullUrl = string.Format("{0}/issues/{1}.json?include=journals", m_sRedmineUrl, m_nIssueID);

                    // 下載 JSON 資料
                    string sJsonResponse = client.DownloadString(sFullUrl);

                    // 解析 JSON
                    JObject cIssueData = JObject.Parse(sJsonResponse);
                    JArray cJournals = (JArray)cIssueData["issue"]["journals"];

                    if (cJournals != null && cJournals.Count > 0)
                    {
                        OutputMessage(string.Format("Issue : {0} Journals : ", m_nIssueID));

                        foreach (var vJournal in cJournals)
                        {
                            OutputMessage(string.Format("Log ID : {0}", vJournal["id"]));
                            OutputMessage(string.Format("Create Time : {0}", vJournal["created_on"]));

                            // 使用者資訊
                            var vUser = vJournal["user"];
                            OutputMessage(string.Format("User : {0}", vUser["name"]));

                            // 備註
                            if (vJournal["notes"] != null && !string.IsNullOrEmpty(vJournal["notes"].ToString()))
                            {
                                OutputMessage(string.Format("Note : {0}", vJournal["notes"]));
                            }

                            // 變更細節
                            var vDetails = (JArray)vJournal["details"];
                            if (vDetails != null && vDetails.Count > 0)
                            {
                                OutputMessage("Log Changed : ");
                                foreach (var vDetail in vDetails)
                                {
                                    string sName = vDetail["name"].ToString();
                                    string sOldValue = vDetail["old_value"] != null ? vDetail["old_value"].ToString() : "None";
                                    string sNewValue = vDetail["new_value"] != null ? vDetail["new_value"].ToString() : "None";

                                    OutputMessage(string.Format("- Properties : {0}", sName));
                                    OutputMessage(string.Format("  Old Value : {0}", sOldValue));
                                    OutputMessage(string.Format("  New Value : {0}", sNewValue));
                                }
                            }

                            OutputMessage("---");
                        }
                    }
                    else
                    {
                        OutputMessage(string.Format("Issue : {0} No Journals", m_nIssueID));
                    }
                }
            }
            catch (WebException ex)
            {
                // 處理 WebException
                if (ex.Response is HttpWebResponse)
                {
                    OutputMessage(string.Format("HTTP Error : {0}", ((HttpWebResponse)ex.Response).StatusCode));

                    // 讀取錯誤回應
                    using (var vReader = new System.IO.StreamReader(((HttpWebResponse)ex.Response).GetResponseStream()))
                    {
                        string sErrorText = vReader.ReadToEnd();
                        OutputMessage(string.Format("Error Message : {0}", sErrorText));
                    }
                }
                else
                {
                    OutputMessage(string.Format("Web Error : {0}", ex.Message));
                }

                m_bErrorFlag = true;
            }
            catch (Exception ex)
            {
                OutputMessage(string.Format("Error Message : {0}", ex.Message));

                m_bErrorFlag = true;
            }

            OutputMessage("");
        }

        public void CreateRedmineIssueByNet(string sSubject, string sDescription)
        {
            if (m_bErrorFlag == true)
                return;

            OutputMessage("Test : Use System.Net to Connect Redmine");

            OutputMessage("");
            OutputMessage(string.Format("Test : Create Issue in Project : {0}", m_sProjectID));

            try
            {
                // 準備 JSON 資料
                string sJsonData = string.Format(@"{{
                    ""issue"": {{
                        ""project_id"": ""{0}"",
                        ""subject"": ""{1}"",
                        ""description"": ""{2}""
                    }}
                }}", m_sProjectID, sSubject, sDescription);

                // 建立 WebClient
                using (WebClient client = new WebClient())
                {
                    // 設定編碼和標頭
                    client.Encoding = Encoding.UTF8;
                    client.Headers[HttpRequestHeader.ContentType] = "application/json";

                    if (m_nLoginType == 0)
                    {
                        // 設定基本驗證
                        string sAuth = Convert.ToBase64String(Encoding.UTF8.GetBytes(string.Format("{0}:{1}", m_sUserName, m_sPassword)));
                        client.Headers[HttpRequestHeader.Authorization] = string.Format("Basic {0}", sAuth);
                    }
                    else if (m_nLoginType == 1)
                    {
                        client.Headers.Add("X-Redmine-API-Key", m_sAPIKey);
                    }

                    // 發送請求
                    byte[] byteResponse_Array = client.UploadData(
                        string.Format("{0}/issues.json", m_sRedmineUrl),
                        "POST",
                        Encoding.UTF8.GetBytes(sJsonData)
                    );

                    // 讀取回應
                    string sResponse = Encoding.UTF8.GetString(byteResponse_Array);
                    OutputMessage("Create Issue Success!");
                    OutputMessage(string.Format("Response Content：{0}", sResponse));
                }
            }
            catch (WebException ex)
            {
                if (ex.Response != null)
                {
                    using (StreamReader cReader = new StreamReader(ex.Response.GetResponseStream()))
                    {
                        string sErrorResponse = cReader.ReadToEnd();
                        OutputMessage(string.Format("Create Issue Fail. Error Message : {0}", sErrorResponse));
                    }
                }
                else
                {
                    OutputMessage(string.Format("Error Message : {0}", ex.Message));
                }

                m_bErrorFlag = true;
            }

            OutputMessage("");
        }

        public void CreateIssueBySendPOSTAPI(string sSubject, string sDescription, int nAssignToID)
        {
            if (m_bErrorFlag == true)
                return;

            //OutputMessage("Test : Use Send POST API Method to Connect Redmine");

            string sCreateIssueAPI = string.Format(@"{0}/{1}", m_sServerUrl, m_sCreateIssueRoute);
            //string sCreateIssueAPI = string.Format(@"{0}/{1}", m_sTestServerUrl, "Finger_AutoTuning/Upload_File_Test");

            //OutputMessage("");
            //OutputMessage(string.Format("Test : Create Issue in Project : {0} ", m_sProjectID));

            // 驗證輸入
            /*
            if (m_cUploadFileInforamtion_List == null || m_cUploadFileInforamtion_List.Count == 0)
            {
                OutputMessage("File Path List No Data");
                OutputMessage("");
                m_bErrorFlag = true;
                return;
            }
            */

            // 檢查所有檔案是否存在
            if (m_cUploadFileInforamtion_List != null && m_cUploadFileInforamtion_List.Count > 0)
            {
                foreach (frmRedmineTask.FileInforamtion cFileInforamtion in m_cUploadFileInforamtion_List)
                {
                    string sFilePath = cFileInforamtion.FilePath;

                    if (File.Exists(sFilePath) == false)
                    {
                        OutputMessage(string.Format("File Not Exist {0}", sFilePath));
                        OutputMessage("");
                        m_bErrorFlag = true;
                        return;
                    }
                }
            }

            try
            {
                // 使用匿名物件
                var vParameterData = new
                {
                    login_type = m_nLoginType,
                    username = m_sUserName,
                    password = m_sPassword,
                    api_key = m_sAPIKey,
                    project_id = m_sProjectID,
                    subject = sSubject,
                    description = sDescription,
                    assigned_to_id = nAssignToID,
                    tracker_id = m_nTrackerID,
                    priority_id = m_nPriorifyID,
                    //WriteIndented = true
                };

                string sJsonData = JsonConvert.SerializeObject(vParameterData);

                //string sPostResult = SendPostRequest(sCreateIssueAPI, sJsonData);
                string sPostResult = SendPostRequestToUploadFile(sCreateIssueAPI, sJsonData, m_cUploadFileInforamtion_List);

                JObject jResult = JObject.Parse(sPostResult);

                string sMessage = (string)jResult["message"];
                string sSuccess = (string)jResult["success"];
                bool bSuccessFlag = (bool)jResult["success"];

                //OutputMessage(string.Format("Create Issue Success : {0}", sSuccess));

                if (bSuccessFlag == true)
                {
                    int nCreateIssueID = (int)jResult["data"]["id"];
                    OutputMessage(string.Format("Create Issue Result : {0}, Issue ID : {1}", sMessage, nCreateIssueID));
                    OutputMessage(string.Format("Create New Issue Link : {0}/issues/{1}", m_sRedmineUrl, nCreateIssueID));
                }
                else
                {
                    OutputMessage(string.Format("Create Issue Result : {0}", sMessage));
                    m_bErrorFlag = true;
                }

                OutputMessage("");
            }
            catch (Exception ex)
            {
                string sError = ex.Message;
                OutputMessage(string.Format("Create Issue Error : {0}", sError));
                OutputMessage("");
                m_bErrorFlag = true;
            }
        }

        // POST 請求
        public string SendPostRequest(string sUrl, string sJsonData, string sAPIKey = null)
        {
            try
            {
                // 建立請求
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(sUrl);
                request.Method = "POST";
                request.ContentType = "application/json";
            
                // 如果有 API Key，加入到 Header
                if (!string.IsNullOrEmpty(sAPIKey))
                {
                    request.Headers.Add("Authorization", string.Format("Bearer {0}", sAPIKey));
                }

                // 寫入 JSON 資料
                using (StreamWriter sw = new StreamWriter(request.GetRequestStream()))
                {
                    sw.Write(sJsonData);
                    sw.Flush();
                }

                // 取得回應
                using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                using (StreamReader sr = new StreamReader(response.GetResponseStream()))
                {
                    return sr.ReadToEnd();
                }
            }
            catch (WebException ex)
            {
                // 處理錯誤回應
                if (ex.Response != null)
                {
                    using (StreamReader sr = new StreamReader(ex.Response.GetResponseStream()))
                    {
                        return sr.ReadToEnd();
                    }
                }

                throw;
            }
        }

        // GET 請求
        public string SendGetRequest(string sUrl, string sAPIKey = null)
        {
            try
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(sUrl);
                request.Method = "GET";

                if (!string.IsNullOrEmpty(sAPIKey))
                {
                    request.Headers.Add("Authorization", string.Format("Bearer {0}", sAPIKey));
                }

                using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                using (StreamReader sr = new StreamReader(response.GetResponseStream()))
                {
                    return sr.ReadToEnd();
                }
            }
            catch (WebException ex)
            {
                if (ex.Response != null)
                {
                    using (StreamReader sr = new StreamReader(ex.Response.GetResponseStream()))
                    {
                        return sr.ReadToEnd();
                    }
                }

                throw;
            }
        }

        // PUT 請求
        public string SendPutRequest(string sUrl, string sJsonData, string sAPIKey = null)
        {
            try
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(sUrl);
                request.Method = "PUT";
                request.ContentType = "application/json";

                if (!string.IsNullOrEmpty(sAPIKey))
                {
                    request.Headers.Add("Authorization", string.Format("Bearer {0}", sAPIKey));
                }

                using (StreamWriter sw = new StreamWriter(request.GetRequestStream()))
                {
                    sw.Write(sJsonData);
                    sw.Flush();
                }

                using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                using (StreamReader sr = new StreamReader(response.GetResponseStream()))
                {
                    return sr.ReadToEnd();
                }
            }
            catch (WebException ex)
            {
                if (ex.Response != null)
                {
                    using (StreamReader sr = new StreamReader(ex.Response.GetResponseStream()))
                    {
                        return sr.ReadToEnd();
                    }
                }

                throw;
            }
        }

        // DELETE 請求
        public string SendDeleteRequest(string sUrl, string sAPIKey = null)
        {
            try
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(sUrl);
                request.Method = "DELETE";

                if (!string.IsNullOrEmpty(sAPIKey))
                {
                    request.Headers.Add("Authorization", string.Format("Bearer {0}", sAPIKey));
                }

                using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                using (StreamReader sr = new StreamReader(response.GetResponseStream()))
                {
                    return sr.ReadToEnd();
                }
            }
            catch (WebException ex)
            {
                if (ex.Response != null)
                {
                    using (StreamReader sr = new StreamReader(ex.Response.GetResponseStream()))
                    {
                        return sr.ReadToEnd();
                    }
                }

                throw;
            }
        }

        public string SendPostRequestToUploadFile(string sUrl, string sJsonData, List<frmRedmineTask.FileInforamtion> cUploadFileInforamtion_List, string sAPIKey = null)
        {
            // 生成邊界字串
            string sBoundary = "---------------------------" + DateTime.Now.Ticks.ToString("x");
        
            // 建立請求
            HttpWebRequest cRequest = (HttpWebRequest)WebRequest.Create(sUrl);
            cRequest.ContentType = "multipart/form-data; boundary=" + sBoundary;
            cRequest.Method = "POST";
            cRequest.KeepAlive = true;
            cRequest.Timeout = 300000; // 5分鐘逾時

            // 如果有 API Key，加入到 Header
            if (!string.IsNullOrEmpty(sAPIKey))
            {
                cRequest.Headers.Add("Authorization", string.Format("Bearer {0}", sAPIKey));
            }
        
            using (MemoryStream cMemoryStream = new MemoryStream())
            {
                byte[] byteBoundaryByte_Array = Encoding.UTF8.GetBytes("\r\n--" + sBoundary + "\r\n");

                string sJsonHeader = "Content-Disposition: form-data; name=\"data\"\r\n" + "Content-Type: application/json\r\n\r\n";

                cMemoryStream.Write(byteBoundaryByte_Array, 0, byteBoundaryByte_Array.Length);
                byte[] byteJsonHeader_Array = Encoding.UTF8.GetBytes(sJsonHeader);
                byte[] byteJsonData_Array = Encoding.UTF8.GetBytes(sJsonData);

                cMemoryStream.Write(byteJsonHeader_Array, 0, byteJsonHeader_Array.Length);
                cMemoryStream.Write(byteJsonData_Array, 0, byteJsonData_Array.Length);
                
                // 如果有提供檔案路徑，則加入檔案
                if (cUploadFileInforamtion_List != null && cUploadFileInforamtion_List.Count > 0)
                {
                    // 加入多個檔案
                    for (int nIndex = 0; nIndex < cUploadFileInforamtion_List.Count; nIndex++)
                    {
                        string sFilePath = cUploadFileInforamtion_List[nIndex].FilePath;
                        FileInfo cFileInfo = new FileInfo(sFilePath);

                        cMemoryStream.Write(byteBoundaryByte_Array, 0, byteBoundaryByte_Array.Length);

                        string sFileHeader = string.Format("Content-Disposition: form-data; name=\"files\"; filename=\"{0}\"\r\n", cFileInfo.Name) +
                                             string.Format("Content-Type: {0}\r\n", GetMimeType(cFileInfo.Extension));

                        if (CheckUseContentLength(cFileInfo.Extension) == true)
                            sFileHeader += string.Format("Content-Length: {0}\r\n\r\n", cFileInfo.Length);
                        else
                            sFileHeader += "\r\n";

                        byte[] byteFileHeader_Array = Encoding.UTF8.GetBytes(sFileHeader);
                        cMemoryStream.Write(byteFileHeader_Array, 0, byteFileHeader_Array.Length);

                        // 寫入檔案內容
                        using (FileStream fs = new FileStream(sFilePath, FileMode.Open, FileAccess.Read))
                        {
                            byte[] byteBuffer = new byte[4096]; // 增加buffer大小以提升效能
                            int nBytesRead;

                            while ((nBytesRead = fs.Read(byteBuffer, 0, byteBuffer.Length)) != 0)
                            {
                                cMemoryStream.Write(byteBuffer, 0, nBytesRead);
                            }
                        }

                        if (cUploadFileInforamtion_List[nIndex].Description != null)
                        {
                            cMemoryStream.Write(byteBoundaryByte_Array, 0, byteBoundaryByte_Array.Length);

                            string sDescriptionHeader = string.Format("Content-Disposition: form-data; name=\"file_description_{0}\"\r\n\r\n", nIndex);
                            string sDescriptionContent = cUploadFileInforamtion_List[nIndex].Description;

                            byte[] byteDescriptionHeader_Array = Encoding.UTF8.GetBytes(sDescriptionHeader);
                            byte[] byteDescriptionContent_Array = Encoding.UTF8.GetBytes(sDescriptionContent);

                            cMemoryStream.Write(byteDescriptionHeader_Array, 0, byteDescriptionHeader_Array.Length);
                            cMemoryStream.Write(byteDescriptionContent_Array, 0, byteDescriptionContent_Array.Length);
                        }
                    }
                }
            
                // 加入結束邊界
                byte[] byteEndBoundary = Encoding.UTF8.GetBytes("\r\n--" + sBoundary + "--\r\n");
                cMemoryStream.Write(byteEndBoundary, 0, byteEndBoundary.Length);
            
                // 設定請求內容長度
                cRequest.ContentLength = cMemoryStream.Length;
            
                // 寫入請求資料
                using (Stream cRequestStream = cRequest.GetRequestStream())
                {
                    cMemoryStream.Position = 0;
                    cMemoryStream.CopyTo(cRequestStream);
                }
            }
        
            // 取得回應
            try
            {
                using (WebResponse cResponse = cRequest.GetResponse())
                using (StreamReader sr = new StreamReader(cResponse.GetResponseStream()))
                {
                    string sResult = sr.ReadToEnd();
                    //OutputMessage(string.Format("Upload Successful. Server Response : {0}", sResult));
                    return sResult;
                }
            }
            catch (WebException ex)
            {
                if (ex.Response != null)
                {
                    using (StreamReader sr = new StreamReader(ex.Response.GetResponseStream()))
                    {
                        string sErrorMessage = sr.ReadToEnd();
                        //OutputMessage(string.Format("Upload Failed. Error : {0}", sErrorMessage));
                        return string.Format("Upload Failed. Error : {0}", sErrorMessage);
                    }
                }
                else
                {
                    //OutputMessage(string.Format("Upload Failed. Error : {0}", ex.Message));
                    return string.Format("Upload Failed. Error : {0}", ex.Message);
                }
            }
        }

        // 取得MIME類型
        private string GetMimeType(string sExtension)
        {
            string sMimeType = "application/octet-stream";

            Dictionary<string, string> dictMimeTypes = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
            {
                {".txt", "text/plain"},
                {".csv", "text/csv"},
                {".pdf", "application/pdf"},
                {".doc", "application/msword"},
                {".docx", "application/vnd.openxmlformats-officedocument.wordprocessingml.document"},
                {".xls", "application/vnd.ms-excel"},
                {".xlsx", "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet"},
                {".png", "image/png"},
                {".jpg", "image/jpeg"},
                {".jpeg", "image/jpeg"},
                {".gif", "image/gif"},
                {".zip", "application/zip"},
                {".7z", "application/x-7z-compressed"},
                {".rar", "application/vnd.rar"},
                {".json", "application/json"},
                {".ppt", "application/vnd.ms-powerpoint"},
                {".pptx", "application/vnd.openxmlformats-officedocument.presentationml.presentation"},
                {".py", "application/x-python"}
            };

            return dictMimeTypes.TryGetValue(sExtension, out sMimeType) ? sMimeType : "application/octet-stream";
        }

        private bool CheckUseContentLength(string sExtension)
        {
            bool bUseFlag = false;

            if (sExtension == ".rar" || sExtension == ".ppt" || sExtension == ".pptx")
                bUseFlag = true;

            return bUseFlag;
        }

        private void OutputMessage(string sMessage)
        {
            m_cfrmRedmineTask.BeginInvoke((MethodInvoker)delegate
            {
                m_cfrmRedmineTask.OutputMessage(sMessage);
            });
        }
    }
}