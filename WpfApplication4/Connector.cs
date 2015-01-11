using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace WpfApplication4
{
    /// <summary>
    /// TCP 通信用クラス
    /// </summary>
    class Connector : IDisposable
    {
        ~Connector()
        {
            Dispose();
        }

        public void Dispose()
        {
            if (_disposed) return;
            _disposed = true;

            Disconnect();

            GC.SuppressFinalize(this);
        }

        private bool _disposed;


        public void Connect(string host, int port)
        {
            _client = new TcpClient(host, port);
            _stream = _client.GetStream();
        }

        public void Disconnect()
        {
            if (_stream != null) _stream.Close();
            _stream = null;

            if (_client != null) _client.Close();
            _client = null;
        }


        public void Send(string data)
        {
            if (_stream == null) return;

            var bytes = Encoding.UTF8.GetBytes(data);
            _stream.Write(bytes, 0, bytes.Length);
        }


        public string Receive()
        {
            if (_stream == null) return null;

            var result = new StringBuilder();

            var bytes = new byte[256];
            while(true)
            {
                var readCount = _stream.Read(bytes, 0, bytes.Length);
                result.Append(Encoding.UTF8.GetString(bytes));

                if (readCount < bytes.Length) break;
            }

            return result.ToString();
        }


        private TcpClient _client;
        private NetworkStream _stream;

    }
}
