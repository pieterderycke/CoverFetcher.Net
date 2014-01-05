using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoverFetcher
{
    public class ByteArrayFileAbstraction : TagLib.File.IFileAbstraction
    {
        private readonly byte[] data;

        public ByteArrayFileAbstraction(string fileName, byte[] data)
        {
            this.Name = fileName;
            this.data = data;
        }

        public void CloseStream(Stream stream)
        {
            stream.Close();
        }

        public string Name { get; private set; }

        public System.IO.Stream ReadStream
        {
            get { return new MemoryStream(data); }
        }

        public System.IO.Stream WriteStream
        {
            get { throw new NotImplementedException(); }
        }
    }
}
