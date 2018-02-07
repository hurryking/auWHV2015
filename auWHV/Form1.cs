using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Web;
using System.Net;
using System.IO;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Threading;

namespace auWHV
{
    public partial class Form1 : Form
    {

        string gToken="";
        string gJsessionid = "";
        CookieCollection gCookieContainer = null;
        string gPaymentID = "";
        string gPriceLOG = "";
        string gUuid="";
        string gSap_guid="";
        string gCprofile_correlation_id = "";//we should update it every time we modify an appication
        string gTrn = "";
                        

        string user = "eru1989";
        string password = "Dd1234567";


        //personal1
        static string familyName = "viki",
                givenName = "v",
                gender = "M",
                applicant_dob_day = "1",
                applicant_dob_month = "1",
                applicant_dob_year = "1993",
                applicant_marital_status = "N",
                city = "lixiangguo",
                applicant_birth_country = "GERM",
                applicant_offshore_residential_country = "GERM",
                passportNumber = "F78787878",
                applicant_travel_passport_country = "D",        //CHN    change to CHN, save ,submit, pay
                applicant_travel_passport_nationality = "D",    //CHN
                applicant_travel_passport_issue_day = "1",
                applicant_travel_passport_issue_month = "1",
                applicant_travel_passport_issue_year = "2010",
                applicant_travel_passport_expiry_day = "2",
                applicant_travel_passport_expiry_month = "2",
                applicant_travel_passport_expiry_year = "2020",
                issuePlace = "German",
                applicant_enter_propose_day = "3",
                applicant_enter_propose_month = "3",
                applicant_enter_propose_year = "2016",
            //personal2
                applicant_occupation = "programmer",
                applicant_employment_seek = "S",
                applicant_qualifications_hold = "05",
            //offshoreAddress
                applicant_offshore_residential_address = "feawfaewff",
                applicant_offshore_residential_suburb = "aefawef",
                applicant_offshore_residential_other_state_or_province = "fawef",
                applicant_offshore_residential_postcode = "234009",
                applicant_offshore_residential_mobile_phone_country_code = "77",
                applicant_offshore_residential_mobile_phone_number = "33344455555",
            //offshoreContact
                applicant_contact_address = applicant_offshore_residential_address,
                applicant_contact_suburb = applicant_offshore_residential_suburb,
                applicant_contact_other_state_or_province = applicant_offshore_residential_other_state_or_province,
                applicant_contact_postcode = applicant_offshore_residential_postcode,
                applicant_contact_country = applicant_offshore_residential_country,
                applicant_email_address_text = "edwqefqew32%40gmail.com",
                dimia_office = "198",
            //payment
                cardnumber = "6225768717808998",
                expirymonth = "4",
                expiryyear = "2017",
                cardname = "Vivi",
                securityCode = "432";


        public Form1()
        {
            InitializeComponent();
        }

        private void process_TextChanged(object sender, EventArgs e)
        {
            process.SelectionStart = process.Text.Length;
            process.ScrollToCaret();
        }

        public delegate void DSetLog(string str1);
        public void setlog(string s)
        {
            if (process.InvokeRequired)
            {
                // 实例一个委托，匿名方法，
                DSetLog sl = new DSetLog(delegate(string text)
                {
                    process.AppendText(DateTime.Now.ToString() + " " + text + Environment.NewLine);
                });
                // 把调用权交给创建控件的线程，带上参数
                process.Invoke(sl, s);
            }
            else
            {
                process.AppendText(DateTime.Now.ToString() + " " + s + Environment.NewLine);
            }
        }

        public void setlogRed(string s)
        {
            if (process.InvokeRequired)
            {
                DSetLog sl = new DSetLog(delegate(string text)
                {
                    process.AppendText(DateTime.Now.ToString() + " " + text + Environment.NewLine);
                    int i = process.Text.LastIndexOf("\n", process.Text.Length - 2);
                    if (i > 1)
                    {
                        process.Select(i, process.Text.Length);
                        process.SelectionColor = Color.Red;
                        process.Select(i, process.Text.Length);
                        process.SelectionFont = new Font(Font, FontStyle.Bold);
                    }
                });
                process.Invoke(sl, s);
            }
            else
            {
                log.AppendText(DateTime.Now.ToString() + " " + s + Environment.NewLine);
                int i = log.Text.LastIndexOf("\n", log.Text.Length - 2);
                if (i > 1)
                {
                    log.Select(i, log.Text.Length);
                    log.SelectionColor = Color.Red;
                    log.Select(i, log.Text.Length);
                    log.SelectionFont = new Font(Font, FontStyle.Bold);
                }
            }
        }

        public delegate void DSetTestLog(HttpWebRequest req, string respHtml);
        public void setTestLog(HttpWebRequest req, string respHtml)
        {
            if (log.InvokeRequired)
            {
                DSetTestLog sl = new DSetTestLog(delegate(HttpWebRequest req1, string text)
                {
                    log.Text = Environment.NewLine + "返回的HTML源码：";
                    log.Text += Environment.NewLine + text;
                    cookies.Text = "Token: " + gToken + Environment.NewLine;
                    cookies.Text += "JSessionID: " + gJsessionid + Environment.NewLine;
                    cookies.Text += "PaymentID: " + gPaymentID + Environment.NewLine;
                    cookies.Text += Environment.NewLine;
                    cookies.Text += "Cookies：" + Environment.NewLine;
                    foreach (Cookie ck in req1.CookieContainer.GetCookies(req1.RequestUri))
                    {
                        cookies.Text += ck + Environment.NewLine;
                    }
                });
                log.Invoke(sl, req, respHtml);
            }
            else
            {
                log.Text = Environment.NewLine + "返回的HTML源码：";
                log.Text += Environment.NewLine + respHtml;
                cookies.Text = "Token: " + gToken + Environment.NewLine;
                cookies.Text += "JSessionID: " + gJsessionid + Environment.NewLine;
                cookies.Text += "PaymentID: " + gPaymentID + Environment.NewLine;
                cookies.Text += Environment.NewLine;
                cookies.Text += "Cookies：" + Environment.NewLine;
                foreach (Cookie ck in req.CookieContainer.GetCookies(req.RequestUri))
                {
                    cookies.Text += ck + Environment.NewLine;
                }
            }
        }

        public void setRequest(HttpWebRequest req)
        {
            req.AllowAutoRedirect = false;
            req.Accept = "text/html,application/xhtml+xml,application/xml;q=0.9,*/*;q=0.8";
            //req.Accept = "*/*";
            req.KeepAlive = true;
            req.UserAgent = "Mozilla/5.0 (Windows NT 6.3; WOW64; rv:37.0) Gecko/20100101 Firefox/37.0";
            //req.UserAgent = "Mozilla/4.0 (compatible; MSIE 8.0; Windows NT 6.1; WOW64; Trident/4.0; SLCC2; .NET CLR 2.0.50727; .NET CLR 3.5.30729; .NET CLR 3.0.30729; Media Center PC 6.0; InfoPath.3; .NET4.0C; .NET4.0E";
            req.Headers["Accept-Encoding"] = "gzip, deflate";
            req.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;
            req.Host = "online.immi.gov.au";            
            req.CookieContainer = new CookieContainer();
            req.CookieContainer.PerDomainCapacity = 40;
            if (gCookieContainer != null)
            {
                req.CookieContainer.Add(gCookieContainer);
            }
            req.ContentType = "application/x-www-form-urlencoded";
        }

