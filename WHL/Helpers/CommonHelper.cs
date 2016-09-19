using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Reflection;
using System.IO;



namespace WHL.Helpers
{
    /// <summary>
    /// Common Helper: a tool static class which has many tool methods.
    /// </summary>
    public class CommonHelper
    {

        public const string USER_FILE_FOLDER = "FILES"; // the user files store folder

        /// <summary>
        /// get value from an object。
        /// </summary>
        /// <param name="obj">the whl entity object</param>
        /// <param name="fieldName">fileName: can with the point "." ,ex: product.name, it will auto set the value name in the product.</param>
        /// <returns></returns>
        public static string GetObjectValue(object obj, String fieldName)
        {
            try
            {
                Type Ts = obj.GetType();

                if (fieldName.IndexOf(".") > -1)
                {
                    String fieldName0 = (fieldName.Split('.'))[0];
                    String fieldName1 = (fieldName.Split('.'))[1];

                    obj = Ts.GetProperty(fieldName0).GetValue(obj, null);
                    fieldName = fieldName1;

                    return GetObjectValue(obj, fieldName);
                }

                object o = Ts.GetProperty(fieldName).GetValue(obj, null);
                string Value = Convert.ToString(o);
                if (string.IsNullOrEmpty(Value)) return null;
                return Value;
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// set an object value
        /// </summary>
        /// <param name="obj">the whl entity object</param>
        /// <param name="fieldName">file name </param>
        /// <param name="value">field value</param>
        public static void SetObjectValue(object obj, string fieldName, object value)
        {
            if (fieldName.IndexOf(".") > -1)//判断 是否多层: 这里先处理2层，Lion：以后要至少处理到第3层
            {   // product.sku
                String fieldName0 = (fieldName.Split('.'))[0];
                String fieldName1 = (fieldName.Split('.'))[1];

                PropertyInfo objProp = obj.GetType().GetProperty(fieldName0);
                Object subObj = System.Activator.CreateInstance(Type.GetType(objProp.PropertyType.FullName, true, true)); // product

                if (subObj != null)
                {
                    PropertyInfo subObjProp = subObj.GetType().GetProperty(fieldName1); // sku
                    subObjProp.SetValue(subObj, value, null);   // set "1234" to product.sku
                    objProp.SetValue(obj, subObj, null);        // set product to inventory
                }
            }else { // avail
                PropertyInfo objProp = obj.GetType().GetProperty(fieldName);
                objProp.SetValue(obj, value, null);
            }
        }

        /// <summary>
        /// Copy obj1 values to obj2, notice the obj1 and obj2 must have the same properties structure.
        /// </summary>
        /// <param name="srcObj"></param>
        /// <param name="tarObj"></param>
        public static void CopyObjectValue(dynamic srcObj , dynamic tarObj){
            
            foreach (System.Reflection.PropertyInfo p in tarObj.GetType().GetProperties())
            {
                string srcObjValue = GetObjectValue(srcObj, p.Name);
                SetObjectValue(tarObj, p.Name, srcObjValue);
            }
        
        }


        /// <summary>
        /// save a file by memory stream
        /// </summary>
        /// <param name="ms"></param>
        /// <param name="filePath"></param>
        public static void SaveFile(MemoryStream ms, String filePath) {
            FileStream fs = new FileStream(filePath, FileMode.OpenOrCreate);
            BinaryWriter w = new BinaryWriter(fs);
            w.Write(ms.ToArray());
            fs.Close();
            ms.Close();
        
        }

    }
}