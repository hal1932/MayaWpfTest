using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfApplication4
{
    /// <summary>
    /// Maya 側においてあるデーモンと接続して、Model クラスを操作するためのラッパ
    /// </summary>
    class MayaModel : IDisposable
    {
        #region IDisposable
        ~MayaModel()
        {
            Dispose();
        }

        public void Dispose()
        {
            if (_disposed) return;
            _disposed = true;

            if (_connector != null)
            {
                _connector.Send("__exit__");
                _connector.Dispose();
            }

            GC.SuppressFinalize(this);
        }

        private bool _disposed;
        #endregion


        /// <summary>
        /// 接続開始、切断は Dispose かファイナライザで行う
        /// </summary>
        public void Setup()
        {
            _connector.Connect("localhost", 1111);
        }


        /// <summary>
        /// Model のインスタンスを生成する
        /// </summary>
        /// <param name="className"></param>
        /// <returns></returns>
        public MayaModelInstance CreateInstance(string className)
        {
            _connector.Send(string.Format("new {0}", className));
            var instanceName = ReadResult(_connector);
            return new MayaModelInstance(_connector, instanceName);
        }


        // @private
        // デーモンから結果を受け取るためのラッパ
        public static string ReadResult(Connector connector)
        {
            var result = connector.Receive();
            if (result.StartsWith("\0")) return string.Empty;

            var tmp = result.Split(' ');
            var len = int.Parse(tmp[0]);
            return tmp[1].Substring(0, len);
        }


        Connector _connector = new Connector();
    }
}