        public int writePostData(HttpWebRequest req, string data)
        {
            byte[] postBytes = Encoding.UTF8.GetBytes(data);
            req.ContentLength = postBytes.Length;
            Stream postDataStream = null;
            try
            {
                postDataStream = req.GetRequestStream();

            }
            catch (WebException webEx)
            {
                setlog("GetRequestStream," + webEx.Status.ToString());
                return -1;
            }
            postDataStream.Write(postBytes, 0, postBytes.Length);
            postDataStream.Close();
            return 1;
        }

        public string resp2html(HttpWebResponse resp)
        {
            string respHtml = "";
            char[] cbuffer = new char[256];
            Stream respStream = resp.GetResponseStream();
            StreamReader respStreamReader = new StreamReader(respStream);//respStream,Encoding.UTF8
            int byteRead = 0;
            try
            {
                byteRead = respStreamReader.Read(cbuffer, 0, 256);

            }
            catch (WebException webEx)
            {
                setlog("respStreamReader, " + webEx.Status.ToString());
                return "";
            }
            while (byteRead != 0)
            {
                string strResp = new string(cbuffer, 0, byteRead);
                respHtml = respHtml + strResp;
                try
                {
                    byteRead = respStreamReader.Read(cbuffer, 0, 256);
                }
                catch (WebException webEx)
                {
                    setlog("respStreamReader, " + webEx.Status.ToString());
                    return "";
                }

            }
            respStreamReader.Close();
            respStream.Close();
            return respHtml;
        }

        /* 
         * return success or not
         */
        public int weLoveMuYue(string url, string method, string referer, bool allowAutoRedirect, string postData)
        {
            HttpWebRequest req = (HttpWebRequest)WebRequest.Create(url);
            HttpWebResponse resp = null;
            setRequest(req);
            req.Method = method;
            req.Referer = referer;
            if (allowAutoRedirect)
            {
                req.AllowAutoRedirect = true;
            }
            while (true)
            {
                if (method.Equals("POST"))
                {
                    writePostData(req, postData);
                }
                string respHtml = "";
                try
                {
                    resp = (HttpWebResponse)req.GetResponse();
                }
                catch (WebException webEx)
                {
                    setlog("respStreamReader, " + webEx.Status.ToString());
                    continue;
                }
                if (resp != null)
                {
                    respHtml = resp2html(resp);
                    if (respHtml.Equals(""))
                    {
                        continue;
                    }
                }
                else
                {
                    continue;
                }
                setTestLog(req, respHtml);
                gCookieContainer = req.CookieContainer.GetCookies(req.RequestUri);
                resp.Close();
                break;
            }
            return 1;
        }

        /* 
         * return responsive HTML
         */
        public string weLoveYue(string url, string method, string referer, bool allowAutoRedirect, string postData)
        {
            HttpWebRequest req = (HttpWebRequest)WebRequest.Create(url);
            HttpWebResponse resp = null;
            setRequest(req);
            req.Method = method;
            req.Referer = referer;
            if (allowAutoRedirect)
            {
                req.AllowAutoRedirect = true;
            }
            while (true)
            {
                if (method.Equals("POST"))
                {
                    writePostData(req, postData);
                }
                string respHtml = "";
                try
                {
                    resp = (HttpWebResponse)req.GetResponse();
                }
                catch (WebException webEx)
                {
                    setlog("respStreamReader, " + webEx.Status.ToString());
                    continue;
                }
                if (resp != null)
                {
                    respHtml = resp2html(resp);
                    if (respHtml.Equals(""))
                    {
                        continue;
                    }
                    setTestLog(req, respHtml);
                    gCookieContainer = req.CookieContainer.GetCookies(req.RequestUri);
                    resp.Close();
                    return respHtml;
                }
                else
                {
                    continue;
                }                
            }
        }

        /*
         * do not handle the response
         */
        public HttpWebResponse weLoveYueer(HttpWebResponse resp, string url, string method, string referer, bool allowAutoRedirect, string postData)
        {
            HttpWebRequest req = (HttpWebRequest)WebRequest.Create(url);
            setRequest(req);
            req.Method = method;
            req.Referer = referer;
            if (allowAutoRedirect)
            {
                req.AllowAutoRedirect = true;
            }
            while (true)
            {
                if (method.Equals("POST"))
                {
                    writePostData(req, postData);
                }
                try
                {
                    resp = (HttpWebResponse)req.GetResponse();
                }
                catch (WebException webEx)
                {
                    setlog("respStreamReader, " + webEx.Status.ToString());
                    continue;
                }
                if (resp != null)
                {
                    gCookieContainer = req.CookieContainer.GetCookies(req.RequestUri);
                    return resp;
                }
                else
                {
                    continue;
                }
            }            
        }

