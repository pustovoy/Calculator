using Calculator.ViewModels;

namespace Calculator.Core
{
    class FirstNumberState: CalculatorState
    {
        public override void Handle(CalculatorViewModel vm)
        {
            vm.FirstNumber = vm.UserInput;
            vm.ResetUserInput();
            vm.SetState(vm.OperatorState);
        }

        public override string GetName()
        {
            return "FirstNumberState";
        }
    }
}
