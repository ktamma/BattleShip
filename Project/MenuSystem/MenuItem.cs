using System;

namespace MenuSystem
{
    public class MenuItem
    {
        protected virtual string Label { get; set; }
        public virtual Func<string> MethodToExecute { get; set; }


        public MenuItem(string label , Func<string> methodToExecute)
        {
            Label = label;
            MethodToExecute = methodToExecute;
        }
        public override string ToString()
        {
            return $"{Label}";
        }

        

        public bool Equals(MenuItem menuItem)
        {
            if (this.Label == menuItem.Label)
            {
                return true;
            }

            return this.GetHashCode() == menuItem.GetHashCode();
        }
    }
}