        public int loginF() {
            setlog( " login1..");
            getLoginHtml:
            string LoginUrl = "https://online.immi.gov.au/lusc/login";
            HttpWebRequest req = (HttpWebRequest)WebRequest.Create(LoginUrl);
            HttpWebResponse resp = null;
            setRequest(req);
            req.Method = "GET";
            string respHtml = "";
            
            try
            {
                resp = (HttpWebResponse)req.GetResponse();
            }
            catch (WebException webEx)
            {
                setlog("respStreamReader, " + webEx.Status.ToString());
                goto getLoginHtml;
            }

            if (resp != null)
            {
                respHtml = resp2html(resp);
                if (respHtml.Equals(""))
                {
                    goto getLoginHtml;
                }
            }
            else
            {
                goto getLoginHtml;
            }
            string tokenValP = @"(?<=name=""wc_t"" value="").+(?="" /><ui:panel id=""_0)";//after@, transfer" by""
            Match foundTokenVal = (new Regex(tokenValP)).Match(respHtml);
            if (foundTokenVal.Success)
            {
                gToken = foundTokenVal.Groups[0].Value;
                setlog(" got token");
            }
            if (req.CookieContainer.GetCookies(req.RequestUri)["AMWEBJCT!%2Flusc!JSESSIONID"] != null)
            {//daiyyr
                setlog( " got AMWEBJCT!%2Flusc!JSESSIONID " );
            }
            else
            {
                setlog( " got AMWEBJCT!%2Flusc!JSESSIONID err " );
                return -1;
            }
            setTestLog(req, respHtml);
            resp.Close();


            //1st post
            setlog( " login2.." );
        post1:
            HttpWebRequest req2 = (HttpWebRequest)WebRequest.Create(LoginUrl);
            setRequest(req2);
            req2.CookieContainer = req.CookieContainer;
            req2.Method = "POST";
            req2.Referer = "https://online.immi.gov.au/lusc/login";
            writePostData(req2, "wc_s=1&wc_t="
            + gToken
            + "&_2b0a0a0a3a1a="
            + user
            + "&_2b0a0a0a3b1a="
            + password
            + "&_2b0a0a0a4a=x&cprofile_timings=interface_controls%7Btime%3A99%2Cresult%3A1%7D%3Bhtml_start_load%7Btime%3A2831%2Cresult%3A1%7D%3Bunload_load%7Btime%3A2828%2Cresult%3A1%7D%3B");

        
            try
            {
                resp = (HttpWebResponse)req2.GetResponse();
            }
            catch (WebException webEx)
            {
                setlog("respStreamReader, " + webEx.Status.ToString());
                goto post1;
            }
            if (resp != null)
            {
                respHtml = resp2html(resp);
                if (respHtml.Equals(""))
                {
                    goto getLoginHtml;
                }
            }
            else
            {
                goto post1;
            }

            setTestLog(req2, respHtml);
            if (respHtml.Contains("Invalid username or password."))
            {
                setlog( " username/password error! " );
                return -1;
            }
            else
            {
                setlog( " password verification OK " );
            }
            resp.Close();


            //2nd post
        post2:
            HttpWebRequest req3 = (HttpWebRequest)WebRequest.Create(LoginUrl);
            setRequest(req3);
            req3.CookieContainer = req2.CookieContainer;
            req3.Method = "POST";
            req3.AllowAutoRedirect = false;
            req3.Referer = "https://online.immi.gov.au/lusc/login";
            writePostData(req3,
                "wc_s=2&wc_t="
            + gToken
            + "&_2b0a0a1a4a=x&cprofile_timings=interface_controls%7Btime%3A14%2Cresult%3A1%7D%3Bhtml_start_load%7Btime%3A1804%2Cresult%3A1%7D%3Bunload_load%7Btime%3A1735%2Cresult%3A1%7D%3Bsubmit_load%7Btime%3A2541%2Cresult%3A1%7D%3Blast_click_load_Login%7Btime%3A2544%2Cresult%3A1%7D%3B");

        
            try
            {
                resp = (HttpWebResponse)req3.GetResponse();
            }
            catch (WebException webEx)
            {
                setlog("respStreamReader, " + webEx.Status.ToString());
                goto post2;
            }
            if (resp != null)
            {
                respHtml = resp2html(resp);
                if (respHtml.Equals(""))
                {
                    goto post2;
                }
            }
            else
            {
                goto post2;
            }

            if (req.CookieContainer.GetCookies(req2.RequestUri)["PD-S-SESSION-ID"] != null)
            {
                setlog( " got PD-S-SESSION-ID" );
            }
            else
            {
                setlog( " got PD-S-SESSION-ID err " );
                return -1;
            }
            setTestLog(req3, respHtml);
            gCookieContainer = req3.CookieContainer.GetCookies(req3.RequestUri);
            resp.Close();
            setlog( " login succeed" );
            return 1;
        }

