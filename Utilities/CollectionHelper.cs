using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.SqlClient;
using System.ComponentModel;
using System.Reflection;
using System.Globalization;
using Exceptionless;
using Exceptionless.Models;
namespace Utilities {
    public class CollectionHelper {
        private CollectionHelper() {
        }

        public static DataTable ConvertTo<T>(IList<T> list) {

            DataTable table = CreateTable<T>();
            Type entityType = typeof(T);
            PropertyDescriptorCollection properties = TypeDescriptor.GetProperties(entityType);

            foreach (T item in list) {
                DataRow row = table.NewRow();

                foreach (PropertyDescriptor prop in properties) {
                    row[prop.Name] = prop.GetValue(item);
                }

                table.Rows.Add(row);
            }

            return table;
        }
        //2
        public static IList<T> ConvertTo<T>(IList<DataRow> rows) {
            IList<T> list = null;

            if (rows != null) {
                list = new List<T>();

                foreach (DataRow row in rows) {
                    T item = CreateItem<T>(row);
                    list.Add(item);
                }
            }

            return list;
        }
        //1
        public static IList<T> ConvertTo<T>(DataTable table) {
            if (table == null) {
                return null;
            }

            List<DataRow> rows = new List<DataRow>();

            foreach (DataRow row in table.Rows) {
                rows.Add(row);
            }

            return ConvertTo<T>(rows);
        }
        //3
        public static T CreateItem<T>(DataRow row) {
            T obj = default(T);
            if (row != null) {
                obj = Activator.CreateInstance<T>();

                foreach (DataColumn column in row.Table.Columns) {
                    PropertyInfo prop = obj.GetType().GetProperty(column.ColumnName);
                    try {
                        //object value = (row[column.ColumnName] == DBNull.Value ? null : Convert.ChangeType(row[column.ColumnName], Nullable.GetUnderlyingType(prop.PropertyType)));

                        //object value = (row[column.ColumnName] == DBNull.Value ? null : row[column.ColumnName]);
                        if (prop != null) {
                            ParsePrimitive(prop, obj, row[column.ColumnName]);
                            //prop.SetValue(obj, value, null);
                        }

                    } catch (Exception ex) {
                           ex.ToExceptionless()
                           .SetMessage("Failed to create item.")
                           .AddTags("Collection Helper")
                           .MarkAsCritical()
                           .AddObject(prop,"Property")
                           .AddObject(obj,"Object")
                           .AddObject(row[column.ColumnName],"Column")
                           .Submit();
                        throw;
                    }
                }
            }

            return obj;
        }

        public static DataTable CreateTable<T>() {
            Type entityType = typeof(T);
            DataTable table = new DataTable(entityType.Name);
            PropertyDescriptorCollection properties = TypeDescriptor.GetProperties(entityType);

            foreach (PropertyDescriptor prop in properties) {
                table.Columns.Add(prop.Name, prop.PropertyType);
            }

            return table;
        }

        private static void ParsePrimitive(PropertyInfo prop, object entity, object value) {
            if (prop.PropertyType == typeof(string)) {
                prop.SetValue(entity, value.ToString().Trim(), null);
                } else if (prop.PropertyType == typeof(int) || prop.PropertyType == typeof(int?)) {
                if (value == null) {
                    prop.SetValue(entity,null,null);
                    } else {
                    prop.SetValue(entity,int.Parse(value.ToString()),null);
                    }
                }
            else if (prop.PropertyType == typeof(UInt32) || prop.PropertyType == typeof(UInt32?))
            {
                if (value == null||value==DBNull.Value)
                {
                    prop.SetValue(entity, null, null);
                }
                else
                {
                    prop.SetValue(entity, UInt32.Parse(value.ToString()), null);
                }
            }
            else if (prop.PropertyType == typeof(DateTime) || prop.PropertyType == typeof(Nullable<DateTime>)) {
                DateTime date;
                bool isValid = DateTime.TryParse(value.ToString(), out date);
                if (isValid) {
                    prop.SetValue(entity, date, null);
                } else {
                    //Making an assumption here about the format of dates in the source data.
                    isValid = DateTime.TryParseExact(value.ToString(), "yyyy-MM-dd", new CultureInfo("en-US"), DateTimeStyles.AssumeUniversal, out date);
                    if (isValid) {
                        prop.SetValue(entity, date, null);
                    }
                }
            } else if (prop.PropertyType == typeof(decimal)) {
                prop.SetValue(entity, decimal.Parse(value.ToString()), null);
            } else if (prop.PropertyType == typeof(bool)) {
             
                prop.SetValue(entity,bool.Parse(value.ToString()),null);
                } else if (prop.PropertyType == typeof(double) || prop.PropertyType == typeof(double?)) {
                double number;
                bool isValid = double.TryParse(value.ToString(), out number);
                if (isValid) {
                    prop.SetValue(entity, double.Parse(value.ToString()), null);
                }
            }
        }

    }
}