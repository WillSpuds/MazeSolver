using System;
using System.IO;
using System.Collections.Generic;

namespace MazeSolver
{
    class Program
    {
        static void Main()
        {
            //Create List of All Mazes in File
            List<string> mazeList = Initialise();

            //Display Maze Files and Choose Which To Run
            string chosenMazeFileName = ChooseMaze(mazeList);

            //Display the Maze
            DisplayMaze(chosenMazeFileName);

            //Load Maze into a 2D array and find A & B
            Maze currentMaze = CreateMaze(chosenMazeFileName);

            //Solves and displays the result
            SolveMaze(currentMaze);

            Console.ReadLine();
        }

        static List<string> Initialise()
        {
            //get current directory and open mazes
            var mazeFolder = Path.Combine(Directory.GetCurrentDirectory(), "Mazes");

            //add each string into the mazelist
            DirectoryInfo d = new DirectoryInfo(@mazeFolder);
            FileInfo[] Files = d.GetFiles();

            List<string> mazeFiles = new List<string>();

            foreach (FileInfo file in Files)
            {
                mazeFiles.Add(file.Name);
            }

            return mazeFiles;
        }

        static string ChooseMaze(List<string> listOfMazes)
        {
            for (int i = 0; i < listOfMazes.Count; i++)
            {
                Console.WriteLine((i + 1) + ") " + listOfMazes[i]);
            }

            Console.WriteLine("Please enter the number of the maze you'd like to complete:");

            int userInt = getIntFromInput(listOfMazes.Count);
            Console.WriteLine("You have selected Maze " + userInt + "!");

            return listOfMazes[userInt - 1];
        }

        static void DisplayMaze(string fileName)
        {
            var mazeFile = Path.Combine(Directory.GetCurrentDirectory(), "Mazes", fileName);

            string text = File.ReadAllText(@mazeFile);

            Console.WriteLine(text);
        }

        static Maze CreateMaze(string fileToRead)
        {
            var mazeFile = Path.Combine(Directory.GetCurrentDirectory(), "Mazes", fileToRead);
            StreamReader file = new StreamReader(mazeFile);

            int numOfRows = 0;
            int numOfColumns = 0;
            string line;

            //creates an array for the maze map to be read into
            while ((line = file.ReadLine()) != null)
            {
                numOfColumns = 0;

                foreach (char element in line)
                {
                    numOfColumns++;
                }
                numOfRows++;
            }

            file.Close();

            //create the array
            int[,] mazeElements = new int[numOfColumns, numOfRows];

            int columnID = 0;
            int rowID = 0;

            StreamReader fileAgain = new StreamReader(mazeFile);

            Tuple<int, int> aPos = new Tuple<int, int>(0, 0);
            Tuple<int, int> bPos = new Tuple<int, int>(0, 0);

            //fills array with the necessary data and finds the start and end points
            while ((line = fileAgain.ReadLine()) != null)
            {
                columnID = 0;
                foreach (char currentElement in line)
                {
                    int currentElementInt = 0;

                    if (isChar(currentElement, "A"))
                    {
                        //store start point
                        aPos = new Tuple<int, int>(columnID, rowID);
                        currentElementInt = 0;
                    }
                    else if (isChar(currentElement, "B"))
                    {
                        bPos = new Tuple<int, int>(columnID, rowID);
                        currentElementInt = 0;
                    }
                    else if (isChar(currentElement, "."))
                    {
                        currentElementInt = 0;
                    }
                    else if (isChar(currentElement, "x"))
                    {
                        currentElementInt = 1;
                    }
                    else
                    {
                        //
                    }

                    mazeElements[columnID, rowID] = currentElementInt;

                    columnID++;
                }
                rowID++;
            }
            file.Close();

            //creates a maze object
            Maze currentMaze = new Maze(mazeElements, aPos, bPos);
            return currentMaze;
        }

