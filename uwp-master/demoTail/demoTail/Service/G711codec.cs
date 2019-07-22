using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Security.Cryptography;
using Windows.Storage.Streams;

namespace demoTail.Service
{
    public class G711codec
    {
        static readonly int BIAS = 0x84; //132 or 1000 0100
        public const int MAX = 32635; //32767 (max 15-bit integer) minus BIAS

        private static short[] seg_end = new short[] { 0xFF, 0x1FF, 0x3FF, 0x7FF, 0xFFF, 0x1FFF, 0x3FFF, 0x7FFF };

        private static byte[] muEncodeTable;

        private static readonly short[] muDecodeTable = new short[] {
          -32124, -31100, -30076, -29052, -28028, -27004, -25980, -24956, -23932, -22908,
          -21884, -20860, -19836, -18812, -17788, -16764, -15996, -15484, -14972, -14460,
          -13948, -13436, -12924, -12412, -11900, -11388, -10876, -10364, -9852, -9340,
          -8828, -8316, -7932, -7676, -7420, -7164, -6908, -6652, -6396, -6140,
          -5884, -5628, -5372, -5116, -4860, -4604, -4348, -4092, -3900, -3772,

          -3644, -3516, -3388, -3260, -3132, -3004, -2876, -2748, -2620, -2492,
          -2364, -2236, -2108, -1980, -1884, -1820, -1756, -1692, -1628, -1564,
          -1500, -1436, -1372, -1308, -1244, -1180, -1116, -1052, -988, -924,
          -876, -844, -812, -780, -748, -716, -684, -652, -620, -588,
          -556, -524, -492, -460, -428, -396, -372, -356, -340, -324,

          -308, -292, -276, -260, -244, -228, -212, -196, -180, -164,
          -148, -132, -120, -112, -104, -96, -88, -80, -72, -64,
          -56, -48, -40, -32, -24, -16, -8, -1, 32124, 31100,
          30076, 29052, 28028, 27004, 25980, 24956, 23932, 22908, 21884, 20860,
          19836, 18812, 17788, 16764, 15996, 15484, 14972, 14460, 13948, 13436,

          12924, 12412, 11900, 11388, 10876, 10364, 9852, 9340, 8828, 8316,
          7932, 7676, 7420, 7164, 6908, 6652, 6396, 6140, 5884, 5628,
          5372, 5116, 4860, 4604, 4348, 4092, 3900, 3772, 3644, 3516,
          3388, 3260, 3132, 3004, 2876, 2748, 2620, 2492, 2364, 2236,
          2108, 1980, 1884, 1820, 1756, 1692, 1628, 1564, 1500, 1436,

          1372, 1308, 1244, 1180, 1116, 1052, 988, 924, 876, 844,
          812, 780, 748, 716, 684, 652, 620, 588, 556, 524,
          492, 460, 428, 396, 372, 356, 340, 324, 308, 292,
          276, 260, 244, 228, 212, 196, 180, 164, 148, 132,
          120, 112, 104, 96, 88, 80, 72, 64, 56, 48,
          40, 32, 24, 16, 8, 0 };
        static void encodeULaw(byte[] outData, int outOffset, int length, short[] inputBuffer)
        {
            for (int i = 0; i < length; i++)
            {
                outData[i + outOffset] = muEncodeTable[inputBuffer[i] & 0xFFFF];
            }
        }

        static short encodeULawWithAverageLevel(byte[] outData, int outOffset, int length, short[] inputBuffer)
        {
            double average = 0.0;
            for (int i = 0; i < length; i++)
            {
                outData[i + outOffset] = muEncodeTable[inputBuffer[i] & 0xFFFF];
                average += Math.Pow(inputBuffer[i], 2);
            }
            average /= length;
            average = Math.Sqrt(average);
            return (short)average;
        }
        /**
        * Encode 50 ms of 16 bit little endian raw audio sampled at 8 kHz to G.711 mu law. <br>
        * <br>
        * Note that the supplied raw audio is packed into a byte array, not a short array. <br>
        * It is more efficient to repack the samples from byte to short here than in the calling method.
        * @param inData - a byte array containing the raw audio to encode
        * @param outData - the byte buffer object to insert the encoded voice data in. <br>
        * Note that outdata.position() must be set to the correct location before calling this method
        * @param length - the length of valid data in the raw audio array
        */

