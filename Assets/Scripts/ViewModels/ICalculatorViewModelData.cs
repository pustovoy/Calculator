namespace Calculator.ViewModels
{
    public interface ICalculatorViewModelData
    {
        string FirstNumber { get; set; }
        string SecondNumber { get; set; }
        string Op { get; set; }
        string Result { get; }
        string UserInput { get; }
        string ActiveState { get; }
    }
}
