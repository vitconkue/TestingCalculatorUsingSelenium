namespace TestingTestShepNz
{
    public class Input
    {
        public string Number1 { get; set; }
        public string Number2 { get; set; }
        public string Operation { get; set; }
        public bool CheckIntegerOnly { get; set; }
        public bool WillCalculate { get; set; }
        public bool WillClear { get; set; }
    }

    public class Output
    {
        public string Number1Field { get; set; }
        public string Number2Field { get; set; }
        public string ResultField { get; set; }
        public string ErrorField { get; set; }

        public override bool Equals(object obj)
        {
            //Check for null and compare run-time types.
            if ((obj == null) || !this.GetType().Equals(obj.GetType()))
            {
                return false;
            }

            Output objOutput = (Output) obj;
            return objOutput.Number1Field == Number1Field && objOutput.Number2Field == Number2Field &&
                   objOutput.ResultField == ResultField && objOutput.ErrorField == ErrorField;
        }
    }

    public class TestCase
    {
        public Input Input { get; set; }
        public Output Output { get; set; }
    }
}