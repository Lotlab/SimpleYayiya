using System.Runtime.InteropServices;
using Lotlab.PluginCommon.FFXIV.Parser;

// 这些是类型别名，方便直接复制c++的数据结构，不需要替换数据类型
using uint64_t = System.UInt64;
using int64_t = System.Int64;
using uint32_t = System.UInt32;
using int32_t = System.Int32;
using uint16_t = System.UInt16;
using int16_t = System.Int16;
using uint8_t = System.Byte;
using int8_t = System.Byte;

namespace SimpleYayiya
{
    /// <summary>
    /// 市场布告板 - 正在出售列表
    /// </summary>
    /// <remarks>
    /// 这个数据结构是直接从Sapphire中复制的，对其做了一些修改以支持C#的语法。
    /// </remarks>
    /// <see cref="https://github.com/SapphireServer/Sapphire/blob/master/src/common/Network/PacketDef/Zone/ServerZoneDef.h"/>
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    struct FFXIVIpcMarketBoardItemListing
    {
        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct ItemListing // 152 bytes each
        {
            public uint64_t listingId;
            public uint64_t retainerId;
            public uint64_t retainerOwnerId;
            public uint64_t artisanId;
            public uint32_t pricePerUnit;
            public uint32_t totalTax;
            public uint32_t itemQuantity;
            public uint32_t itemId;
            public uint16_t lastReviewTime;
            public uint16_t containerId;
            public uint32_t slotId;
            public uint16_t durability;
            public uint16_t spiritBond;
            /**
             * auto materiaId = (i & 0xFF0) >> 4;
             * auto index = i & 0xF;
             * auto leftover = i >> 8;
             */
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 5)]
            public uint16_t[] materiaValue;
            public uint16_t padding1;
            public uint32_t padding2;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 32)]
            public char[] retainerName;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 32)]
            public char[] playerName;
            public byte hq; // 注意：C# 的 bool 型大小为4，在这里换成 byte
            public uint8_t materiaCount;
            public uint8_t onMannequin;
            public uint8_t marketCity;
            public uint16_t dyeId;
            public uint16_t padding3;
            public uint32_t padding4;
        }

        public IPCHeader ipc; // IPC 包头

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 10)]
        public ItemListing[] listing; // Multiple packets are sent if there are more than 10 search results.

        public uint8_t listingIndexEnd;
        public uint8_t listingIndexStart;
        public uint16_t requestId;
        public uint32_t padding;
    };
}