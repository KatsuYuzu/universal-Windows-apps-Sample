using Microsoft.ServiceBus.Notifications;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace 通知送信
{
    class Program
    {
        static void Main(string[] args)
        {
            SendNotificationAsync();
            Console.ReadLine();
        }

        private static async Task SendNotificationAsync()
        {
            // トースト通知用文字列
            const string toastFormat = @"<toast><visual><binding template=""ToastText01""><text id=""1"">{0}</text></binding></visual></toast>";

            // 接続文字列（DefaultFullSharedAccessSignature）
            // ハブ名
            var hub = NotificationHubClient.CreateClientFromConnectionString(
                "Endpoint=sb://<hub name>.servicebus.windows.net/;SharedAccessKeyName=DefaultFullSharedAccessSignature;SharedAccessKey=<key>",
                "<hub name>");

            // 通知
            var toast = string.Format(toastFormat, "すべての購読者宛の通知");
            await hub.SendWindowsNativeNotificationAsync(toast);

            // 通知の対象を制限する際の購読例
            var tags = new HashSet<string>();
            tags.Add("group1");

            var toast1 = string.Format(toastFormat, "group1の購読者宛の通知");
            await hub.SendWindowsNativeNotificationAsync(toast1, tags);

            tags.Clear();
            tags.Add("group2");

            var toast2 = string.Format(toastFormat, "group2の購読者宛の通知");
            await hub.SendWindowsNativeNotificationAsync(toast2, tags);
        }
    }
}
