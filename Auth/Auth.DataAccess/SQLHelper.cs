using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Auth.DataAccess
{
    public class SQLHelper
    {
        //数据库连接字符串
        private string connSTR;

        //数据库连接对象
        private SqlConnection myConnection;

        //数据库操作命令对象
        private SqlCommand myCommand;

        /// <summary>
        /// 构造函数，创建连接
        /// </summary>
        public SQLHelper()
        {
            connSTR = ConfigurationManager.ConnectionStrings["BaseDataCS"].ConnectionString;
            myConnection = new SqlConnection(connSTR);
        }

        #region 执行带参数的sql语句（插入、删除、修改）,返回-1表示执行失败

        public int ExcuSqlWithPara(string cmdText, SqlParameter[] para)
        {
            //创建Command
            myCommand = new SqlCommand(cmdText, myConnection);

            //传递参数
            for (int i = 0; i < para.Length; i++)
            {
                myCommand.Parameters.Add(para[i]);
            }

            //定义返回值
            int nResult = -1;

            try
            {
                //打开链接
                myConnection.Open();

                //执行SQL语句
                nResult = myCommand.ExecuteNonQuery();
            }
            catch (SqlException ex)
            {
                //抛出异常
                throw new Exception(ex.Message, ex);
            }
            finally
            {   //关闭链接
                myConnection.Close();
            }
            //返回nResult
            return nResult;
        }
        #endregion

        #region  执行带参数的sql语句（select语句），返回数据流
        public SqlDataReader GetDRWithPara(string cmdText, SqlParameter[] para)
        {
            //创建Command
            myCommand = new SqlCommand(cmdText, myConnection);
            for (int i = 0; i < para.Length; i++)
            {
                myCommand.Parameters.Add(para[i]);
            }

            ///定义返回值
            SqlDataReader dr = null;
            try
            {
                ///打开链接
                myConnection.Open();
                ///执行SQL语句
                dr = myCommand.ExecuteReader(CommandBehavior.CloseConnection);
            }
            catch (SqlException ex)
            {
                ///抛出异常
                throw new Exception(ex.Message, ex);
            }
            return dr;
        }
        #endregion

        #region  执行带参数的sql语句（select语句），返回数据表
        /// <summary>
        /// 执行带参数的sql语句（select语句），返回数据表
        /// </summary>
        /// <param name="cmdText">带参数的SQL语句</param>
        /// <param name="para">参数列表</param>
        public DataTable GetDTWithPara(string cmdText, SqlParameter[] para)
        {
            //创建Command
            myCommand = new SqlCommand(cmdText, myConnection);
            for (int i = 0; i < para.Length; i++)
            {
                myCommand.Parameters.Add(para[i]);
            }
            SqlDataAdapter da = new SqlDataAdapter();
            DataSet ds = new DataSet();
            try
            {
                ///打开链接
                myConnection.Open();
                ///执行SQL语句
                da.SelectCommand = myCommand;
                da.Fill(ds);
            }
            catch (SqlException ex)
            {
                ///抛出异常
                throw new Exception(ex.Message, ex);
            }
            //返回dr时不能关闭连接
            finally
            {   ///关闭链接
                myConnection.Close();
            }
            //返回nResult
            return ds.Tables[0];
        }
        #endregion

        #region 执行不带参数的sql语句（select语句），返回数据表
        public DataTable GetDataTable(string cmdText)
        {
            ///定义返回值
            DataTable dt = null;
            try
            {
                myConnection.Open();
                SqlDataAdapter da = new SqlDataAdapter(cmdText, myConnection);
                DataSet ds = new DataSet();
                da.Fill(ds, "Table1");
                dt = ds.Tables["Table1"];
                da.Dispose();

            }
            catch (SqlException ex)
            {
                throw new Exception(ex.Message, ex);
            }
            finally
            {
                myConnection.Close();

            }
            return dt;
        }
        #endregion
    }
}
