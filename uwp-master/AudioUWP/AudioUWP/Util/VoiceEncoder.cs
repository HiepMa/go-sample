using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AudioUWP.Util
{
    class VoiceEncoder
    {
        //bool haveFirstPacket = false;
        //short expectedSequenceNumber = 0;
        //short currentSequenceNumber = 0;
        //bool haveData = false;
        //private bool debug;
        ////SpeechCall.RxInfo info;

        //static readonly byte G711PayloadType = 0;

        ////Voice Data Block Type
        //public enum VDBT
        //{
        //    INVALID,
        //    G711,
        //    DATA,
        //    VOICE,
        //    DMRSTREAMINFO,
        //    MPTSTREAMINFO,
        //    IPDATA,
        //    IRDATAREPORT,
        //    EMBEDDED,
        //    PING;
        //};

        ///**
        // * @param debug - log extended debug information
        // */
        //VoiceEncoder(bool debug)
        //{
        //    this.debug = debug;
        //}

        ///**
        // * Unpack RTP voice packet with a header with extension and 20 ms voice data. <br>
        // * <pre>
        // * offset|0|1|2|3|  4-7  |8| 9-11| 12-15 | 16-19 | 20-23 | 24-27 | 28-31 |
        // *  0    | V |P|X| CC ---|M| Payload type| Sequence number --------------|
        // *  32   | Timestamp ----------------------------------------------------|
        // *  64   | SSRC identifier ----------------------------------------------|
        // *  96   | Extension header ID ----------| Extension header length ------|
        // *  128  | reserved -----| Called MPT1327 address -----------------------|
        // *  160  | reserved -----| Caller MPT1327 address -----------------------|
        // *  192  | reserved -----| Talker MPT1327 address -----------------------|
        // *  224  | Source channel -----------------------------------------------|
        // *  256  | Type -| State | Call flags ---| RSSI ---------| BER or SINAD -|
        // *  288  | UUID 0-31 ----------------------------------------------------|
        // *  320  | UUID 32-63 ---------------------------------------------------|
        // *  352  | UUID 64-95 ---------------------------------------------------|
        // *  384  | UUID 96-127 --------------------------------------------------|
        // *  416  | reserved ---------------------| Encrypt.method| Key ID -------|
        // *  448  | Initialization vector ----------------------------------------|
        // * </pre>
        // * @param contents - the incoming datagram (wrapped in a ByteBuffer) from the UDP socket
        // * @param voiceBuffer - an array containing the decoded raw audio
        // * @return true if packet was successfully processed
        // */
        //bool decodePacket(MemoryStream contents, byte[] voiceBuffer)
        //{
        //    if (debug)
        //    {
        //        StringBuilder sb = new StringBuilder(720);
        //        contents.position(0);
        //        for (int i = 0; i < SpeechCall.DATAGRAM_LENGTH; i++)
        //        {
        //            sb.Append(Integer.toHexString(contents.get() & 0xff));
        //            sb.Append(i % 2 == 1 ? ':' : '.');
        //        }
        //    }

        //    try
        //    {
        //        contents.position(0);
        //        byte rtpVersion = contents.get(); // Ensure these become unsigned byte values.
        //        byte payloadType = contents.get();

        //        if ((rtpVersion & 0xc0) != 0x80 || (payloadType & 0x3) != G711PayloadType)
        //        {
        //            //VoiceUtil.trace("rx: packet with wrong rtp-version=" + Integer.toHexString(rtpVersion & 0xff)
        //                  //+ " or payload-type=" + Integer.toHexString(payloadType & 0xff));
        //            return false;
        //        }
        //        currentSequenceNumber = contents.getShort();
        //        if (!haveFirstPacket)
        //        {
        //            expectedSequenceNumber = currentSequenceNumber;
        //            haveFirstPacket = true;
        //        }
        //        if ((rtpVersion & 0x10) == 0x10) // Extension header set, assume it is what we expect
        //        {
        //            contents.position(12); // Move to start of extension info block, check for block type.
        //            short extension = contents.getShort();
        //            if ((extension & (short)0xff00) != (short)0xA000)
        //            {
        //                //VoiceUtil.trace("rx: packet with wrong rtp-extension=" + Integer.toHexString(extension & 0xffff));
        //                return false;
        //            }
        //            /*contents.position(16);
        //            info.caller = contents.getInt();
        //            info.called = contents.getInt();
        //            info.talker = contents.getInt();
        //            info.sourceChannel = contents.getInt();
        //            info.info = contents.getInt();
        //            info.uuid1 = contents.getInt();
        //            info.uuid2 = contents.getInt();
        //            info.uuid3 = contents.getInt();
        //            info.uuid4 = contents.getInt();*/
        //            try
        //            {
        //                contents.position(60); // Move to start of G711 voice block.
        //            }
        //            catch (Exception ex)
        //            {
        //                // packet with no voice data, ignore
        //                return true;
        //            }
        //        }
        //        else // No extension header
        //        {
        //            try
        //            {
        //                contents.position(12); // Move to start of G711 voice block.
        //            }
        //            catch (Exception ex)
        //            {
        //                // packet with no voice data, ignore
        //                return true;
        //            }
        //        }
        //    }
        //    catch (Exception e)
        //    {
        //        //VoiceUtil.trace("rx: wrong packet header length of " + contents.array().length);
        //        return false;
        //    }

        //    G711Codec.decodeULaw(contents, voiceBuffer, voiceBuffer.Length);
        //    return true;
        //}
    }
}
