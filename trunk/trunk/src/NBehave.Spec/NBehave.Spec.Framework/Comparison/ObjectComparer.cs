using System;

namespace NBehave.Spec.Framework
{
    public class ObjectComparer : Comparer
    {
        internal ObjectComparer(object expected, object actual)
            : base(expected, actual)
        {
        }

        protected override bool DoComparesOK()
        {
            return ObjectsCompareOK(expected, actual);
        }

        private bool ObjectsCompareOK(object expectedObj, object actualObj)
        {
            if (expectedObj != null && actualObj != null)
            {
                if (expectedObj.GetType().IsArray && actualObj.GetType().IsArray)
                {
                    Array expectedArray = expectedObj as Array;
                    Array actualArray = actualObj as Array;

                    return ArraysCompareOK(expectedArray, actualArray);
                }
                else
                {
                    return expectedObj.Equals(actualObj);
                }
            }

            return expectedObj == actualObj;
        }

        private bool ArraysCompareOK(Array expectedArray, Array actualArray)
        {
            if (expectedArray.Length == actualArray.Length)
            {
                for (int i = 0; i < expectedArray.Length; i++)
                {
                    if (! ObjectsCompareOK(expectedArray.GetValue(i), actualArray.GetValue(i)))
                        return false;
                }

                return true;
            }

            return false;
        }
    }
}