        static void SolveMaze(Maze currentMaze)
        {
            Maze m_Maze = currentMaze;
            int[,] m_iMaze;

            //Show Solution
            //start
            int iY = m_Maze.startPos.Item1;
            int iX = m_Maze.startPos.Item2;


            //endpoint 6,1
            int iSelectedY = m_Maze.endPos.Item1;
            int iSelectedX = m_Maze.endPos.Item2;

            //get these figures from the maze
            //theMaze.startPos

            m_iMaze = m_Maze.GetMaze;

            int[,] iSolvedMaze = m_Maze.FindPath(iSelectedY, iSelectedX, iY, iX);

            if (iSolvedMaze != null)
            {
                m_iMaze = iSolvedMaze;
            }

            int rowCount = 0;
            int colCount = 0;

            int totalCount = 0;
            string stepIcon = "O";
            Console.WriteLine();

            foreach (var item in iSolvedMaze)
            {
                if (colCount >= iSolvedMaze.GetLength(0))
                {
                    rowCount++;
                    Console.WriteLine();
                    colCount = 0;
                }

                if (iSolvedMaze[colCount, rowCount] == 100)
                {
                    stepIcon = ">";
                }
                else
                {
                    //write original element

                    stepIcon = m_Maze.map[colCount, rowCount].ToString();

                    if (stepIcon == "2")
                    {
                        stepIcon = ".";
                    }
                    else if (stepIcon == "1")
                    {
                        stepIcon = "x";
                    }

                }
                Console.Write(stepIcon);
                colCount++;
                totalCount++;
            }

            Tuple<int, int> firstStep = m_Maze.startPos;
            Tuple<int, int> finalStep = m_Maze.endPos;

            bool northCheck;
            bool eastCheck;
            bool southCheck;
            bool westCheck;


            string optimalRoute = "";
            Tuple<int, int> previousStep = new Tuple<int, int>(0, 0);
            Tuple<int, int> currentStep, nextStep, northStep, eastStep, southStep, westStep;

            bool pathDecoded = false;

            previousStep = firstStep;

            currentStep = new Tuple<int, int>(firstStep.Item1, firstStep.Item2);

            nextStep = currentStep;

            //Converts the mazes path into a series of compass directions
            while (!pathDecoded)
            {
                northStep = new Tuple<int, int>(currentStep.Item1, currentStep.Item2 - 1);
                eastStep = new Tuple<int, int>(currentStep.Item1 + 1, currentStep.Item2);
                southStep = new Tuple<int, int>(currentStep.Item1, currentStep.Item2 + 1);
                westStep = new Tuple<int, int>(currentStep.Item1 - 1, currentStep.Item2);

                northCheck = (northStep.Item1 == previousStep.Item1) && (northStep.Item2 == previousStep.Item2);
                eastCheck = (eastStep.Item1 == previousStep.Item1) && (eastStep.Item2 == previousStep.Item2);
                southCheck = (southStep.Item1 == previousStep.Item1) && (southStep.Item2 == previousStep.Item2);
                westCheck = (westStep.Item1 == previousStep.Item1) && (westStep.Item2 == previousStep.Item2);

                if ((iSolvedMaze[northStep.Item1, northStep.Item2] == 100) && (!northCheck))
                {
                    optimalRoute = optimalRoute + "N";
                    nextStep = northStep;
                }
                else if ((iSolvedMaze[eastStep.Item1, eastStep.Item2] == 100) && (!eastCheck))
                {
                    optimalRoute = optimalRoute + "E";

                    nextStep = eastStep;
                }
                else if ((iSolvedMaze[southStep.Item1, southStep.Item2] == 100) && (!southCheck))
                {
                    optimalRoute = optimalRoute + "S";
                    nextStep = southStep;
                }
                else if ((iSolvedMaze[westStep.Item1, westStep.Item2] == 100) && (!westCheck))
                {
                    optimalRoute = optimalRoute + "W";
                    nextStep = westStep;
                }

                previousStep = currentStep;
                currentStep = nextStep;

                if ((currentStep.Item1 == finalStep.Item1) && (currentStep.Item2 == finalStep.Item2))
                {
                    pathDecoded = true;
                }
            }

            Console.WriteLine();
            Console.WriteLine("The optimal route is: " + optimalRoute);
        }

        static int getIntFromInput(int max)
        {
            //Method for error checking all inputs within the applciation
            int returnInt = 0;

            bool validInput = false;

            while (validInput == false)
            {

                string inputString = Console.ReadLine();

                //Makes sure that the inputted value is an integer AND within range, which is passed in as a parameter
                try
                {
                    returnInt = Convert.ToInt32(inputString);
                    if (returnInt >= 1 && returnInt <= max)
                    {
                        validInput = true;
                    }
                    else
                    {
                        Console.WriteLine("Please enter a number between 1 and " + max);
                    }
                }
                catch
                {
                    Console.WriteLine("Only enter a number");
                }
            }
            return returnInt;
        }
        static bool isChar(char character, string condition)
        {
            bool isSame = false;

            string charString = character.ToString();

            if (charString == condition)
            {
                isSame = true;
            }
            return isSame;
        }
    }


}