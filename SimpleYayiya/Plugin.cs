using System;
using Lotlab.PluginCommon.FFXIV;
using Lotlab.PluginCommon.FFXIV.Parser;
using Advanced_Combat_Tracker;
using System.Windows.Forms;
using System.Runtime.InteropServices;

namespace SimpleYayiya
{
    public class Plugin : IActPluginV1
    {
        /// <summary>
        /// FFXIV 解析插件的引用
        /// </summary>
        ACTPluginProxy FFXIV { get; set; } = null;

        /// <summary>
        /// 网络包解析器
        /// </summary>
        NetworkParser parser { get; } = new NetworkParser(false);

        /// <summary>
        /// 状态标签的引用
        /// </summary>
        Label statusLabel = null;

        /// <summary>
        /// ACT插件接口 - 初始化插件
        /// </summary>
        /// <remarks>
        /// 我们在这里初始化整个插件
        /// </remarks>
        /// <param name="pluginScreenSpace">插件所在的Tab页面</param>
        /// <param name="pluginStatusText">插件列表的状态标签</param>
        public void InitPlugin(TabPage pluginScreenSpace, Label pluginStatusText)
        {
            // 设置状态标签引用方便后续使用
            statusLabel = pluginStatusText;

            // 遍历所有插件
            var plugins = ActGlobals.oFormActMain.ActPlugins;
            foreach (var item in plugins)
            {
                // 判断是否为FFXIV解析插件。在这里使用了开发公共库判断。
                // 你也可以使用以下方法，通过文件名判断是否为FFXIV解析插件。
                // if (item.pluginFile.Name.ToUpper().Contains("FFXIV_ACT_PLUGIN"))
                if (ACTPluginProxy.IsFFXIVPlugin(item.pluginObj))
                {
                    var obj = item.pluginObj;

                    // 如果你直接引用了解析插件，那么你可以直接使用下面这行代码拿到插件的实例
                    // var instance = obj as FFXIV_ACT_Plugin.FFXIV_ACT_Plugin;

                    // 在这里我们使用代理类来操作解析插件
                    FFXIV = new ACTPluginProxy(obj);

                    break;
                }
            }

            // 若解析插件不存在或不正常工作，则提醒用户，并结束初始化
            if (FFXIV == null || !FFXIV.PluginStarted)
            {
                statusLabel.Text = "FFXIV ACT Plugin is not working!";
                return;
            }

            // 注册网络事件
            FFXIV.DataSubscription.NetworkReceived += OnNetworkReceived;

            // 为了简便起见，我们不涉及任何UI相关的内容。
            // 如果你需要添加自己的页面，请自行编写UserControl。
            // pluginScreenSpace.Controls.Add(new YourUserControl());

            // 直接隐藏掉不需要显示的插件页面
            (pluginScreenSpace.Parent as TabControl).TabPages.Remove(pluginScreenSpace);

            // 更新状态标签的内容
            statusLabel.Text = "Plugin working!";
        }

        /// <summary>
        /// 收到网络数据包的回调
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="epoch"></param>
        /// <param name="message"></param>
        private void OnNetworkReceived(string connection, long epoch, byte[] message)
        {
            // 判断是否为目标数据包。
            // 一般来说，需要通过Opcode判断是否为目标数据包。但由于此数据包长度比较特殊，在这里为了简洁就直接使用长度作为判断依据。
            if (message.Length != Marshal.SizeOf<FFXIVIpcMarketBoardItemListing>()) return;

            // 解析数据包
            var packet = parser.ParseAsPacket<FFXIVIpcMarketBoardItemListing>(message);

            // 由于列表是从低到高排列的，我们要获取最低价，只需要第一个数据包即可。
            if (packet.listingIndexStart != 0) return;
            // 获取第一个售卖的信息
            var item = packet.listing[0];
            // 判断是否为空
            if (item.itemId == 0) return;

            // 获取并计算价格
            var price = item.pricePerUnit;
            var targetPrice = Math.Max(price - 1, 1);

            // 设置剪贴板内容
            ActGlobals.oFormActMain.Invoke(new Action(() => {
                Clipboard.SetText(targetPrice.ToString());
            }));
        }

        /// <summary>
        /// ACT插件接口 - 反初始化插件
        /// </summary>
        public void DeInitPlugin()
        {
            // 取消注册网络事件
            if (FFXIV != null)
            {
                FFXIV.DataSubscription.NetworkReceived -= OnNetworkReceived;
            }

            FFXIV = null;
            statusLabel.Text = "Plugin Exit!";
        }
    }
}


