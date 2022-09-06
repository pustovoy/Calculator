using Calculator.ViewModels;

namespace Calculator.Core
{
    class ExceptionState: CalculatorState
    {
        public override void Handle(CalculatorViewModel vm)
        {
            vm.SetState(vm.FirstNumberState);
            vm.Model.ResetData();
            vm.WriteSaveFile();
            vm.UpdateOutput();
        }

        public override string GetName()
        {
            return "ExceptionState";
        }
    }
}
