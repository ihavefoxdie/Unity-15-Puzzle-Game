using GameAI;
using System.Collections.Generic;
using System.IO;
using System;
using Utils;
using System.Linq;

class AStar<T>
{
    public int StatesChecked { get; set; }
    private readonly StateNode<T> SolutionState;
    private T EmptyTile { get; set; }
    private readonly PriorityQueue<StateNode<T>, double> Queue;
    private readonly HashSet<string> Hash;
    private readonly HashSet<double> _hash;

    public AStar(StateNode<T> initial, StateNode<T> solution, T empty)
    {
        Queue = new PriorityQueue<StateNode<T>, double>();
        Queue.Enqueue(initial, initial.F);
        SolutionState = solution;
        EmptyTile = empty;
        Hash = new HashSet<string>();
        Hash.Add(Queue.Peek().StringRepresentation);
    }


    private double Heuristics(StateNode<T> node)
    {
        double result = 0.0;
        //loop for calculating MD for each element in the array
        for (var i = 0; i < node.State.GetLength(0); i++)
        {
            for (var j = 0; j < node.State.GetLength(1); j++)
            {
                var elem = node.State[i, j];
                if (elem!.Equals(EmptyTile)) continue; //MD calculation for empty tiles is not needed

                var found = false;

                //loop for finding the same element in the goal state to calculate MD
                for (var h = 0; h < SolutionState.State.GetLength(0); h++)
                {
                    for (var k = 0; k < SolutionState.State.GetLength(1); k++)
                    {
                        if (SolutionState.State[h, k]!.Equals(elem))
                        {
                            result += Math.Abs(h - i) + Math.Abs(j - k);
                            found = true;
                            break;
                        }
                    }
                    if (found)
                        break;
                }
            }
        }
        return result;
    }

    private void Expanding(StateNode<T> node, int altrow, int altcolumn)
    {
        T temporary;
        T[,] newState;
        StateNode<T> newNode;
        var column = node.EmptyColumn;
        var row = node.EmptyRow;

        newState = (T[,])node.State.Clone();
        temporary = newState[altrow, altcolumn];
        newState[altrow, altcolumn] = EmptyTile;
        newState[row, column] = temporary;
        newNode = new StateNode<T>(newState, altrow, altcolumn, node.Depth + 1);

        if (!Hash.Contains(newNode.StringRepresentation))
        {
            newNode.F = node.Depth + Heuristics(newNode);
            Queue.Enqueue(newNode, newNode.F);
            Hash.Add(newNode.StringRepresentation);
        }
    }

    //this method creates and enqueues new nodes that are not already included in the queue.
    private void ExpandNodes(StateNode<T> node)
    {
        var column = node.EmptyColumn;
        var row = node.EmptyRow;

        //Moving empty tile up
        if (row > 0)
        {
            Expanding(node, row - 1, column);
        }

        //Moving empty tile down
        if (row < node.Size - 1)
        {
            Expanding(node, row + 1, column);
        }

        //Moving empty tile left
        if (column > 0)
        {
            Expanding(node, row, column - 1);
        }

        //Moving empty tile right
        if (column < node.Size - 1)
        {
            Expanding(node, row, column + 1);
        }
    }

    /*//this method creates and enqueues new nodes that are not already included in the queue.
    private void ExpandNodes(StateNode<T> node)
    {
        T temporary;
        T[,] newState;
        var column = node.EmptyColumn;
        var row = node.EmptyRow;
        StateNode<T> newNode;

        //Moving empty tile up
        if (row > 0)
        {
            newState = (T[,])node.State.Clone();
            temporary = newState[row - 1, column];
            newState[row - 1, column] = EmptyTile;
            newState[row, column] = temporary;
            newNode = new StateNode<T>(newState, row - 1, column, node.Depth + 1);

            if (!hash.Contains(newNode.StringRepresentation))
            {
                newNode.F = node.Depth + Heuristics(newNode);
                queue.Enqueue(newNode, newNode.F);
                hash.Add(newNode.StringRepresentation);
            }
        }

        //Moving empty tile down
        if (row < node.Size - 1)
        {
            newState = (T[,])node.State.Clone();
            temporary = newState[row + 1, column];
            newState[row + 1, column] = EmptyTile;
            newState[row, column] = temporary;
            newNode = new StateNode<T>(newState, row + 1, column, node.Depth + 1);

            if (!hash.Contains(newNode.StringRepresentation))
            {
                newNode.F = node.Depth + Heuristics(newNode);
                queue.Enqueue(newNode, newNode.F);
                hash.Add(newNode.StringRepresentation);
            }
        }

        //Moving empty tile left
        if (column > 0)
        {
            newState = (T[,])node.State.Clone();
            temporary = newState[row, column - 1];
            newState[row, column - 1] = EmptyTile;
            newState[row, column] = temporary;
            newNode = new StateNode<T>(newState, row, column - 1, node.Depth + 1);

            if (!hash.Contains(newNode.StringRepresentation))
            {
                newNode.F = node.Depth + Heuristics(newNode);
                queue.Enqueue(newNode, newNode.F);
                hash.Add(newNode.StringRepresentation);
            }
        }

        //Moving empty tile right
        if (column < node.Size - 1)
        {
            newState = (T[,])node.State.Clone();
            temporary = newState[row, column + 1];
            newState[row, column + 1] = EmptyTile;
            newState[row, column] = temporary;
            newNode = new StateNode<T>(newState, row, column + 1, node.Depth + 1);

            if (!hash.Contains(newNode.StringRepresentation))
            {
                newNode.F = node.Depth + Heuristics(newNode);
                queue.Enqueue(newNode, newNode.F);
                hash.Add(newNode.StringRepresentation);
            }
        }
    }*/

