using System;
using System.IO;

namespace TheWarriors
{
    public class ArchiveFileBinaryReader : BinaryReader
    {
        public ArchiveFileBinaryReader(Stream StreamFile_)  : base(StreamFile_) { }

        public long Position()
        {
            return BaseStream.Position;
        }

        public long Length()
        {
            return BaseStream.Length;
        }

        public void SeekBeginning(long lOffset_)
        {
            BaseStream.Seek(lOffset_, SeekOrigin.Begin);
        }

        public void SeekCurrent(long lOffset_)
        {
            BaseStream.Seek(lOffset_, SeekOrigin.Current);
        }

        public override int ReadInt32()
        {
            byte[] bBytesRead = ReadBytes(4);

            if (BitConverter.IsLittleEndian == false)
            {
                Array.Reverse(bBytesRead);
            }

            return BitConverter.ToInt32(bBytesRead, 0);
        }

        public override UInt32 ReadUInt32()
        {
            byte[] bBytesRead = ReadBytes(4);

            if (BitConverter.IsLittleEndian == false)
            {
                //Array.Reverse(bBytesRead);
            }

            return BitConverter.ToUInt32(bBytesRead, 0);
        }

        public override Int16 ReadInt16()
        {
            byte[] bBytesRead = ReadBytes(2);

            if (BitConverter.IsLittleEndian == false)
            {
                Array.Reverse(bBytesRead);
            }

            return BitConverter.ToInt16(bBytesRead, 0);
        }

        public override UInt16 ReadUInt16()
        {
            byte[] bBytesRead = ReadBytes(2);

            if (BitConverter.IsLittleEndian == false)
            {
                Array.Reverse(bBytesRead);
            }

            return BitConverter.ToUInt16(bBytesRead, 0);
        }
    }
}