        public int createF()
        {
        /*
        //test if there is an early apply
        string Url = "https://online.immi.gov.au/ola/app";
        HttpWebRequest req = (HttpWebRequest)WebRequest.Create(Url);
        HttpWebResponse resp = null;
        setRequest(req);
        req.Method = "GET";
        req.CookieContainer = new CookieContainer();
        req.CookieContainer.PerDomainCapacity = 40; 
        req.CookieContainer.Add(gCookieContainer);
        string respHtml = "";
        getApps:
        try
        {
            resp = (HttpWebResponse)req.GetResponse();
        }
        catch (WebException webEx)
        {
            setlog("respStreamReader, " + webEx.Status.ToString());
            goto getApps;
        }
        if (resp != null)
        {
            respHtml = resp2html(resp);
            if (respHtml.Equals(""))
            {
                goto getApps;
            }
        }
        else
        {
            goto getLoginHtml;
        }
        setTestLog(req, respHtml);
        resp.Close();
        */
        get2:
            HttpWebResponse resp = null;
            string respHtml = "";

            if (true)//to save time of one request/response, do not probe, and create a new application in any case
            {
                //new apply
                setlog( " create.." );
                string newApplyUrl = "https://online.immi.gov.au/visas/applyNow.do?form=WHM";
                HttpWebRequest req2 = (HttpWebRequest)WebRequest.Create(newApplyUrl);

                setRequest(req2);
                req2.Method = "GET";
                req2.CookieContainer = new CookieContainer();
                req2.CookieContainer.PerDomainCapacity = 40;
                req2.CookieContainer.Add(gCookieContainer);
            
                try
                {
                    resp = (HttpWebResponse)req2.GetResponse();
                }
                catch (WebException webEx)
                {
                    setlog("respStreamReader, " + webEx.Status.ToString());
                    goto get2;
                }
                if (resp != null)
                {
                    respHtml = resp2html(resp);
                    if (respHtml.Equals(""))
                    {
                        goto get2;
                    }
                }
                else
                {
                    goto get2;
                }

                string ValP = @"(?<=base href=""https://online.immi.gov.au/visas/\./;jsessionid=).{51}-worker";
                Match foundVal = (new Regex(ValP)).Match(respHtml);
                if (foundVal.Success)
                {
                    gJsessionid = foundVal.Groups[0].Value;
                    setlog( " got JsessionID" );
                }
                else
                {
                    setlog( " failed to get JsessionID" );
                    return -1;
                }
                setTestLog(req2, respHtml);
                resp.Close();


                //post new apply
            post1:
                string postUrl = "https://online.immi.gov.au/visas/acceptance.do;jsessionid=" + gJsessionid;
                HttpWebRequest req3 = (HttpWebRequest)WebRequest.Create(postUrl);
                setRequest(req3);
                req3.Method = "POST";
                req3.CookieContainer = req2.CookieContainer;
                req3.Referer = "https://online.immi.gov.au/visas/applyNow.do?form=WHM";
                req3.ContentType = "application/x-www-form-urlencoded";
                writePostData(req3, "action=I+have+read+and+agree+to+the+terms+and+conditions");
            
                try
                {
                    resp = (HttpWebResponse)req3.GetResponse();
                }
                catch (WebException webEx)
                {
                    setlog("respStreamReader, " + webEx.Status.ToString());
                    goto post1;
                }
                if (resp != null)
                {
                    respHtml = resp2html(resp);
                    if (respHtml.Equals(""))
                    {
                        goto post1;
                    }
                }
                else
                {
                    goto post1;
                }
                setTestLog(req3, respHtml);
                resp.Close();

            post2:
                string post2URL = "https://online.immi.gov.au/visas/valuesStatement.do";
                HttpWebRequest req4 = (HttpWebRequest)WebRequest.Create(post2URL);
                setRequest(req4);
                req4.Method = "POST";
                req4.CookieContainer = req3.CookieContainer;
                req4.Referer = postUrl;
                req4.ContentType = "application/x-www-form-urlencoded";
                writePostData(req4, "next.x=63&next.y=13");
           
                try
                {
                    resp = (HttpWebResponse)req4.GetResponse();
                }
                catch (WebException webEx)
                {
                    setlog("respStreamReader, " + webEx.Status.ToString());
                    goto post2;
                }
                if (resp != null)
                {
                    respHtml = resp2html(resp);
                    if (respHtml.Equals(""))
                    {
                        goto post2;
                    }
                }
                else
                {
                    goto post2;
                }
                setTestLog(req4, respHtml);
                resp.Close();
            post3:
                string post3URL = "https://online.immi.gov.au/visas/whm/identifyApplicantDetails.do";
                HttpWebRequest req5 = (HttpWebRequest)WebRequest.Create(post3URL);
                setRequest(req5);
                req5.Method = "POST";
                req5.CookieContainer = req4.CookieContainer;
                req5.Referer = post2URL;
                req5.ContentType = "application/x-www-form-urlencoded";
                writePostData(req5,
                 "applicant_name_family=" + familyName
                + "&applicant_name_given=" + givenName
                + "&applicant_sex=" + gender//M
                + "&applicant_dob_day=" + applicant_dob_day//1
                + "&applicant_dob_month=" + applicant_dob_month//1
                + "&applicant_dob_year=" + applicant_dob_year//1993
                + "&applicant_marital_status=" + applicant_marital_status//N
                + "&applicant_birth_town=" + city
                + "&applicant_birth_country=" + applicant_birth_country//GERM
                + "&applicant_offshore_residential_country=" + applicant_offshore_residential_country//GERM
                + "&applicant_travel_passport_number=" + passportNumber
                + "&applicant_travel_passport_country=" + applicant_travel_passport_country//D
                + "&applicant_travel_passport_nationality=" + applicant_travel_passport_nationality//D
                + "&applicant_travel_passport_issue_day=" + applicant_travel_passport_issue_day//1
                + "&applicant_travel_passport_issue_month=" + applicant_travel_passport_issue_month//1
                + "&applicant_travel_passport_issue_year=" + applicant_travel_passport_issue_year//2010
                + "&applicant_travel_passport_expiry_day=" + applicant_travel_passport_expiry_day//2
                + "&applicant_travel_passport_expiry_month=" + applicant_travel_passport_expiry_month//2
                + "&applicant_travel_passport_expiry_year=" + applicant_travel_passport_expiry_year//2020
                + "&applicant_travel_passport_issue_place=" + issuePlace
                + "&applicant_grant_number=&applicant_visa_number=&applicant_health_examination_flag=N&applicant_health_examination_details_text=&applicant_health_assessment_portal_id="
                + "&applicant_enter_propose_day=" + applicant_enter_propose_day//3
                + "&applicant_enter_propose_month=" + applicant_enter_propose_month//3
                + "&applicant_enter_propose_year=" + applicant_enter_propose_year//2016
                + "&applicant_declare_dependent_children_flag=N&applicant_alias_flag=N&applicant_other_citizenships_flag=N&applicant_ever_held_WH462_flag=N&next.x=18&next.y=17"
                );
            
                try
                {
                    resp = (HttpWebResponse)req5.GetResponse();
                }
                catch (WebException webEx)
                {
                    setlog("respStreamReader, " + webEx.Status.ToString());
                    goto post3;
                }
                if (resp != null)
                {
                    respHtml = resp2html(resp);
                    if (respHtml.Contains("which Australia has a working holiday maker arrangement"))
                    {
                        setlog( " application is not open, retry.." );
                        goto post3;
                    }
                    if (respHtml.Equals(""))
                    {
                        goto post3;
                    }
                    else
                    {
                        setlog( " succeed to post passport information!" );
                    }
                }
                else
                {
                    goto post3;
                }
                setTestLog(req5, respHtml);
                resp.Close();
            post4:
                string post4URL = "https://online.immi.gov.au/visas/whm/criticalDataConfirmation.do";
                HttpWebRequest req6 = (HttpWebRequest)WebRequest.Create(post4URL);
                setRequest(req6);
                req6.Method = "POST";
                req6.CookieContainer = req5.CookieContainer;
                req6.Referer = post3URL;
                req6.ContentType = "application/x-www-form-urlencoded";
                writePostData(req6, "applicant_declare_critical_data_correct_flag=Y&next.x=49&next.y=5");
            
                try
                {
                    resp = (HttpWebResponse)req6.GetResponse();
                }
                catch (WebException webEx)
                {
                    setlog("respStreamReader, " + webEx.Status.ToString());
                    goto post4;
                }
                if (resp != null)
                {
                    respHtml = resp2html(resp);
                    if (respHtml.Equals(""))
                    {
                        goto post4;
                    }
                }
                else
                {
                    goto post4;
                }
                setTestLog(req6, respHtml);
                gCookieContainer = req6.CookieContainer.GetCookies(req6.RequestUri);
                resp.Close();
            }
            else
            
            {
                //we do not edit application here
            }
            
            return 1;
        }
        public int personalDetails2F()
        {
            setlog( " personalDetails2.." );
        post1:
            string postURL = "https://online.immi.gov.au/visas/whm/applicantDetailsCont.do";
            HttpWebRequest req = (HttpWebRequest)WebRequest.Create(postURL);
            HttpWebResponse resp = null;
            setRequest(req);
            req.Method = "POST";
            req.CookieContainer.Add(gCookieContainer);
            req.Referer = "https://online.immi.gov.au/visas/whm/criticalDataConfirmation.do";
            req.ContentType = "application/x-www-form-urlencoded";
            writePostData(req,
                "applicant_occupation=" + applicant_occupation//programmer
                + "&applicant_employment_seek=" + applicant_employment_seek//S
                + "&applicant_qualifications_hold=" + applicant_qualifications_hold//05
                + "&next.x=32&next.y=9");
            string respHtml = "";
        
            try
            {
                resp = (HttpWebResponse)req.GetResponse();
            }
            catch (WebException webEx)
            {
                setlog("respStreamReader, " + webEx.Status.ToString());
                goto post1;
            }
            if (resp != null)
            {
                respHtml = resp2html(resp);
                if (respHtml.Equals(""))
                {
                    goto post1;
                }
            }
            else
            {
                goto post1;
            }
            setTestLog(req, respHtml);
            gCookieContainer = req.CookieContainer.GetCookies(req.RequestUri);
            resp.Close();
            setlog( " personal details2 ok" );
            return 1;
        }
        public int offshoreAddressF()
        {
            setlog( " offshoreAddress.." );
            if (
                weLoveMuYue("https://online.immi.gov.au/visas/applicantOffshoreAddress.do",
                        "POST",
                        "https://online.immi.gov.au/visas/whm/applicantDetailsCont.do",
                        false,
                        "applicant_offshore_residential.address_line1=" + applicant_offshore_residential_address//feawfaewff
                        + "&applicant_offshore_residential.address_line2="
                        + "&applicant_offshore_residential.suburb=" + applicant_offshore_residential_suburb//aefawef
                        + "&applicant_offshore_residential.other_state_or_province=" + applicant_offshore_residential_other_state_or_province//fawef
                        + "&applicant_offshore_residential.postcode=" + applicant_offshore_residential_postcode//234009
                        + "&applicant_offshore_residential.country=" + applicant_offshore_residential_country//GERM
                        + "&applicant_offshore_residential_home_phone_country_code="
                        + "&applicant_offshore_residential_home_phone_areacode="
                        + "&applicant_offshore_residential_home_phone_number="
                        + "&applicant_offshore_residential_work_phone_country_code="
                        + "&applicant_offshore_residential_work_phone_areacode="
                        + "&applicant_offshore_residential_work_phone_number="
                        + "&applicant_offshore_residential_mobile_phone_country_code=" + applicant_offshore_residential_mobile_phone_country_code//77
                        + "&applicant_offshore_residential_mobile_phone_number=" + applicant_offshore_residential_mobile_phone_number//33344455555
                        + "&applicant_authorise_enquiry_person_flag=N&next.x=38&next.y=12"
                        )
                == 1
                )
            {
                setlog(" offshoreAddress OK");
            }
            return 1;
        }
        public int offshoreContactF()
        {
            setlog( " offshoreContact.." );
            if (
                weLoveMuYue("https://online.immi.gov.au/visas/applicantOffshoreContact.do",
                        "POST",
                        "https://online.immi.gov.au/visas/applicantOffshoreAddress.do",
                        false,
                        "applicant_contact.address_line1=" + applicant_contact_address//eff
                        + "&applicant_contact.address_line2="
                        + "&applicant_contact.suburb=" + applicant_contact_suburb//weaf
                        + "&applicant_contact.state=australian_state"
                        + "&applicant_contact.other_state_or_province=" + applicant_contact_other_state_or_province//fewa
                        + "&applicant_contact.postcode=" + applicant_contact_postcode//234009
                        + "&applicant_contact.country=" + applicant_contact_country//GERM
                        + "&applicant_declare_consent_to_electronic_comms=Y"
                        + "&applicant_email_address_text=" + applicant_email_address_text//edwqefqew32%40gmail.com
                        + "&dimia_office=" + dimia_office//198
                        + "&next.x=26&next.y=2"
                        )
                == 1
                )
            {
                setlog(" offshoreContact OK");
            }
            return 1;
        }
        public int healthDeclarationsF()
        {
            setlog( " healthDeclarations.." );
            if (
                weLoveMuYue("https://online.immi.gov.au/visas/whm/healthDeclarations.do",
                        "POST",
                        "https://online.immi.gov.au/visas/applicantOffshoreContact.do",
                        false,
                        "applicant_health_risk_overseas_visit_flag=N&applicant_health_risk_enter_hospital_flag=N"
                        + "&applicant_health_risk_assist_or_intend_in_medical_procedure_flag=N"
                        + "&applicant_health_risk_work_in_preschool_flag=N&applicant_health_risk_classroom_flag=N"
                        + "&applicant_health_risk_tb_flag=N&applicant_health_risk_medical_cost_flag=N"
                        + "&applicant_health_risk_assisstance_with_care_mobility_flag=N&next.x=52&next.y=11"
                        )
                == 1
                )
            {
                setlog(" healthDeclarations OK");
            }
            return 1;
        }
        public int characterDeclarationsF()
        {
            setlog( " characterDeclarations.." );
            if (
                weLoveMuYue("https://online.immi.gov.au/visas/characterDeclarations.do",
                        "POST",
                        "https://online.immi.gov.au/visas/whm/healthDeclarations.do",
                        false,
                        "applicant_character_risk_charged_flag=N&applicant_character_risk_charged_text=&applicant_character_risk_convicted_flag"
                        + "=N&applicant_character_risk_convicted_text=&applicant_character_risk_arrest_warrant_flag=N&applicant_character_risk_arrest_warrant_text"
                        + "=&applicant_character_risk_sex_offence_flag=N&applicant_character_risk_sex_offence_text=&applicant_character_risk_sex_offender_register_flag"
                        + "=N&applicant_character_risk_sex_offender_register_text=&applicant_character_risk_acquitted_flag=N&applicant_character_risk_acquitted_text"
                        + "=&applicant_character_risk_not_fit_to_plead_flag=N&applicant_character_risk_not_fit_to_plead_text=&applicant_character_risk_national_security_flag"
                        + "=N&applicant_character_risk_national_security_text=&applicant_character_risk_warcrime_flag=N&applicant_character_risk_warcrime_text"
                        + "=&applicant_character_risk_assoc_criminal_group_flag=N&applicant_character_risk_assoc_criminal_group_text"
                        + "=&applicant_character_risk_assoc_violence_organisation_flag=N&applicant_character_risk_assoc_violence_organisation_text"
                        + "=&applicant_character_risk_served_in_military_flag=N&applicant_character_risk_trained_in_military_flag"
                        + "=N&applicant_character_risk_people_smuggling_flag=N&applicant_character_risk_people_smuggling_text=&applicant_character_risk_deported_flag"
                        + "=N&applicant_character_risk_deported_text=&applicant_character_risk_overstay_flag=N&applicant_character_risk_overstay_text"
                        + "=&applicant_character_risk_outstanding_debt_any_flag=N&applicant_character_risk_outstanding_debt_any_text"
                        + "=&next.x=39&next.y=16"
                        )
                == 1
                )
            {
                setlog(" characterDeclarations ok");
            }
            return 1;
        }
        public int FinalDeclarationF()
        {
            setlog( " FinalDeclaration.." );
            if (
                weLoveMuYue(
				        "https://online.immi.gov.au/visas/whm/applicationFinalDeclaration.do",
                        "POST",
                        "https://online.immi.gov.au/visas/characterDeclarations.do",
                        false,
                        "applicant_declare_final_info_supplied_correct_flag=Y&applicant_pic4020_maybe_unable_grant_required_flag"
                        + "=Y&applicant_pic4020_maybe_cancelled_required_flag=Y&applicant_declare_final_abide_conditions_flag=Y"
                        + "&applicant_declare_final_n_months_with_one_employer_flag=Y&applicant_declare_final_no_long_study_flag"
                        + "=Y&applicant_declare_final_sufficient_funds_flag=Y&applicant_declare_final_supplement_holiday_funds_flag"
                        + "=Y&applicant_declare_final_first_whm_flag=Y&applicant_declare_final_values_statement_flag=Y&applicant_declare_final_advise_info_change_flag"
                        + "=Y&applicant_declare_read_privacy_notice_flag=Y&applicant_consent_to_collection_flag=Y&next.x=30&next.y=16"
                        )
                == 1
                )
            {
                setlog(" FinalDeclaration OK");
            }
            
            return 1;
        }
        public int reviewF(){
            setlog( " review.." );
            if (
                weLoveMuYue(
						"https://online.immi.gov.au/visas/whm/review.do",
                        "POST",
                        "https://online.immi.gov.au/visas/whm/applicationFinalDeclaration.do",
                        false,
                        "next.x=39&next.y=7"
                        )
                == 1
                )
            {
                setlog(" review OK");
            }            
            return 1;
        }
        public int submitF()
        {
            setlog( "submit.." );
            HttpWebResponse resp = null;
            post:
            resp = weLoveYueer(
                        resp,
						"https://online.immi.gov.au/visas/submitOption.do",
                        "POST",
                        "https://online.immi.gov.au/visas/whm/review.do",
                        true,
                        "action=Submit+Now"
                        );

            if (resp != null)
            {
                setlog(" submit ok..");
            }

            string respHtml = resp2html(resp);
            if (respHtml.Equals(""))
            {
                goto post;
            }
            resp.Close();

            
            /*
            //get gPriceLOG
            if (resp.Headers["Location"] == null)
            {
                process.Text += "submit err.." );
                return;
            }
            string ValP = @"(?<=PRICELOG=).+(?=$)";
            Match foundVal = (new Regex(ValP)).Match(resp.Headers["Location"]);//daiyyr
            if (foundVal.Success)
            {
                gPriceLOG = foundVal.Groups[0].Value;
                process.Text += "submit ok" );
            }
            */

            //to get PaymentID            
            string ValP = @"(?<=payments\().+(?=\))";
            Match foundVal = (new Regex(ValP)).Match(resp.ResponseUri.ToString());
            if (foundVal.Success)
            {
                gPaymentID = foundVal.Groups[0].Value;
                setlog( " got paymentID" );
            }

            //get uuid
            ValP = @"(?<=value="").*?(?="" id=""uuid)";
            foundVal = (new Regex(ValP)).Match(respHtml);
            if (foundVal.Success)
            {
                gUuid = foundVal.Groups[0].Value;
            }

            //get sap_guid
            ValP = @"(?<=value="").{32}(?="" id=""sap_guid)";
            foundVal = (new Regex(ValP)).Match(respHtml);
            if (foundVal.Success)
            {
                gSap_guid = foundVal.Groups[0].Value;
            }
            return 1;
        }
        public int payF()
        {
            //string postURL = "https://online.immi.gov.au/payments/eval.htm?PRICELOG=" + gPriceLOG + "&sap-sessioncmd=open";
            //req2.Referer = "https://online.immi.gov.au/payments(bD1lbiZjPTEwMA==)/appspayment.htm?sap-params=bHZfc2FwX2d1aWQ9MDA1MDU2QUUxOUQ0MUVFNTgyRjQ3MEQ0MDkwRERFODImbHZfZXh0X3VpZD0xNTc0NUYzQy1BOEMyLTQxQ0YtQjY3OC02NzIwRkU2REYzQTU%3d";
            //we can get sap-params from response-head-location of https://online.immi.gov.au/payments(bD1lbiZjPTEwMA==)/eval.htm?PRICELOG=2004947185

            setlog("payment..");
            string respHtml = weLoveYue(
                        "https://online.immi.gov.au/payments(" + gPaymentID + ")/appspayment.htm",
                        "POST",
                        null,
                        true,
                        "yesButton=Yes&cardnumber=" + cardnumber//6225768717808998
                        + "&expirymonth=" + expirymonth//4
                        + "&expiryyear=" + expiryyear//2017
                        + "+&cardname=" + cardname//resgsrg
                        + "&securityCode=" + securityCode//432
                        + "&cardFee=0.00&totalAmountWithFee=420.00&OnInputProcessing=ccbpaysubmit&uuid=" + gUuid
                        + "&sap_guid=" + gSap_guid
            );
            if (respHtml.Contains("Invalid"))
            {
                setlog( " credit card imformation error!" );
            }
            else
            {
                setlog( " payment succeed! Welcome to Australia!!!" );
            }
            return 1;
        }
        public int modifyNationalityF()
        {
            setlog("modifyNationality..");
            string respHtml = weLoveYue(
                        "https://online.immi.gov.au/visas/whm/identifyApplicantDetails.do",
                        "POST",
                        "https://online.immi.gov.au/visas/valuesStatement.do",
                        false,
                        "applicant_name_family=" + familyName
            + "&applicant_name_given=" + givenName
            + "&applicant_sex=" + gender//M
            + "&applicant_dob_day=" + applicant_dob_day//1
            + "&applicant_dob_month=" + applicant_dob_month//1
            + "&applicant_dob_year=" + applicant_dob_year//1993
            + "&applicant_marital_status=" + applicant_marital_status//N
            + "&applicant_birth_town=" + city
            + "&applicant_birth_country=" + applicant_birth_country//GERM
            + "&applicant_offshore_residential_country=" + applicant_offshore_residential_country//GERM
            + "&applicant_travel_passport_number=" + passportNumber
            + "&applicant_travel_passport_country=" + "IRL"//CHN
            + "&applicant_travel_passport_nationality=" + "IRL"//CHN
            + "&applicant_travel_passport_issue_day=" + applicant_travel_passport_issue_day//1
            + "&applicant_travel_passport_issue_month=" + applicant_travel_passport_issue_month//1
            + "&applicant_travel_passport_issue_year=" + applicant_travel_passport_issue_year//2010
            + "&applicant_travel_passport_expiry_day=" + applicant_travel_passport_expiry_day//2
            + "&applicant_travel_passport_expiry_month=" + applicant_travel_passport_expiry_month//2
            + "&applicant_travel_passport_expiry_year=" + applicant_travel_passport_expiry_year//2020
            + "&applicant_travel_passport_issue_place=" + issuePlace
            + "&applicant_grant_number=&applicant_visa_number=&applicant_health_examination_flag=N&applicant_health_examination_details_text=&applicant_health_assessment_portal_id="
            + "&applicant_enter_propose_day=" + applicant_enter_propose_day//3
            + "&applicant_enter_propose_month=" + applicant_enter_propose_month//3
            + "&applicant_enter_propose_year=" + applicant_enter_propose_year//2016
            + "&applicant_declare_dependent_children_flag=N&applicant_alias_flag=N&applicant_other_citizenships_flag=N&applicant_ever_held_WH462_flag=N&next.x=18&next.y=17"
            );

            if (respHtml.Contains("which Australia has a working holiday maker arrangement"))
            {
                setlog( " application is not open, process shutdown" );
                return -1;
            }
            else
            {
                setlog( " succeed to post passport information!" );
            }
            return 1;
        }
        public int editF()
        {
            setlog("get app page..");
            string respHtml = weLoveYue(
					"https://online.immi.gov.au/ola/app",
                    "GET",
                    "https://online.immi.gov.au/usm/",
                    false,
                    null
                    );
            if (respHtml.Contains("_0a2a0j0b3a-row-"))//there is at least one earlier application
            {
                //edit apply

                //get cprofile_correlation_id
                string ValP = @"(?<=cprofile_correlation_id\"" value=\"").*?(?=\"")";
                Match foundVal = (new Regex(ValP)).Match(respHtml);
                if (foundVal.Success)
                {
                    gCprofile_correlation_id = foundVal.Groups[0].Value;
                    gCprofile_correlation_id = gCprofile_correlation_id.Replace("[", "%5B").Replace(":", "%3A").Replace("]", "%5D");
                    setlog( " got cprofile_correlation_id" );
                }
                
                //get gTrn
                ValP = @"(?<=_0a2a0j0b3a-row-r0-_0a0a"" type=""link"">).*?(?=</ui)";
                foundVal = (new Regex(ValP)).Match(respHtml);
                if (foundVal.Success)
                {
                    gTrn = foundVal.Groups[0].Value;                    
                    setlog( " got trn" );
                }

                setlog( " edit.." );
                respHtml = weLoveYue(
                    "https://online.immi.gov.au/ola/app",
                    "POST",
                    "https://online.immi.gov.au/ola/app",
                    true,
                    "wc_s=1&wc_t=" + gToken + "&_0a0a1-h=x&_0a1a-h=x&_0a1b-h=x&_0a2a0j0b0b0a0a=&_0a2a0j0b3a-row-r0-_0g0a0c=x&_0a2a0j0b3a-row-r0-_0g0a-h=x"
                     + "&_0a2a0j0b3a-h=x&cprofile_correlation_id=" + gCprofile_correlation_id
                     + "&_0a1a0.selected=x&_0a1a-h=x&_0a1b-h=x&_0a2a0j0b3a-row-r0-_0g0a0.open=true&_0a2a0j0b3a-row-r0-_0g0a-h=x"
                     + "&cprofile_timings=interface_controls%7Btime%3A0%2Cresult%3A1%7D%3Bhtml_start_load%7Btime%3A6972%2Cresult%3A1%7D%3Bunload_load%7B"
                     + "time%3A8005%2Cresult%3A1%7D%3Bsubmit_load%7Btime%3A11289%2Cresult%3A1%7D%3Blast_click_load_Continue%7Btime%3A11291%2Cresult%3A1%7D%3B"                 
                    );
                //req2.Headers.Add("Origin", "https://online.immi.gov.au");
                //req2.Headers.Add("Pragma", "no-cache");

                //update cprofile_correlation_id
                ValP = @"(?<=cprofile_correlation_id\"" value=\"").*?(?=\"")";
                foundVal = (new Regex(ValP)).Match(respHtml);
                if (foundVal.Success)
                {
                    gCprofile_correlation_id = foundVal.Groups[0].Value;
                    gCprofile_correlation_id = gCprofile_correlation_id.Replace("[", "%5B").Replace(":", "%3A").Replace("]", "%5D");
                    setlog( " got cprofile_correlation_id" );
                }

