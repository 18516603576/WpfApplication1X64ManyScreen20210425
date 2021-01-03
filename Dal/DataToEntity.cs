using System;
using System.Collections.Generic;
using System.Data;
using System.Reflection;

namespace Dal
{

    public class DataToEntity<T> where T : new()
    {
        /// <summary>
        /// 填充对象列表：用DataSet的第一个表填充实体类
        /// </summary>
        /// <param name="ds">DataSet</param>
        /// <returns></returns>
        public static List<T> FillModel(DataSet ds)
        {
            if (ds == null || ds.Tables[0] == null || ds.Tables[0].Rows.Count == 0)
            {
                return null;
            }
            else
            {
                return FillModel(ds.Tables[0]);
            }
        }

        // <summary>  
        /// 填充对象列表：用DataSet的第index个表填充实体类
        /// </summary>  
        public static List<T> FillModel(DataSet ds, int index)
        {
            if (ds == null || ds.Tables.Count <= index || ds.Tables[index].Rows.Count == 0)
            {
                return null;
            }
            else
            {
                return FillModel(ds.Tables[index]);
            }
        }

        /// <summary>  
        /// 填充对象列表：用DataTable填充实体类
        /// </summary>   
        public static List<T> FillModel(DataTable dt)
        {
            List<T> modelList = new List<T>();
            if (dt == null || dt.Rows.Count == 0)
            {
                return modelList;
            }
            foreach (DataRow dr in dt.Rows)
            {
                //T model = (T)Activator.CreateInstance(typeof(T));  
                T model = new T();
                for (int i = 0; i < dr.Table.Columns.Count; i++)
                {
                    PropertyInfo propertyInfo = model.GetType().GetProperty(dr.Table.Columns[i].ColumnName, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);

                    if (propertyInfo != null && dr[i] != DBNull.Value)
                    {
                        string value = dr[i] == null ? "" : dr[i].ToString();
                        //string typestr = dr[i].GetType().Name;
                        //if(typestr.Equals("DateTime"))
                        //value = dr[i].ToString();

                        Type type = propertyInfo.PropertyType;
                        if (type == typeof(Boolean))
                        {
                            var convertType = Convert.ToInt32(value);
                            var convertValue = false;
                            if (convertType == 0)
                            {
                                convertValue = false;
                            }
                            else
                            {
                                convertValue = true;
                            }

                            propertyInfo.SetValue(model, convertValue, null);
                        }
                        else if (type == typeof(Int32))
                        {
                            var convertType = Convert.ToInt32(value);
                            propertyInfo.SetValue(model, convertType, null);
                        }
                        else if (type == typeof(decimal))
                        {
                            propertyInfo.SetValue(model, Convert.ToDecimal(value), null);
                        }
                        else if (type == typeof(DateTime))
                        {
                            propertyInfo.SetValue(model, Convert.ToDateTime(value), null);
                        }
                        else if (type == typeof(string))
                        {
                            propertyInfo.SetValue(model, value, null);
                        }

                    }

                }

                modelList.Add(model);
            }
            return modelList;
        }

        /// <summary>  
        /// 填充对象：用DataRow填充实体类
        /// </summary>  
        public static T FillModel(DataRow dr)
        {
            if (dr == null)
            {
                return default(T);
            }

            //T model = (T)Activator.CreateInstance(typeof(T));  
            T model = new T();

            for (int i = 0; i < dr.Table.Columns.Count; i++)
            {
                PropertyInfo propertyInfo = model.GetType().GetProperty(dr.Table.Columns[i].ColumnName, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);
                if (propertyInfo != null && dr[i] != DBNull.Value)
                {
                    string value = dr[i] == null ? "" : dr[i].ToString();
                    //string typestr = dr[i].GetType().Name;
                    //if(typestr.Equals("DateTime"))
                    //value = dr[i].ToString();
                    //propertyInfo.SetValue(model, value, null);
                    Type type = propertyInfo.PropertyType;
                    if (type == typeof(Boolean))
                    {
                        var convertType = Convert.ToInt32(value);
                        var convertValue = false;
                        if (convertType == 0)
                        {
                            convertValue = false;
                        }
                        else
                        {
                            convertValue = true;
                        }

                        propertyInfo.SetValue(model, convertValue, null);
                    }
                    else if (type == typeof(Int32))
                    {
                        var convertType = Convert.ToInt32(value);
                        propertyInfo.SetValue(model, convertType, null);
                    }
                    else if (type == typeof(decimal))
                    {
                        propertyInfo.SetValue(model, Convert.ToDecimal(value), null);
                    }
                    else if (type == typeof(DateTime))
                    {
                        propertyInfo.SetValue(model, Convert.ToDateTime(value), null);
                    }
                    else if (type == typeof(string))
                    {
                        propertyInfo.SetValue(model, value, null);
                    }
                }
            }
            return model;
        }

    }
}
