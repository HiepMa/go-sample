using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using demoTail.Utils;

namespace demoTail.Service
{
    struct RTPPacket
    {
        // RTP Packet
        /**
         * Bit_Offset |0|1|2|3|4-7|8| 9-11 | 12-15 | 16-19 | 20-23 | 24-27 | 28-31 |
         *  0         | V |P|X|CC-|M| Payload Type | Sequence number --------------|
         *  32        |                          TimeStamp                         |
         *  64        |                      SSRC identifier                       |
         *  96        |                      CSRC identifier                       |
         *  
         */

        /**
        * Create the RTP header with extension for this call. <br>
        * <pre>
        * offset|0|1|2|3|  4-7  |8| 9-11| 12-15 | 16-19 | 20-23 | 24-27 | 28-31 |
        *  0    | V |P|X| CC ---|M| Payload type| Sequence number --------------|
        *  32   | Timestamp ----------------------------------------------------|
        *  64   | SSRC identifier ----------------------------------------------|
        *  96   | Extension header ID ----------| Extension header length ------|
        *  128  | reserved -----| Called MPT1327 address -----------------------|
        *  160  | reserved -----| Caller MPT1327 address -----------------------|
        *  192  | reserved -----| Talker MPT1327 address -----------------------|
        *  224  | Source channel -----------------------------------------------|
        *  256  | Type -| State | Call flags ---| RSSI ---------| BER or SINAD -|
        *  288  | UUID 0-31 ----------------------------------------------------|
        *  320  | UUID 32-63 ---------------------------------------------------|
        *  352  | UUID 64-95 ---------------------------------------------------|
        *  384  | UUID 96-127 --------------------------------------------------|
        *  416  | reserved ---------------------| Encrypt.method| Key ID -------|
        *  448  | Initialization vector ----------------------------------------|
        * </pre>
        * @param info - a structure containing required call info
        * @param port - target port
        * @param debug - log extended debug information
        */
        public const int RtpHeaderSize = 12;
        public const int RtpProtocolVersion = 2;

        public int ProtocolVersion { get; private set; }
        public bool PaddingFlag { get; private set; }
        public bool ExtensionFlag { get; private set; }
        public int CsrcCount { get; private set; }
        public bool MarkerBit { get; private set; }
        public int PayloadType { get; private set; }
        public ushort SeqNumber { get; private set; }
        public uint Timestamp { get; private set; }
        public uint SyncSourceId { get; private set; }
        public int ExtensionHeaderId { get; private set; }

        public ArraySegment<byte> PayloadSegment { get; set; }

        internal RTPPacket(ushort seqNumber,ArraySegment<byte> payloadSegment)
        {
            ProtocolVersion = 1;
            PaddingFlag = false;
            ExtensionFlag = false;
            CsrcCount = 0;
            MarkerBit = false;
            PayloadType = 0;
            SeqNumber = 0;
            Timestamp = 0;
            SyncSourceId = 0;
            ExtensionHeaderId = 0;
            SeqNumber = seqNumber;
            PayloadSegment = payloadSegment;
        }
        public static bool TryParse(ArraySegment<byte> byteSegment, out RTPPacket rtpPacket)
        {
            rtpPacket = new RTPPacket();

            Debug.Assert(byteSegment.Array != null, "byteSegment.Array != null");

            if (byteSegment.Count < RtpHeaderSize)
                return false;

            int offset = byteSegment.Offset;
            rtpPacket.ProtocolVersion = byteSegment.Array[offset] >> 6;

            if (rtpPacket.ProtocolVersion != RtpProtocolVersion)
                return false;

            rtpPacket.PaddingFlag = (byteSegment.Array[offset] >> 5 & 1) != 0;
            rtpPacket.ExtensionFlag = (byteSegment.Array[offset] >> 4 & 1) != 0;
            rtpPacket.CsrcCount = byteSegment.Array[offset++] & 0xF;

            rtpPacket.MarkerBit = byteSegment.Array[offset] >> 7 != 0;
            rtpPacket.PayloadType = byteSegment.Array[offset++] & 0x7F;

            rtpPacket.SeqNumber = (ushort)BigEndianConverter.ReadUInt16(byteSegment.Array, offset);
            offset += 2;

            rtpPacket.Timestamp = BigEndianConverter.ReadUInt32(byteSegment.Array, offset);
            offset += 4;

            rtpPacket.SyncSourceId = BigEndianConverter.ReadUInt32(byteSegment.Array, offset);
            offset += 4 + 4 * rtpPacket.CsrcCount;

            if (rtpPacket.ExtensionFlag)
            {
                rtpPacket.ExtensionHeaderId = BigEndianConverter.ReadUInt16(byteSegment.Array, offset);
                offset += 2;

                int extensionHeaderLength = BigEndianConverter.ReadUInt16(byteSegment.Array, offset) * 4;
                offset += 2 + extensionHeaderLength;
            }

            int payloadSize = byteSegment.Offset + byteSegment.Count - offset;

            if (rtpPacket.PaddingFlag)
            {
                int paddingBytes = byteSegment.Array[byteSegment.Offset + byteSegment.Count - 1];
                payloadSize -= paddingBytes;
            }

            rtpPacket.PayloadSegment = new ArraySegment<byte>(byteSegment.Array, offset, payloadSize);
            return true;
        }
    }
}