                /**
                 * 
                 * if it is a virgin application(never been edited), we should post a reqV to choose role
                 * 
                 **/
                setlog("virgin application, choose role..");
                respHtml = weLoveYue(
                    "https://online.immi.gov.au/ola/app",
                    "POST",
                    "https://online.immi.gov.au/visas/applyNow.do?form=WHM",
                    true,
                    "wc_s=1&wc_t=" + gToken + "&_0a0a1-h=x&_0a1a-h=x&_0a1b-h=x&_0a2a0c0b0b=1&_0a2a0c0b0b-h=x"
                 + "&_0a2a0c0c0=x&cprofile_correlation_id=" + gCprofile_correlation_id
                 + "&_0a1a0.selected=x&_0a1a-h=x&_0a1b-h=x&_0a2a0j0b3a-row-r0-_0g0a0.open=true&_0a2a0j0b3a-row-r0-_0g0a-h=x"
                 + "&cprofile_timings=interface_controls%7Btime%3A0%2Cresult%3A1%7D%3Bhtml_start_load%7Btime%3A6972%2Cresult%3A1%7D%3Bunload_load%7B"
                 + "time%3A8005%2Cresult%3A1%7D%3Bsubmit_load%7Btime%3A11289%2Cresult%3A1%7D%3Blast_click_load_Continue%7Btime%3A11291%2Cresult%3A1%7D%3B"
                 );

