using System;
using System.IO;

namespace TheWarriors
{
    public class ArchiveFileStream : Stream
    {
        public Stream BaseStream;

        public long lBaseStreamOffset;

        public long lBaseStreamLength;

        public long lBaseStreamPosition;

        public UInt32 uiFileHash;

        public ArchiveFileStream(Stream BaseStream_, long lFileOffset_, long lBaseStreamLength_, UInt32 uiFileHash_)
        {
            BaseStream = BaseStream_;
            lBaseStreamOffset = lFileOffset_;
            lBaseStreamLength = lBaseStreamLength_;
            lBaseStreamPosition = 0;
            uiFileHash = uiFileHash_;
        }

        public override bool CanRead
        {
            get { return true; }
        }

        public override bool CanSeek
        {
            get { return true; }
        }

        public override bool CanWrite
        {
            get { return false; }
        }

        public override long Length
        {
            get { return lBaseStreamLength; }
        }

        public bool Test { get { return true; } }

        public override long Position
        {
            get { return lBaseStreamPosition; }
            set { Seek(value, SeekOrigin.Begin); }
        }

        public override void Flush()
        {
            throw new NotImplementedException();
        }

        public override int Read(byte[] bBuffer, int iOffset, int iCount)
        {
            long lBasePosition = 0;
            int iBytesRead = 0;

            if (lBaseStreamPosition > lBaseStreamLength)
            {
                return 0;
            }

            lBasePosition = lBaseStreamOffset + lBaseStreamPosition;

            if (BaseStream.Position != lBasePosition)
            {
                BaseStream.Seek(lBasePosition, SeekOrigin.Begin);
            }

            iBytesRead = BaseStream.Read(bBuffer, iOffset, (int)Math.Min(lBaseStreamLength - lBaseStreamPosition, iCount));

            lBaseStreamPosition += iBytesRead;

            return iBytesRead;
        }

        public override long Seek(long lOffset, SeekOrigin Origin)
        {
            switch (Origin)
            {
                case SeekOrigin.Begin:
                    {
                        lBaseStreamPosition = lOffset;
                    }
                    break;
                case SeekOrigin.Current:
                    {
                        lBaseStreamPosition += lOffset;
                    }
                    break;
                case SeekOrigin.End:
                    {
                        lBaseStreamPosition = lBaseStreamLength - lOffset;
                    }
                    break;
            }

            if (lBaseStreamPosition < 0 || lBaseStreamPosition > lBaseStreamLength)
            {
                throw new ArgumentOutOfRangeException("*** Error: StreamSection (" + String.Format("{0:X8}", uiFileHash) + ") offset out of bounds.");
            }

            return BaseStream.Seek(lBaseStreamPosition + lBaseStreamOffset, SeekOrigin.Begin) - lBaseStreamOffset;
        }

        public override void SetLength(long value)
        {
            throw new NotImplementedException();
        }

        public override void Write(byte[] bBuffer, int iOffset, int iCount)
        {
            throw new NotImplementedException();
        }
    }
}