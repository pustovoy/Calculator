using Calculator.Core;

namespace Calculator.ViewModels
{
    public interface ICalculatorViewModel: ICalculatorViewModelData
    {
        ReactiveProperty<string> Output { get; set; }
        ReactiveProperty<CalculatorState> State { get; set; }
        void OnControlButtonClicked();
        void OnUserInput(string val);
    }
}