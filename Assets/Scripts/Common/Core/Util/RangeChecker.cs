using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace seiko.utilities
{
    public class RangeChecker
    {
        public static bool IsInSphere(Vector3 origin, float radius, Vector3 targetPos)
        {
            var sum = 0f;
            for (var i = 0; i < 3; i++)
                sum += Mathf.Pow(origin[i] - targetPos[i], 2);
            return sum <= Mathf.Pow(radius, 2f);
        }

        public static bool IsInCircle(Vector2 origin, float radius, Vector2 targetPos)
        {
            var sum = 0f;
            for (var i = 0; i < 2; i++)
                sum += Mathf.Pow(origin[i] - targetPos[i], 2);
            return sum <= Mathf.Pow(radius, 2f);
        }

        public static bool IsRangePartiallyInsaideRange(Vector2 targetMin, Vector2 targetMax, Vector2 originMin, Vector2 originMax)
        {
            bool result = false;
            if (IsRangeInRange(targetMin.y, targetMax.y, originMin.y, originMax.y))
            {
                if (IsRangeInRange(targetMin.x, targetMax.x, originMin.x, originMax.x))
                {
                    result = true;
                }
            }

            return result;
        }

        public static bool IsRangeInRange(float targetFrom, float targetTo, float rangeFrom, float rangeTo)
        {
            var result = false;
            var level0 = rangeFrom <= targetTo;
            // Debug.Log("level0: " + level0);
            if (level0)
            {
                var level1 = targetFrom <= rangeTo;
                // Debug.Log("level1: " + level1);
                if (level1)
                {
                    result = true;
                }
            }
            // var result = rangeFrom <= targetTo && targetFrom <= rangeTo;
            // Debug.Log("IsRangeInRange_Result: " + result);
            return result;
        }

        public static bool IsPositionInsideCollider(Vector3 position, BoxCollider dangerObj)
        {
            if (dangerObj.bounds.Contains(position))
            {
                return true;
            }
            return false;
        }

        public static bool IsPointInRange(float target, float from, float to)
        {
            var result = from <= target && target <= to;
            return result;
        }
    }
}