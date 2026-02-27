using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class HelperUtilities
{
    /// <summary>
    /// 空字符串调试检查
    /// </summary>
    /// <returns></returns>
    public static bool ValidateCheckEmptyString(Object thisObject, string fieldName, string stringToCheck)
    {
        if (stringToCheck == "")
        {
            Debug.Log(fieldName + " 为空，必须在对象 " + thisObject.name.ToString() + " 中包含这个值！");
            return true;
        }

        return false;
    }

    /// <summary>
    /// 检查列表是否为空或包含空值，如果有错误，则返回true。
    /// </summary>
    /// <returns></returns>
    public static bool ValidateCheckEnumerableValues(Object thisObject, string fieldName, IEnumerable enumerableObjectToCheck)
    {
        bool error = false;
        int count = 0;

        foreach (var item in enumerableObjectToCheck)
        {
            if (item == null)
            {
                Debug.Log(fieldName + " 在对象 " + thisObject.name.ToString() + " 中含有空值！");
                error = true;
            }
            else
            {
                count++;
            }
        }

        if (count == 0)
        {
            Debug.Log(fieldName + " 在对象 " + thisObject.name.ToString() + " 中没有值！");
            error = true;
        }

        return error;
    }
}
