using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace WpfApplication4
{
    class DelegateCommand : ICommand
    {
        public DelegateCommand(Action<object> execute) { _execute = execute; }
        public void Execute(object param) { _execute(param); }

        public bool CanExecute(object param) { return true; }
        public event EventHandler CanExecuteChanged { add { } remove { } }

        private Action<object> _execute;
    }


    class MainWindowViewModel : INotifyPropertyChanged
    {
        public DelegateCommand UpdateCommand { get; private set; }
        public DelegateCommand SelectCommand { get; private set; }

        #region NodeArray
        public string[] NodeArray
        {
            get { return _nodeArray; }
            set
            {
                _nodeArray = value;
                OnPropertyChanged("NodeArray");
            }
        }
        private string[] _nodeArray;
        #endregion

        #region SelectedNode
        public string SelectedNode
        {
            set
            {
                OnSelectionChanged(value);
            }
        }
        #endregion


        public MainWindowViewModel()
        {
            UpdateCommand = new DelegateCommand((param) => OnUpdate());

            // Maya 側のデーモンに接続して
            _model.Setup();

            // Model のインスタンスを生成する
            _modelInstance = _model.CreateInstance("SceneInspector");
        }


        #region INotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged == null) return;
            PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion


        private void OnUpdate()
        {
            var nodeArrayStr = _modelInstance.Invoke("getNodeList");
            NodeArray = nodeArrayStr.Split(',');
        }


        private void OnSelectionChanged(string selectedNode)
        {
            _modelInstance.Invoke("selectNode", selectedNode);
        }


        private MayaModel _model = new MayaModel();
        private MayaModelInstance _modelInstance;
    }
}