                /****
                 * 
                 * we are supposed to be navigated to 
                 * https://online.immi.gov.au/visas/startRetrieve.do?action=edit&trn=EGO8SI5TLV&signature=Sp63cDbEOp5%2FpEpCNCHMyQTtFkA%3D
                 * 
                 * which hold a jsessionid
                 ****/

                ValP = @"(?<=base href=""https://online.immi.gov.au/visas/\./;jsessionid=).{51}-worker";
                foundVal = (new Regex(ValP)).Match(respHtml);
                if (foundVal.Success)
                {
                    gJsessionid = foundVal.Groups[0].Value;
                    setlog( " got JsessionID" );
                }
                else
                {
                    setlog( " failed to get JsessionID" );
         //           return -1;
                }
                 

                //set acceptance
                setlog("set acceptance..");
                weLoveMuYue(
						"https://online.immi.gov.au/visas/acceptance.do;jsessionid=" + gJsessionid,
                        "POST",
                        "https://online.immi.gov.au/visas/applyNow.do?form=WHM",
                        false,
                        "action=I+have+read+and+agree+to+the+terms+and+conditions"
                        );

                //edit this apply
                setlog("edit this apply..");
                weLoveMuYue(
                        "https://online.immi.gov.au/visas/valuesStatement.do",
                        "POST",
                        "https://online.immi.gov.au/visas/acceptance.do",
                        false,
                        "next.x=63&next.y=13"
                        );

