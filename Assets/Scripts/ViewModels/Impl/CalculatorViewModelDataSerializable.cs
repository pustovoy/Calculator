namespace Calculator.ViewModels
{
    public class CalculatorViewModelDataSerializable : ICalculatorViewModelData
    {
        public CalculatorViewModelDataSerializable(string firstNumber, string secondNumber, string op, string result, string userInput, string activeState)
        {
            FirstNumber = firstNumber;
            SecondNumber = secondNumber;
            Op = op;
            Result = result;
            UserInput = userInput;
            ActiveState = activeState;
        }

        public string FirstNumber { get; set; }
        public string SecondNumber { get; set; }
        public string Op { get; set; }
        public string Result { get; private set; }
        public string UserInput { get; private set; }
        public string ActiveState { get; private set; }
    }
}
