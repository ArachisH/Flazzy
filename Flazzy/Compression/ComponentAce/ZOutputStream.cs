// Copyright (c) 2006, ComponentAce
// http://www.componentace.com
// All rights reserved.

// Redistribution and use in source and binary forms, with or without modification, are permitted provided that the following conditions are met:

// Redistributions of source code must retain the above copyright notice, this list of conditions and the following disclaimer. 
// Redistributions in binary form must reproduce the above copyright notice, this list of conditions and the following disclaimer in the documentation and/or other materials provided with the distribution. 
// Neither the name of ComponentAce nor the names of its contributors may be used to endorse or promote products derived from this software without specific prior written permission. 
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
/*
Copyright (c) 2001 Lapo Luchini.

Redistribution and use in source and binary forms, with or without
modification, are permitted provided that the following conditions are met:

1. Redistributions of source code must retain the above copyright notice,
this list of conditions and the following disclaimer.

2. Redistributions in binary form must reproduce the above copyright 
notice, this list of conditions and the following disclaimer in 
the documentation and/or other materials provided with the distribution.

3. The names of the authors may not be used to endorse or promote products
derived from this software without specific prior written permission.

THIS SOFTWARE IS PROVIDED ``AS IS'' AND ANY EXPRESSED OR IMPLIED WARRANTIES,
INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND
FITNESS FOR A PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE AUTHORS
OR ANY CONTRIBUTORS TO THIS SOFTWARE BE LIABLE FOR ANY DIRECT, INDIRECT,
INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT
LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA,
OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF
LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING
NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS SOFTWARE,
EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
*/
/*
* This program is based on zlib-1.1.3, so all credit should go authors
* Jean-loup Gailly(jloup@gzip.org) and Mark Adler(madler@alumni.caltech.edu)
* and contributors of zlib.
*/

using System;
using System.IO;

namespace Flazzy.Compression.ComponentAce
{
    public class ZOutputStream : Stream
    {
        private void InitBlock()
        {
            flush_Renamed_Field = ZlibConstants.Z_NO_FLUSH;
            buf = new byte[bufsize];
        }
        /// <summary> Returns the total number of bytes input so far.</summary>
        virtual public long TotalIn
        {
            get
            {
                return z.total_in;
            }

        }
        /// <summary> Returns the total number of bytes output so far.</summary>
        virtual public long TotalOut
        {
            get
            {
                return z.total_out;
            }

        }

        private readonly bool _leaveOpen;
        protected internal ZStream z = new ZStream();
        protected internal int bufsize = 4096;
        protected internal int flush_Renamed_Field;
        protected internal byte[] buf, buf1 = new byte[1];

        private readonly Stream _output;

        public ZOutputStream(Stream output, int level, bool leaveOpen = false)
        {
            _output = output;
            _leaveOpen = leaveOpen;

            InitBlock();
            z.deflateInit(level);
        }

        public void WriteByte(int b)
        {
            buf1[0] = (byte)b;
            Write(buf1, 0, 1);
        }
        //UPGRADE_TODO: The differences in the Expected value  of parameters for method 'WriteByte'  may cause compilation errors.  'ms-help://MS.VSCC.2003/commoner/redir/redirect.htm?keyword="jlca1092_3"'
        public override void WriteByte(byte b)
        {
            WriteByte((int)b);
        }

        public override void Write(byte[] b1, int off, int len)
        {
            if (len == 0)
                return;
            int err;
            byte[] b = new byte[b1.Length];
            System.Array.Copy(b1, 0, b, 0, b1.Length);
            z.next_in = b;
            z.next_in_index = off;
            z.avail_in = len;
            do
            {
                z.next_out = buf;
                z.next_out_index = 0;
                z.avail_out = bufsize;
                err = z.deflate(flush_Renamed_Field);
                if (err != ZlibConstants.Z_OK && err != ZlibConstants.Z_STREAM_END)
                {
                    throw new IOException("Deflating: " + z.msg);
                }
                _output.Write(buf, 0, bufsize - z.avail_out);
            }
            while (z.avail_in > 0 || z.avail_out == 0);
        }

        public virtual void finish()
        {
            do
            {
                z.next_out = buf;
                z.next_out_index = 0;
                z.avail_out = bufsize;
                int errorCode = z.deflate(ZlibConstants.Z_FINISH);
                if (errorCode != ZlibConstants.Z_STREAM_END && errorCode != ZlibConstants.Z_OK)
                {
                    throw new IOException("Deflating: " + z.msg);
                }
                if (bufsize - z.avail_out > 0)
                {
                    _output.Write(buf, 0, bufsize - z.avail_out);
                }
            }
            while (z.avail_in > 0 || z.avail_out == 0);
            Flush();
        }
        public virtual void end()
        {
            z.deflateEnd();
            z.free();
            z = null;
        }

        public override long Position
        {
            get { return 0; }
            set { throw new NotSupportedException(); }
        }
        public override long Length => 0;
        public override bool CanRead => false;
        public override bool CanSeek => false;
        public override bool CanWrite => true;

        public override void Flush()
        {
            _output.Flush();
        }
        public override void SetLength(long value)
        { }
        public override long Seek(long offset, SeekOrigin origin)
        {
            return 0;
        }
        public override int Read(byte[] buffer, int offset, int count)
        {
            return 0;
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                try
                {
                    finish();
                }
                finally
                {
                    end();
                    if (!_leaveOpen)
                    {
                        _output.Dispose();
                    }
                }
            }
        }
    }
}