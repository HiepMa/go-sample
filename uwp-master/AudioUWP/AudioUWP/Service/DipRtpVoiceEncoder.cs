using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AudioUWP.Service
{
    class DipRtpVoiceEncoder
    {
        //    byte rssi = 0;
        //    byte sinad = 0;

        //    private int overCount = 0;
        //    int sequenceNo;
        //    MemoryStream packet;
        //    private bool _debug;

        //    //Voice Data Block Type
        //    public enum VDBT
        //    {
        //        INVALID,
        //        G711,
        //        DATA,
        //        VOICE,
        //        DMRSTREAMINFO,
        //        MPTSTREAMINFO,
        //        IPDATA,
        //        IRDATAREPORT,
        //        EMBEDDED,
        //        PING
        //    };

        //    /**
        //     * Create the RTP header with extension for this call. <br>
        //     * <pre>
        //     * offset|0|1|2|3|  4-7  |8| 9-11| 12-15 | 16-19 | 20-23 | 24-27 | 28-31 |
        //     *  0    | V |P|X| CC ---|M| Payload type| Sequence number --------------|
        //     *  32   | Timestamp ----------------------------------------------------|
        //     *  64   | SSRC identifier ----------------------------------------------|
        //     *  96   | Extension header ID ----------| Extension header length ------|
        //     *  128  | reserved -----| Called MPT1327 address -----------------------|
        //     *  160  | reserved -----| Caller MPT1327 address -----------------------|
        //     *  192  | reserved -----| Talker MPT1327 address -----------------------|
        //     *  224  | Source channel -----------------------------------------------|
        //     *  256  | Type -| State | Call flags ---| RSSI ---------| BER or SINAD -|
        //     *  288  | UUID 0-31 ----------------------------------------------------|
        //     *  320  | UUID 32-63 ---------------------------------------------------|
        //     *  352  | UUID 64-95 ---------------------------------------------------|
        //     *  384  | UUID 96-127 --------------------------------------------------|
        //     *  416  | reserved ---------------------| Encrypt.method| Key ID -------|
        //     *  448  | Initialization vector ----------------------------------------|
        //     * </pre>
        //     * @param info - a structure containing required call info
        //     * @param port - target port
        //     * @param debug - log extended debug information
        //     */
        //    DipRtpVoiceEncoder(SpeechCall.TxInfo info, int port, bool debug)
        //    {
        //        this._debug = debug;

        //        // initialise the sequence number
        //        Random r = new Random();
        //        sequenceNo = r.Next();

        //        packet = new MemoryStream(220)
        //        {
        //            Position = 0
        //        };
        //        BinaryWriter writer = new BinaryWriter(packet);

        //        writer.Write(0x90000000);       // version << 30 + extension << 28 . Sequence number is added later
        //        writer.Write(0);                // timestamp is added later
        //        writer.Write((int)(VoiceController.getInstance().sourceChannel << 6) + (port % 8) + (overCount % 8)); // ssrc
        //        writer.Write(0xA001000B);       // profile extension header << 16 + extension header length
        //        writer.Write(info.called);
        //        writer.Write(info.caller);
        //        writer.Write(((MptTrunkAddress)LDT.getMyMptAddress()).hashCode());
        //        writer.Write((int)(VoiceController.getInstance().sourceChannel));
        //        // the node sets the state field if required. RSSI and SINAD are added later
        //        writer.Write((info.group == true ? 1 << 28 : 0) + (info.emergency == true ? 1 << 23 : 0)
        //              + (info.broadcast == true ? 1 << 19 : 0) + (info.priority == true ? 1 << 16 : 0));
        //        writer.Write(0);                // no encryption is used
        //        writer.Write(0);
        //        writer.Write(0);                // the node adds the UUID
        //        writer.Write(0);
        //        writer.Write(0);
        //        writer.Write(0);
        //    }

        //    /**
        //     * Always call this method before sending the first packet of an "over".
        //     */
        //    void incrementOverCount()
        //    {
        //        VoiceUtil.trace("increment over count = " + overCount);
        //        overCount++;
        //    }

        //    /**
        //     * Create an RTP voice packet with a header and 20 ms voice data.
        //     * @param rawAudio - an array containing the raw audio to encode rawAudio
        //     * @param length - the length of valid data in the raw audio array
        //     * @return - the packet as a byte array, ready to send
        //     */
        //    byte[] createPacket(byte[] rawAudio, int length)
        //    {
        //        long now = System.currentTimeMillis();

        //        packet.position(2);
        //        packet.putShort((short)(++sequenceNo & 0xffff));
        //        packet.putInt((int)(now & 0xffffffff));
        //        packet.position(30);
        //        packet.put(rssi);
        //        packet.put(sinad);
        //        packet.position(60);
        //        G711Codec.encodeULaw(rawAudio, packet, length);
        //        byte[] contents = new byte[SpeechCall.DATAGRAM_LENGTH];
        //        System.arraycopy(packet.array(), 0, contents, 0, SpeechCall.DATAGRAM_LENGTH);
        //        if (debug)
        //        {
        //            StringBuilder sb = new StringBuilder(720);
        //            for (int i = 0; i < contents.length; i++)
        //            {
        //                sb.append(Integer.toHexString(contents[i] & 0xff));
        //                sb.append(i % 2 == 1 ? ':' : '.');
        //            }
        //            VoiceUtil.trace("tx=" + sb);
        //        }
        //        return contents;
        //    }
    }
}
