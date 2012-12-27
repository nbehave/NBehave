using System.Collections.Generic;

namespace $rootnamespace$
{
  public class Calculator
    {
        private readonly Queue<int> buffer = new Queue<int>();

        public void Enter(int number)
        {
            buffer.Enqueue(number);
        }

        public void Add()
        {
            buffer.Enqueue(buffer.Dequeue() + buffer.Dequeue());
        }

        public int Value()
        {
            return buffer.Peek();
        }
    }
}