        static void encodeULaw(byte[] inData, MemoryStream outData, int length)
        {
            // m must be an int, not a short, else you get negative array indexes!
            // I wish Java had unsigned data types!
            int m;
            for (int i = 0; i < length; i += 2)
            {
                // & 0xffff emulates an unsigned short (required for the array index)
                // Note: raw data is little endian
                m = ((inData[i + 1] << 8) + (inData[i] & 0xff)) & 0xffff;
                outData.WriteByte(muEncodeTable[m]);
            }
        }

        static void decodeULaw(byte[] data, int offset, int length, short[] outputBuffer)
        {
            for (int i = 0; i < length; i++)
            {
                outputBuffer[i] = muDecodeTable[data[i + offset] & 0xFF];
            }
        }

        static short decodeULawWithAverageLevel(byte[] data, int offset, int length, short[] outputBuffer)
        {
            double average = 0.0;
            for (int i = 0; i < length; i++)
            {
                outputBuffer[i] = muDecodeTable[data[i + offset] & 0xFF];
                average += Math.Pow(outputBuffer[i], 2);
            }
            average /= length;
            average = Math.Sqrt(average);
            return (short)average;
        }

        /**
        * Decode 50 ms of G.711 mu law to 16 bit little endian raw audio played at 8 kHz. <br>
        * <br>
        * Note that the decoded raw audio is packed into a byte array, not a short array. <br>
        * It is more efficient to repack the samples from short to byte here than in the calling method.
        * @param inData - the byte buffer object containing the encoded voice data
        * Note that indata.position() must be set to the correct location before calling this method
        * @param outData - a byte array to put the decoded raw audio in
        * @param length - the length of valid data in the raw audio array
        */

        static void decodeULaw(MemoryStream inData, byte[] outData, int length)
        {
            short m, n;
            for (int i = 0; i < length; i += 2)
            {
                try
                {
                    // (short) & 0xff emulates an unsigned byte (required for the array index)
                    n =(short) inData.ReadByte();
                    m = muDecodeTable[n & 0xff];
                    // make raw data little endian
                    outData[i + 1] = (byte)((int)((uint)m >> 8));
                    outData[i] = (byte)(m & 0xff);
                }
                catch (Exception bue)
                {
                    // handle a packet with less than 20ms data
                    return;
                }
            }
        }
        private static byte linear2ulaw(int pcm_val) /* 2's complement (16-bit range) */
        {
            int mask;
            int seg;
            byte uval;

            // if someone has passed in a bum short (e.g. they manipulated an int
            // rather than a short, so the value looks positive, not negative) fix that
            if ((pcm_val & 0x8000) != 0)
            {
                pcm_val = -(~pcm_val & 0x7fff) - 1;
            }

            // Get the sign and the magnitude of the value
            if (pcm_val < 0)
            {
                pcm_val = BIAS - pcm_val;
                mask = 0x7F;
            }
            else
            {
                pcm_val += BIAS;
                mask = 0xFF;
            }

            // Convert the scaled magnitude to segment number
            seg = search(pcm_val, seg_end, 8);

            // Combine the sign, segment, quantization bits; and complement the code word
            if (seg >= 8) // out of range, return maximum value
            {
                return (byte)(0x7F ^ mask);
            }
            uval = (byte)((seg << 4) | ((pcm_val >> (seg + 3)) & 0xF));
            return (byte)(uval ^ mask);
        }

        private static int search(int val, short[] table, int size)
        {
            int i;

            for (i = 0; i < size; i++)
            {
                if (val <= table[i])
                    return (i);
            }
            return (size);
        }
        static void startup()
        {
            muEncodeTable = new byte[(short.MaxValue - short.MinValue) + 1];
            for (int i = short.MinValue; i <= short.MaxValue; i++)
            {
                muEncodeTable[i & 0xFFFF] = linear2ulaw(i);
            }
        }
    }
}
