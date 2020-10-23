using System;
using Urho3DNet;

namespace Salamandr.utils.Network
{

    public class GameBuffer
    {
        int read;
        int write;
        public byte[] bytes;

        public GameBuffer(int value)
        {
            this.allocate(value);
        }

        public GameBuffer()
        {
            this.allocate(256);
        }

        public GameBuffer allocate( int value )
        {
            bytes = new byte[value];
            write = 0;
            read = 0;
            return this;
        }

        public void copyFrom(byte[] bb)
        {
            bytes = bb;
        }

        public int getLength()
        {
            if( write > 0 )
                return write;

            return read;
        }

        public byte[] ToArray()
        {
            return bytes;
        }

        public void writeByte(byte b)
        {
            bytes[write] = b;
            write++;
        }

        public void writeInteger(int i)
        {
            byte[] bb = System.BitConverter.GetBytes(i);
            for (int b = 0; b < 4; b++)
            {
                bytes[write] = bb[b];
                write++;
            }
        }

        public void writeFloat(float f)
        {
            byte[] bb = System.BitConverter.GetBytes(f);
            for (int b = 0; b < 4; b++)
            {
                bytes[write] = bb[b];
                write++;
            }
        }

        public float readFloat()
        {
            byte[] bb = new byte[4];
            for (int b = 0; b < 4; b++)
            {
                bb[b] = readByte();
            }
            return BitConverter.ToSingle(bb, 0);
        }

        public void writeLong(long l)
        {
            byte[] bb = System.BitConverter.GetBytes(l);
            for (int b = 0; b < 4; b++)
            {
                bytes[write] = bb[b];
                write++;
            }
        }

        public void writeString(string str)
        {
            if (str != null && str.Length > 0)
            {
                writeInteger(str.Length);
                for (int s = 0; s < str.Length; s++)
                {
                    writeByte((byte)str[s]);
                }
            }
        }

        public int readPos()
        {
            return read;
        }

        public byte readByte()
        {
            byte b = bytes[read];
            read++;
            return b;
        }

        public int readInteger()
        {
            byte[] bb = new byte[4] { readByte(), readByte(), readByte(), readByte() };
            return System.BitConverter.ToInt32(bb, 0);
        }

        public string readString()
        {
            string result = "";
            int length = readInteger();
            for( int i=0; i<length; i++ )
            {
                result += (char)readByte();
            }
            return result;
        }

        public string readUnformatedString()
        {
            string result = "";
            int length = bytes.Length - read;
            for (int i = 0; i < length; i++)
            {
                byte b = readByte();
                if (b == 13)
                {
                    b = readByte();
                    if (b != 10)
                        read--;
                    return result;
                }
                else
                    result += (char)b;
            }
            return result;
        }

        public void writeVector3(Vector3 position)
        {
            writeFloat(position.X);
            writeFloat(position.Y);
            writeFloat(position.Z);
        }

        public Vector3 readVector3()
        {
            Vector3 position = new Vector3(readFloat(), readFloat(), readFloat());
            return position;
        }

        internal void writeVector2(Vector2 vec)
        {
            writeFloat(vec.X);
            writeFloat(vec.Y);
        }
    }

}
