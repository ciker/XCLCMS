﻿using Microsoft.Practices.EnterpriseLibrary.Data;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Text;

namespace XCLCMS.Data.DAL.View
{
    public class v_Article : XCLCMS.Data.DAL.Common.BaseDAL
    {
        public v_Article()
        { }

        #region Method

        /// <summary>
        /// 得到一个对象实体
        /// </summary>
        public XCLCMS.Data.Model.View.v_Article GetModel(long ArticleID)
        {
            XCLCMS.Data.Model.View.v_Article model = new XCLCMS.Data.Model.View.v_Article();
            Database db = base.CreateDatabase();
            DbCommand dbCommand = db.GetSqlStringCommand("select * from v_Article where ArticleID=@ArticleID");
            db.AddInParameter(dbCommand, "ArticleID", DbType.Int64, ArticleID);
            DataSet ds = db.ExecuteDataSet(dbCommand);

            var lst = XCLNetTools.Generic.ListHelper.DataTableToList<XCLCMS.Data.Model.View.v_Article>(ds.Tables[0]);
            return null != lst && lst.Count > 0 ? lst[0] : null;
        }

        /// <summary>
        /// 获得数据列表
        /// </summary>
        public List<XCLCMS.Data.Model.View.v_Article> GetModelList(string strWhere)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("select UpdaterName,UpdaterID,UpdateTime,CreaterName,CreaterID,CreateTime,RecordState,PublishTime,LinkUrl,Comments,Tags,KeyWords,TopEndTime,TopBeginTime,IsTop,IsEssence,IsRecommend,VerifyState,ArticleState,URLOpenType,HotCount,BadCount,MiddleCount,GoodCount,CommentCount,IsCanComment,ViewCount,MainImage3,MainImage2,MainImage1,Summary,Contents,ArticleContentType,FromInfo,AuthorName,SubTitle,Title,Code,ArticleID,ArticleTypeIDs,ArticleTypeNames ");
            strSql.Append(" FROM v_Article ");
            if (strWhere.Trim() != "")
            {
                strSql.Append(" where " + strWhere);
            }
            Database db = base.CreateDatabase();
            DbCommand dbCommand = db.GetSqlStringCommand(strSql.ToString());
            var ds = db.ExecuteDataSet(dbCommand);
            return XCLNetTools.Generic.ListHelper.DataSetToList<XCLCMS.Data.Model.View.v_Article>(ds) as List<XCLCMS.Data.Model.View.v_Article>;
        }

        #endregion Method

        #region Extend Method

        /// <summary>
        /// 分页数据列表
        /// </summary>
        public List<XCLCMS.Data.Model.View.v_Article> GetPageList(XCLNetTools.Entity.PagerInfo pageInfo, string strWhere, string fieldName, string fieldKey, string fieldOrder)
        {
            DataTable dt = XCLCMS.Data.DAL.Common.Common.GetPageList("v_Article", pageInfo, strWhere, fieldName, fieldKey, fieldOrder);
            return XCLNetTools.Generic.ListHelper.DataTableToList<XCLCMS.Data.Model.View.v_Article>(dt) as List<XCLCMS.Data.Model.View.v_Article>;
        }

        /// <summary>
        /// 分页数据列表
        /// </summary>
        public List<XCLCMS.Data.Model.View.v_Article> GetPageList(XCLNetTools.Entity.PagerInfo pageInfo, XCLCMS.Data.Model.Custom.ArticleSearchCondition condition)
        {
            string join_ArticleType = string.Empty;
            var where = new List<string>();
            string strSql = @"

                                            DECLARE @_pageIndex INT=@PageIndex
                                            DECLARE @_pageSize INT=@PageSize

                                            DECLARE @Start INT=0
                                            DECLARE @End INT=0

                                            --获取总记录数
                                            SELECT @TotalCount=COUNT(1) FROM v_Article AS tb_Article WITH(NOLOCK)
                                            #join_ArticleType#
                                            #where#

                                            IF(@_pageIndex<=0) SET @_pageIndex=1
                                            IF(@_pageSize<=0) SET @_pageSize=10

                                            SET @Start=(@_pageIndex-1)*@_pageSize+1
                                            SET @End=@Start+@_pageSize-1

                                            IF(@Start>@TotalCount) SET @Start=@TotalCount
                                            IF(@End>@TotalCount) SET @End=@TotalCount

                                            --分页
                                            ;WITH Data AS
                                            (
                                                SELECT
                                                tb_Article.*,
                                                ROW_NUMBER() OVER (ORDER BY tb_Article.ArticleID DESC) AS RowNumber
                                                FROM dbo.v_Article AS tb_Article WITH(NOLOCK)
                                                #join_ArticleType#
                                                #where#
                                            )
                                            SELECT
                                            *
                                            FROM Data
                                            WHERE RowNumber >= @Start AND RowNumber <= @End

                                    ";

            Database db = base.CreateDatabase();
            DbCommand dbCommand = db.GetSqlStringCommand(strSql.ToString());
            db.AddInParameter(dbCommand, "PageIndex", DbType.Int32, pageInfo.PageIndex);
            db.AddInParameter(dbCommand, "PageSize", DbType.Int32, pageInfo.PageSize);
            db.AddOutParameter(dbCommand, "TotalCount", DbType.Int32, 4);

            if (null != condition)
            {
                if (condition.ArticleTypeID > 0)
                {
                    join_ArticleType = " inner join ArticleType as  tb_ArticleType on tb_Article.ArticleID=tb_ArticleType.FK_ArticleID";
                    where.Add("tb_ArticleType.FK_TypeID=@FK_TypeID");
                    db.AddInParameter(dbCommand, "FK_TypeID", DbType.Int32, condition.ArticleTypeID);
                }
                if (!string.IsNullOrEmpty(condition.RecordState))
                {
                    where.Add("tb_Article.RecordState=@RecordState");
                    db.AddInParameter(dbCommand, "RecordState", DbType.AnsiString, condition.RecordState);
                }
            }

            if (where.Count > 0)
            {
                where[0] = " where " + where[0];
            }
            dbCommand.CommandText = strSql.Replace("#where#", string.Join(" and ", where.ToArray())).Replace("#join_ArticleType#", join_ArticleType);

            var ds = db.ExecuteDataSet(dbCommand);
            pageInfo.RecordCount = XCLNetTools.Common.DataTypeConvert.ToInt(dbCommand.Parameters["@TotalCount"].Value);
            return XCLNetTools.Generic.ListHelper.DataSetToList<XCLCMS.Data.Model.View.v_Article>(ds) as List<XCLCMS.Data.Model.View.v_Article>;
        }

        #endregion Extend Method
    }
}