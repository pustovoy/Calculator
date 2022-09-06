using Calculator.ViewModels;

namespace Calculator.Core
{
    public abstract class CalculatorState
    {
        public virtual void Handle(CalculatorViewModel vm)
        {

        }

        public virtual string GetName()
        {
            return "Abstract state";
        }
    }
}
