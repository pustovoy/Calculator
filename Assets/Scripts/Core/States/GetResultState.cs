using Calculator.ViewModels;

namespace Calculator.Core
{
    class GetResultState : CalculatorState
    {
        public override void Handle(CalculatorViewModel vm)
        {
            vm.SetState(vm.ResultState);
            vm.CalculateResult();
        }
        public override string GetName()
        {
            return "GetResultState";
        }
    }
}
