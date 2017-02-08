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
    public class ZInputStream : Stream
    {
        private readonly byte[] _chunk;
        private readonly Stream _input;
        private readonly ZStream _zStream;

        public int FlushMode { get; set; }
        /// <summary>
        /// Returns the total number of bytes input so far.
        /// </summary>
        public long TotalIn => _zStream.total_in;
        ///
        /// <summary> Returns the total number of bytes output so far.</summary>
        ///
        public long TotalOut => _zStream.total_out;

        public ZInputStream(Stream input)
        {
            _input = input;
            _chunk = new byte[512];

            _zStream = new ZStream();
            _zStream.inflateInit();
            _zStream.next_in = _chunk;
            _zStream.next_in_index = 0;
            _zStream.avail_in = 0;
        }

        public override long Position
        {
            get { return _input.Position; }
            set { throw new NotSupportedException(); }
        }
        public override long Length => _input.Length;
        public bool IsDataAvailable { get; private set; } = true;

        public override bool CanRead => true;
        public override bool CanSeek => false;
        public override bool CanWrite => false;

        public override void SetLength(long value)
        { }
        public override long Seek(long offset, SeekOrigin origin)
        {
            return 0;
        }

        public override int ReadByte()
        {
            var buffer = new byte[1];
            if (Read(buffer, 0, 1) != -1)
            {
                return (buffer[0] & 0xFF);
            }
            return -1;
        }
        public override int Read(byte[] buffer, int offset, int count)
        {
            if (count == 0) return 0;

            int errorCode = 0;
            _zStream.next_out = buffer;
            _zStream.avail_out = count;
            _zStream.next_out_index = offset;
            do
            {
                if ((_zStream.avail_in == 0) && IsDataAvailable)
                {
                    _zStream.next_in_index = 0;
                    _zStream.avail_in = SupportClass.ReadInput(_input, _chunk, 0, _chunk.Length);
                    if (_zStream.avail_in == -1)
                    {
                        _zStream.avail_in = 0;
                        IsDataAvailable = false;
                    }
                }

                errorCode = _zStream.inflate(FlushMode);
                if (!IsDataAvailable && (errorCode == ZlibConstants.Z_BUF_ERROR))
                {
                    return -1;
                }
                if (errorCode != ZlibConstants.Z_OK && errorCode != ZlibConstants.Z_STREAM_END)
                {
                    throw new IOException("Inflating: " + _zStream.msg);
                }
                if (!IsDataAvailable && (_zStream.avail_out == count))
                {
                    return -1;
                }
            }
            while (errorCode == ZlibConstants.Z_OK && _zStream.avail_out == count);
            return (count - _zStream.avail_out);
        }
        public override void Write(byte[] buffer, int offset, int count)
        {
            throw new NotSupportedException();
        }

        public override void Flush()
        {
            _input.Flush();
        }
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _input.Dispose();
            }
        }
    }
}