    //this method goes through every state enqueued, creating new ones with the ExpandNodes method until solution is found.
    public StateNode<T> Execute()
    {
        while (true)
        {
            var dequeuedElement = Queue.Dequeue();

            //var element = queue.Dequeue();
            if (Queue.Count > 100)
            {
                StreamWriter file = new StreamWriter("myfile", true);
                while (Queue.Count > 0)
                {
                    var element = Queue.Dequeue();
                    file.WriteLine("{0}|{1}|{2},{3},{4}", element.F, element.StringRepresentation, element.EmptyColumn, element.EmptyRow, element.Depth);
                }
                file.Close();
            }

            StatesChecked++;

            if (dequeuedElement.StringRepresentation.Equals(SolutionState.StringRepresentation))
            {
                File.Delete("myfile");
                File.Delete("tempBak");
                return dequeuedElement;
            }

            ExpandNodes(dequeuedElement);

            if (Queue.Count == 0)
            {
                foreach (var line in File.ReadLines("myfile"))
                {
                    if(line == null)
                        throw new Exception("ERROR! No solution found! Perhaps given puzzle configuration is unsolvable?");
                    
                    string text = line.ToString();
                    int counter = 0;
                    string number = " ";

                    while (text[counter] != '|') { number = number.Insert(number.Length - 1, text[counter].ToString()); counter++; }
                    counter++;
                    int FValue = Convert.ToInt32(number);

                    int[,] state = new int[dequeuedElement.State.GetLength(0), dequeuedElement.State.GetLength(1)];
                    for (int i = 0; i < state.GetLength(0); i++)
                        for (int j = 0; j < state.GetLength(1); j++)
                        {
                            number = " ";
                            while (text[counter] != ',')
                            {
                                number = number.Insert(number.Length - 1, text[counter].ToString());
                                counter++;
                            }
                            state[i, j] = Convert.ToInt32(number);
                            counter++;
                        }
                    counter++;

                    number = " ";
                    while (text[counter] != ',')
                    {
                        number = number.Insert(number.Length - 1, text[counter].ToString());
                        counter++;
                    }
                    int emptyCol = Convert.ToInt32(number);
                    counter++;

                    number = " ";
                    while (text[counter] != ',')
                    {
                        number = number.Insert(number.Length - 1, text[counter].ToString());
                        counter++;
                    }
                    int emptyRow = Convert.ToInt32(number);
                    counter++;

                    number = " ";
                    while (counter < text.Length)
                    {
                        number = number.Insert(number.Length - 1, text[counter].ToString());
                        counter++;
                    }
                    int depth = Convert.ToInt32(number);
                    counter++;

                    var node = new StateNode<T>((T[,])(object)state, emptyRow, emptyCol, depth)
                    {
                        F = FValue
                    };


                    Queue.Enqueue(node, node.F);
                    var Lines = File.ReadAllLines(@"myfile");
                    var newLines = Lines.Where(lines => !lines.Contains(text));
                    File.WriteAllLines(@"temporary", newLines);

                    break;
                }
                File.Replace("temporary", "myfile", "tempBak");
                /*if (Queue.Count == 0)
                {
                    File.Delete("myfile");
                    File.Delete("tempBak");
                    throw new Exception("ERROR! No solution found! Perhaps given puzzle configuration is unsolvable?");
                }*/
            }
        }
    }
}