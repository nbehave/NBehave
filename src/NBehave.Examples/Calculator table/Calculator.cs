using System.Collections.Generic;

namespace NBehave.Examples.Calculator_table
{
    public class Calculator
    {
        private readonly Queue<int> _buffer = new Queue<int>();

        public void Enter(int number)
        {
            _buffer.Enqueue(number);
        }

        public void Add()
        {
            _buffer.Enqueue(_buffer.Dequeue() + _buffer.Dequeue());
        }

        public int Value()
        {
            return _buffer.Peek();
        }
    }
}
