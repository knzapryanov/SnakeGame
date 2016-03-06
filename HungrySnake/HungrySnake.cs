using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace HungrySnake
{
    class HungrySnake
    {
        // Struct holding each element position on the console
        struct Position
        {
            public Position(int row, int col)
            {
                this.Row = row;
                this.Col = col;
            }

            public int Row;
            public int Col;
        }

        static void Main()
        {
            // Set the console height buffer to be console window height (we do not want the console to have scroll bar)
            Console.BufferHeight = Console.WindowHeight;

            // Create array with 4 Positions which will be actualy the directions change Positions (row, col changes affect the direction)
            Position[] directions = new Position[]
            {
                new Position(0, 1),     // right
                new Position(0, -1),    // left
                new Position(1, 0),     // down
                new Position(-1, 0)     // up
            };
            byte right = 0;
            byte left = 1;
            byte down = 2;
            byte up = 3;

            // Create Queue of positions which will contain the snake elements positions
            Queue<Position> snakeElements = new Queue<Position>();

            // Enqueue the Positions of the starting number of elements for the snake (row = 0, col = i)
            for (int i = 0; i < 6; i++)
            {
                snakeElements.Enqueue(new Position(0, i));
            }

            // Set the console cursor position for each snake element and print it. This is the starting snake print.
            foreach (var position in snakeElements)
            {
                Console.SetCursorPosition(position.Col, position.Row);
                Console.Write("*");
            }

            // Generate random position for the first food and print it on the console
            Random randomNumberGenerator = new Random();
            Position foodPosition = new Position(randomNumberGenerator.Next(Console.WindowHeight), randomNumberGenerator.Next(Console.WindowWidth));
            Console.SetCursorPosition(foodPosition.Col, foodPosition.Row);
            Console.Write("@");

            // Set the starting sleep time for the current thread (affects the snake speed) and set the starting direction for the snake
            int sleepTime = 100;
            int direction = right;

            // Infinite loop which will generate the snake movement. Game over breaks the loop.
            while (true)
            {
                // If the user press any keyboard key and the key is some of the arrow keys change the direction accordingly
                // Also change the direction only if the pressed direction is not directly opposite of the current direction (the snake cant make U-turns over herself :D)
                if (Console.KeyAvailable)
                {                  
                    ConsoleKeyInfo userInput = Console.ReadKey();
                    if (userInput.Key == ConsoleKey.RightArrow)
                    {
                        if(direction != left) direction = right;
                    }
                    else if (userInput.Key == ConsoleKey.LeftArrow)
                    {
                        if(direction != right) direction = left;
                    }
                    else if (userInput.Key == ConsoleKey.DownArrow)
                    {
                        if(direction != up) direction = down;
                    }
                    else if (userInput.Key == ConsoleKey.UpArrow)
                    {
                        if(direction != down) direction = up;
                    }
                }

                // Get the Position of the current snake head which is the last element in the snakeElements Queue
                Position snakeOldHead = snakeElements.Last();
                // Get the next direction change Position from the array using the user input direction for array index
                Position nextDirection = directions[direction];
                // Generate Position for the new snake head. Using the snake old head position and and the next direction changes.
                Position snakeNewHead = new Position(snakeOldHead.Row + nextDirection.Row, snakeOldHead.Col + nextDirection.Col);
                // If the new snake head position is on some of the borders of the console teleport it on the opposite side.
                if (snakeNewHead.Row < 0) snakeNewHead.Row = Console.WindowHeight - 1;
                if (snakeNewHead.Col < 0) snakeNewHead.Col = Console.WindowWidth - 1;
                if (snakeNewHead.Row == Console.WindowHeight) snakeNewHead.Row = 0;
                if (snakeNewHead.Col == Console.WindowWidth) snakeNewHead.Col = 0;

                // If the new snake head Position is on some of the current snake elements positions the snake bites herself and game over.
                // Print the message and the user points.
                if(snakeElements.Contains(snakeNewHead))
                {
                    Console.SetCursorPosition(0,0);
                    //Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("Tragic death of the snake occured! Game over!");
                    Console.WriteLine("Your points are: {0}", (snakeElements.Count - 6) * 100);
                    Thread.Sleep(1000);
                    return;
                }
                // Enqueue the new snake head position in the snake elements
                snakeElements.Enqueue(snakeNewHead);
                // Set the console cursor to the new snake head position
                Console.SetCursorPosition(snakeNewHead.Col, snakeNewHead.Row);
                // According to the direction print the right head
                if (direction == right) Console.Write('>');
                if (direction == left) Console.Write('<');
                if (direction == down) Console.Write('v');
                if (direction == up) Console.Write('^');
                // Set the cursor position to the old head and print regular snake element
                Console.SetCursorPosition(snakeOldHead.Col, snakeOldHead.Row);
                Console.Write('*');

                // If the new snake head position is on the food position the snake eat the food. We do not dequeue snake element and the snake grows.
                if (foodPosition.Row == snakeNewHead.Row && foodPosition.Col == snakeNewHead.Col)
                {
                    // Decrease the sleepTime on each food eaten. The game becomes harder.
                    sleepTime -= 1;
                    // Generate new food on random position which is not on the snake elements position.
                    do
                    {
                        foodPosition = new Position(randomNumberGenerator.Next(Console.WindowHeight), randomNumberGenerator.Next(Console.WindowWidth));
                    } while (snakeElements.Contains(foodPosition));
                    Console.SetCursorPosition(foodPosition.Col, foodPosition.Row);
                    Console.Write("@");
                }
                // If the snake do not ate the food just dequeue the last element of the snake ant print empty symbol on its position. 
                else
                {
                    Position snakeTail = snakeElements.Dequeue();
                    Console.SetCursorPosition(snakeTail.Col, snakeTail.Row);
                    Console.Write(' ');
                }

                Thread.Sleep(sleepTime);
            }
        }
    }
}
