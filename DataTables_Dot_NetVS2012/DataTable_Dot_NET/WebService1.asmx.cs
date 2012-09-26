//Created by Nick Benedict at Coastal Web Development LLC CoastalWebDevelopment.com benni12@gmail.com
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;
using System.Web.Script.Services;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;

namespace DataTable_Dot_NET
{
    /// <summary>
    /// Summary description for WebService1
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
     [System.Web.Script.Services.ScriptService]
    public class WebService1 : System.Web.Services.WebService
    {

        [WebMethod]
        [ScriptMethod(UseHttpGet = true)]
     
        public string GetItems()
        {
            int sEcho = ToInt(HttpContext.Current.Request.Params["sEcho"]);
            int iDisplayLength = ToInt(HttpContext.Current.Request.Params["iDisplayLength"]);
            int iDisplayStart = ToInt(HttpContext.Current.Request.Params["iDisplayStart"]);
            string rawSearch = HttpContext.Current.Request.Params["sSearch"];
            
            string participant = HttpContext.Current.Request.Params["iParticipant"];

            var sb = new StringBuilder();

            var whereClause = string.Empty;
            if (participant.Length > 0)
            {
                sb.Append(" Where UserName like ");
                sb.Append("'%" + participant + "%'");
                whereClause = sb.ToString();
            }
            sb.Clear();

            var filteredWhere = string.Empty;
            
            var wrappedSearch = "'%" + rawSearch + "%'";

            if (rawSearch.Length > 0)
            { 
                sb.Append(" WHERE ID LIKE ");
                sb.Append(wrappedSearch);
                sb.Append(" OR UserName LIKE ");
                sb.Append(wrappedSearch);                                  

                filteredWhere = sb.ToString();
            }


            //ORDERING

            sb.Clear();

            string orderByClause = string.Empty;
            sb.Append(ToInt(HttpContext.Current.Request.Params["iSortCol_0"]));

            sb.Append(" ");

            sb.Append(HttpContext.Current.Request.Params["sSortDir_0"]);

            orderByClause = sb.ToString();
           
            if (!String.IsNullOrEmpty(orderByClause))
            {

                orderByClause = orderByClause.Replace("0", ", ID ");
                orderByClause = orderByClause.Replace("1", ", UserName ");             

            
                orderByClause = orderByClause.Remove(0, 1);
            }
            else
            {
                orderByClause = "ID ASC";
            }
            orderByClause = "ORDER BY " + orderByClause;

            sb.Clear();

            var numberOfRowsToReturn = "";
            numberOfRowsToReturn = iDisplayLength == -1 ? "TotalRows" : (iDisplayStart + iDisplayLength).ToString();

          
            string query = @" 
                            declare @MA TABLE(  ID INT, UserName VARCHAR(100))
                            INSERT
                            INTO
	                            @MA ( ID, UserName )
	                                Select ID, Username 
	                                FROM [User] 
	                                {4}                   

                            SELECT *
                            FROM
	                            (SELECT row_number() OVER ({0}) AS RowNumber
		                              , *
	                             FROM
		                             (SELECT (SELECT count([@MA].ID)
				                              FROM
					                              @MA) AS TotalRows
			                               , ( SELECT  count( [@MA].ID) FROM @MA {1}) AS TotalDisplayRows			   
			                               ,[@MA].ID
			                               ,[@MA].UserName       
		                              FROM
			                              @MA {1}) RawResults) Results
                            WHERE
	                            RowNumber BETWEEN {2} AND {3}";


            query = String.Format(query, orderByClause, filteredWhere, iDisplayStart + 1, numberOfRowsToReturn, whereClause);

            var connectionString = ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;
            SqlConnection conn = new SqlConnection(connectionString);
          
            try
            {
                conn.Open();
            }
            catch(Exception e )
            {
                Console.WriteLine(e.ToString());
            }

            var DB=new SqlCommand();
            DB.Connection=conn;
            DB.CommandText=query;
            var  data = DB.ExecuteReader();
            
            var totalDisplayRecords = "";
            var totalRecords = "";
            string outputJson = string.Empty;

            var rowClass = "";
            var count = 0;
        
            while(data.Read())
            {

                if (totalRecords.Length ==0)
                {
                    totalRecords = data["TotalRows"].ToString();
                    totalDisplayRecords = data["TotalDisplayRows"].ToString();
                }
                sb.Append("{");
                sb.AppendFormat(@"""DT_RowId"": ""{0}""", count++);
                sb.Append(",");
                sb.AppendFormat(@"""DT_RowClass"": ""{0}""", rowClass);
                sb.Append(",");
                sb.AppendFormat(@"""0"": ""{0}""", data["ID"]); 
                sb.Append(",");
                sb.AppendFormat(@"""1"": ""{0}""", data["UserName"]);
                sb.Append("},");
            }

            // handles zero records
            if (totalRecords.Length == 0)
            {
                sb.Append("{");
                sb.Append(@"""sEcho"": ");
                sb.AppendFormat(@"""{0}""", sEcho);
                sb.Append(",");
                sb.Append(@"""iTotalRecords"": 0");
                sb.Append(",");
                sb.Append(@"""iTotalDisplayRecords"": 0");
                sb.Append(", ");
                sb.Append(@"""aaData"": [ ");
                sb.Append("]}");
                outputJson = sb.ToString();

                return outputJson;
            }
            outputJson = sb.Remove(sb.Length - 1, 1).ToString();
            sb.Clear();

            sb.Append("{");
            sb.Append(@"""sEcho"": ");
            sb.AppendFormat(@"""{0}""", sEcho);
            sb.Append(",");
            sb.Append(@"""iTotalRecords"": ");
            sb.Append(totalRecords);
            sb.Append(",");
            sb.Append(@"""iTotalDisplayRecords"": ");
            sb.Append(totalDisplayRecords);
            sb.Append(", ");
            sb.Append(@"""aaData"": [ ");
            sb.Append(outputJson);
            sb.Append("]}");
            outputJson = sb.ToString();

            return outputJson;
        }

        public static int ToInt(string toParse)
        {
            int result;
            if (int.TryParse(toParse, out result)) return result;           

            return result;
        }
    
    
    }
}