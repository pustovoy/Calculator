using Calculator.Core;
using Calculator.ViewModels;
using UnityEngine;

namespace Calculator.Views
{
    public class BaseGuiView : MonoBehaviour, IView
    {
        public readonly ReactiveProperty<ViewModel> ViewModelProperty = new ReactiveProperty<ViewModel>();

        public BaseGuiView()
        {
            this.ViewModelProperty.OnValueChanged += OnViewModelChanged;
        }

        public ViewModel VM 
        { 
            get { return ViewModelProperty.Value; }
            set { ViewModelProperty.Value = value; }
        }

        protected virtual void OnViewModelChanged(ViewModel oldViewModel, ViewModel newViewModel)
        {

        }
    }
}
