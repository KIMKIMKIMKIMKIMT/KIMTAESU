using System.Collections.Generic;
using AppsFlyerSDK;
using Firebase.Analytics;

public static class InAppActivity
{
    public static void SendEvent(string eventName)
    {
        var dic = new Dictionary<string, string>
        {
            ["C"] = eventName
        };
        AppsFlyer.sendEvent(eventName, dic);
        FirebaseAnalytics.LogEvent(eventName);
    }

    public static void SendLvEvent(int lv)
    {
        switch (lv)
        {
            case 60:
            case 70:
            case 80:
            case 90:
            case 100:
            case 150:
            case 200:
            case 250:
            case 300:
            case 350:
            case 400:
            case 450:
            case 500:
            case 550:
            case 600:
            case 650:
            case 700:
            case 750:
            case 800:
            case 850:
            case 900:
            case 950:
            case 999:
                AppsFlyer.sendEvent($"level_{lv}", new Dictionary<string, string>(){{"C", $"level_{lv}"}});
                FirebaseAnalytics.LogEvent($"level_{lv}");
                break;
        }
    }

    public static void SendStageEvent(int stage)
    {
        switch (stage)
        {
            case 50:
            case 100:
            case 300:
            case 500:
            case 700:
            case 1000:
            case 1500:
            case 2000:
            case 2500:
            case 3000:
            case 3500:
            case 4000:
            case 4500:
            case 5000:
            case 5500:
            case 6000:
            case 6500:
            case 7000:
            case 7500:
            case 8000:
            case 8500:
            case 9000:
            case 9500:
            case 10000:
                AppsFlyer.sendEvent($"stage_{stage}", new Dictionary<string, string>(){{"C", $"stage_{stage}"}});
                FirebaseAnalytics.LogEvent($"stage_{stage}");
                break;
        }
    }

    public static void SendRetentionEvent(int retentionIndex)
    {
        switch (retentionIndex)
        {
            case 1:
            case 2:
            case 3:
            case 4:
            case 5:
            case 6:
            case 7:
            case 15:
            case 30:
                AppsFlyer.sendEvent($"RET.U_{retentionIndex}DAY", new Dictionary<string, string>(){{"C", $"RET.U_{retentionIndex}DAY"}});
                FirebaseAnalytics.LogEvent($"RET.U_{retentionIndex}DAY");
                break;
        }
    }
}