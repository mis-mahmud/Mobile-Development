using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Web;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace BitopiDBContext
{
    public class UtilityOptions
    {


        public static string toSqlString(SqlParameter[] param, string sql)
        {
            if (param == null) return sql;
            sql += " ";
            foreach (var p in param)
            {
                sql += p.ParameterName + "='" + p.SqlValue + "',";
            }
            //sql = sql.Substring(sql.LastIndexOf(','), 1);
            return sql;
        }
        public static string toSqlString(Dictionary<string, string> param, string sql)
        {
            if (param == null) return sql;
            sql += " ";
            foreach (var p in param)
            {
                sql += p.Key + "='" + p.Value + "',";
            }
            //sql = sql.Substring(sql.LastIndexOf(','), 1);
            return sql;
        }
        public static T DesrializeFromXml<T>(string xml, T toDesirialize, string xmlRoot)
        {
            var doc = XDocument.Parse(xml);
            var reader = doc.Root.CreateReader();
            XmlSerializer deserializer = new XmlSerializer(toDesirialize.GetType(), new XmlRootAttribute(xmlRoot));
            //TextReader textReader = new StreamReader(@"C:\test.xml");
            toDesirialize = (T)deserializer.Deserialize(reader);
            reader.Close();
            return toDesirialize;
        }
        public static string GetXMLFromObject(object o)
        {
            StringWriter sw = new StringWriter();
            XmlTextWriter tw = null;
            try
            {
                XmlSerializer serializer = new XmlSerializer(o.GetType());
                tw = new XmlTextWriter(sw);
                serializer.Serialize(tw, o);
            }
            catch (Exception ex)
            {
                //Handle Exception Code
            }
            finally
            {
                sw.Close();
                if (tw != null)
                {
                    tw.Close();
                }
            }
            return sw.ToString();
        }
    }
}