            /*
                string post3URL = "https://online.immi.gov.au/visas/whm/identifyApplicantDetails.do";
                HttpWebRequest req5 = (HttpWebRequest)WebRequest.Create(post3URL);
                setRequest(req5);
                req5.Method = "POST";
                req5.CookieContainer = req4.CookieContainer;
                req5.Referer = post2URL;
                req5.ContentType = "application/x-www-form-urlencoded";
                writePostData(req5,
                 "applicant_name_family=" + familyName
                + "&applicant_name_given=" + givenName
                + "&applicant_sex=" + gender//M
                + "&applicant_dob_day=" + applicant_dob_day//1
                + "&applicant_dob_month=" + applicant_dob_month//1
                + "&applicant_dob_year=" + applicant_dob_year//1993
                + "&applicant_marital_status=" + applicant_marital_status//N
                + "&applicant_birth_town=" + city
                + "&applicant_birth_country=" + applicant_birth_country//GERM
                + "&applicant_offshore_residential_country=" + applicant_offshore_residential_country//GERM
                + "&applicant_travel_passport_number=" + passportNumber
                + "&applicant_travel_passport_country=" + applicant_travel_passport_country//D
                + "&applicant_travel_passport_nationality=" + applicant_travel_passport_nationality//D
                + "&applicant_travel_passport_issue_day=" + applicant_travel_passport_issue_day//1
                + "&applicant_travel_passport_issue_month=" + applicant_travel_passport_issue_month//1
                + "&applicant_travel_passport_issue_year=" + applicant_travel_passport_issue_year//2010
                + "&applicant_travel_passport_expiry_day=" + applicant_travel_passport_expiry_day//2
                + "&applicant_travel_passport_expiry_month=" + applicant_travel_passport_expiry_month//2
                + "&applicant_travel_passport_expiry_year=" + applicant_travel_passport_expiry_year//2020
                + "&applicant_travel_passport_issue_place=" + issuePlace
                + "&applicant_grant_number=&applicant_visa_number=&applicant_health_examination_flag=N&applicant_health_examination_details_text=&applicant_health_assessment_portal_id="
                + "&applicant_enter_propose_day=" + applicant_enter_propose_day//3
                + "&applicant_enter_propose_month=" + applicant_enter_propose_month//3
                + "&applicant_enter_propose_year=" + applicant_enter_propose_year//2016
                + "&applicant_declare_dependent_children_flag=N&applicant_alias_flag=N&applicant_other_citizenships_flag=N&applicant_ever_held_WH462_flag=N&next.x=18&next.y=17"
                );
            post3:
                try
                {
                    resp = (HttpWebResponse)req5.GetResponse();
                }
                catch (WebException webEx)
                {
                    if (webEx.Status == WebExceptionStatus.Timeout)
                    {
                        setlog( " create post time out.." );
                        goto post3;
                    }
                }
            if (resp != null)
            {
                respHtml = resp2html(resp);
                if (respHtml.Contains("which Australia has a working holiday maker arrangement"))
                {
                    setlog( " application is not open, retry.." );
                    goto post3;
                }
                else
                {
                    setlog( " succeed to post passport information!" );
                }
            }
            else
            {
                goto post3;
            }
                setTestLog(req5, respHtml);
                resp.Close();

                string post4URL = "https://online.immi.gov.au/visas/whm/criticalDataConfirmation.do";
                HttpWebRequest req6 = (HttpWebRequest)WebRequest.Create(post4URL);
                setRequest(req6);
                req6.Method = "POST";
                req6.CookieContainer = req5.CookieContainer;
                req6.Referer = post3URL;
                req6.ContentType = "application/x-www-form-urlencoded";
                writePostData(req6, "applicant_declare_critical_data_correct_flag=Y&next.x=49&next.y=5");
            post4:
                try
                {
                    resp = (HttpWebResponse)req6.GetResponse();
                }
                catch (WebException webEx)
                {
                    if (webEx.Status == WebExceptionStatus.Timeout)
                    {
                        goto post4;
                    }
                }
                if (resp != null)
                {
                    respHtml = resp2html(resp);
                }
                else
                {
                    goto post4;
                }
                setTestLog(req6, respHtml);
                gCookieContainer = req6.CookieContainer.GetCookies(req6.RequestUri);
                resp.Close();
            */

            }
            else
            {
                setlog( " there is no any earlier application!" );
            }
             
             
            return 1;
        }
        private void login_handler()
        {
            loginF();
        }
        private void login_Click(object sender, EventArgs e)
        {
            Thread t = new Thread(login_handler);
            t.Start();
        }
        private void create_handler()
        {
            createF();
        }
        private void create_Click(object sender, EventArgs e)
        {
            Thread t = new Thread(create_handler);
            t.Start();
        }
        private void personal_handler()
        {
            personalDetails2F();
        }
        private void personalDetails2_Click(object sender, EventArgs e)
        {
            Thread t = new Thread(personal_handler);
            t.Start();
        }
        private void offshoreAddress_handler()
        {
            offshoreAddressF();
        }
        private void offshoreAddress_Click(object sender, EventArgs e)
        {
            Thread t = new Thread(offshoreAddress_handler);
            t.Start();
        }
        private void offshoreContact_handler()
        {
            offshoreContactF();
        }
        private void offshoreContact_Click(object sender, EventArgs e)
        {
            Thread t = new Thread(offshoreContact_handler);
            t.Start();
        }
        private void healthDeclarations_handler()
        {
            healthDeclarationsF();
        }
        private void healthDeclarations_Click(object sender, EventArgs e)
        {
            Thread t = new Thread(healthDeclarations_handler);
            t.Start();
        }
        private void characterDeclarations_handler()
        {
            characterDeclarationsF();
        }
        private void characterDeclarations_Click(object sender, EventArgs e)
        {
            Thread t = new Thread(characterDeclarations_handler);
            t.Start();
        }
        private void FinalDeclaration_handler()
        {
            FinalDeclarationF();
        }
        private void FinalDeclaration_Click(object sender, EventArgs e)
        {
            Thread t = new Thread(FinalDeclaration_handler);
            t.Start();
        }
        private void review_handler()
        {
            reviewF();
        }
        private void review_Click(object sender, EventArgs e)
        {
            Thread t = new Thread(review_handler);
            t.Start();
        }
        private void submit_handler()
        {
            submitF();
        }
        private void submit_Click(object sender, EventArgs e)
        {
            Thread t = new Thread(submit_handler);
            t.Start();
        }
        private void pay_handler()
        {
            payF();
        }
        private void pay_Click(object sender, EventArgs e)
        {
            Thread t = new Thread(pay_handler);
            t.Start();
        }

        private void modify_handler()
        {
            modifyNationalityF();
        }

        private void modifyNationality_Click(object sender, EventArgs e)
        {
            Thread t = new Thread(modify_handler);
            t.Start();
        }

        private void edit_handler()
        {
            editF();
        }

        private void edit_Click(object sender, EventArgs e)
        {
            Thread t = new Thread(edit_handler);
            t.Start();
        }

        private void auto_handler()
        {
            loginF();
            createF();
            personalDetails2F();
            offshoreAddressF();
            offshoreContactF();
            healthDeclarationsF();
            characterDeclarationsF();
            FinalDeclarationF();
            reviewF();
            submitF();
            payF();
        }

        private void auto_Click(object sender, EventArgs e)
        {
            Thread t = new Thread(auto_handler);
            t.Start();
        }        
    }
}


//改国籍后保存, 直接post声明页可以？