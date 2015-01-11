using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfApplication4
{
    /// <summary>
    /// Maya 側においてあるデーモンが生成したクラスのインスタンスを操作するためのラッパ
    /// </summary>
    class MayaModelInstance
    {
        public MayaModelInstance(Connector connector, string instanceName)
        {
            _connector = connector;
            _instanceName = instanceName;
        }


        /// <summary>
        /// インスタンスメソッドを呼び出す
        /// </summary>
        /// <param name="methodName"></param>
        /// <returns></returns>
        public string Invoke(string methodName)
        {
            _connector.Send(string.Format(
                "invoke {0} {1}", _instanceName, methodName));
            return MayaModel.ReadResult(_connector);
        }


        /// <summary>
        /// インスタンスメソッドを呼び出す
        /// </summary>
        /// <param name="methodName"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public string Invoke(string methodName, params string[] args)
        {
            _connector.Send(string.Format(
                "invoke {0} {1} {2}", _instanceName, methodName, string.Join(" ", args)));
            return MayaModel.ReadResult(_connector);
        }



        private Connector _connector;
        private string _instanceName;

    }
}
