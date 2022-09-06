using Calculator.ViewModels;

namespace Calculator.Core
{
    class OperatorState: CalculatorState
    {
        public override void Handle(CalculatorViewModel vm)
        {
            vm.Op = vm.UserInput;
            vm.ResetUserInput();
            vm.SetState(vm.SecondNumberState);
        }
        public override string GetName()
        {
            return "OperatorState";
        }
    }
}
