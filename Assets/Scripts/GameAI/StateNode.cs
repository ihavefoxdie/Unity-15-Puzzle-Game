using System;
using System.Collections;
using System.Collections.Generic;


namespace GameAI
{
    class StateNode<T>
    {
        public double F { get; set; }
        public T[,] State { get; set; }
        public int EmptyColumn { get; private set; }
        public int EmptyRow { get; private set; }
        public int Depth { get; set; }
        public string StringRepresentation { get; set; }

        public StateNode(T[,] state, int emptyRow, int emptyColumn, int depth)
        {
            if (state.GetLength(0) != state.GetLength(1))
                throw new Exception("Number of columns and rows isn't the same!.");

            State = (T[,])state.Clone();
            EmptyColumn = emptyColumn;
            EmptyRow = emptyRow;
            Depth = depth;
            StringRepresentation = "";

            for (var i = 0; i < State.GetLength(0); i++)
            {
                for (var j = 0; j < State.GetLength(1); j++)
                    StringRepresentation += state[i, j] + ",";
            }
            if (StringRepresentation is null)
                throw new Exception("the array is empty!");
        }

        public int Size
        {
            get { return State.GetLength(0); }
        }

        public void Print()
        {
            for (var i = 0; i < State.GetLength(0); i++)
            {
                for (var j = 0; j < State.GetLength(1); j++)
                    Console.Write(State[i, j] + ", ");
                Console.WriteLine();
            }
        }
    }

}
