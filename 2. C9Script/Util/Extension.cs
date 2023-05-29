using System;
using GameData;
using LitJson;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.UI;
using Object = UnityEngine.Object;

public static class Extension
{
    public static T GetOrAddComponent<T>(this GameObject gameObject) where T : Component
    {
        T component = gameObject.GetComponent<T>();

        if (component == null)
            component = gameObject.AddComponent<T>();

        return component;
    }

    public static void BindEvent(this Button btn, Action action, UIEvent type = UIEvent.Click)
    {
        UI_Base.BindEvent(btn.gameObject, action, type);
    }
    
    public static void SetScrollRect(this Button btn, ScrollRect scrollRect)
    {
        UI_Base.SetScrollRect(btn.gameObject, scrollRect);
    }

    public static void BindEvent(this GameObject go, Action action, UIEvent type = UIEvent.Click)
    {
        UI_Base.BindEvent(go, action, type);
    }

    public static void ClearEvent(this Button btn, UIEvent type = UIEvent.Click)
    {
        UI_Base.ClearEvent(btn.gameObject, type);
    }

    public static void DestroyInChildren(this Transform tr)
    {
        for (int i = 0; i < tr.childCount; i++)
        {
            Transform childTr = tr.GetChild(i);

            if (childTr == null)
                continue;

            Object.Destroy(childTr.gameObject);
        }
    }

    public static bool IsEndCurrentAnimation(this Animator animator)
    {
        return animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1.0f;
    }

    public static bool ContainsHashKey(this Animator animator, int hash)
    {
        for (int i = 0; i < animator.parameters.Length; i++)
        {
            if (animator.parameters[i].nameHash == hash)
                return true;
        }

        return false;
    }

    public static string ToJson(this PetData petData)
    {
        return JsonConvert.SerializeObject(petData);
    }
    public static string ToJson(this WorldWoodData woodData)
    {
        return JsonConvert.SerializeObject(woodData);
    }
    public static T ToData<T>(this JsonData jsonData)
    {
        return JsonConvert.DeserializeObject<T>(jsonData.ToString());
    }

    public static string ToJson(this BaseLog baseLog)
    {
        return JsonConvert.SerializeObject(baseLog);
    }

    public static DateTime ToKstTime(this DateTime dateTime)
    {
        var time = TimeZoneInfo.ConvertTimeToUtc(dateTime);
        time = time.AddHours(9);
        return new DateTime(
            time.Year,
            time.Month,
            time.Day,
            time.Hour,
            time.Minute,
            time.Second);
    }
}