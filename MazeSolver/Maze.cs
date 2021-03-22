using System;
using System.Collections.Generic;
using System.Text;

namespace MazeSolver
{
	delegate void MazeChangedHandler(int iChanged, int jChanged);
	class Maze
	{
		public int[,] map;
		public Tuple<int, int> startPos, endPos;

		int[,] m_iMaze;
		int m_iRows;
		int m_iCols;
		int iPath = 100;
		public event MazeChangedHandler OnMazeChangedEvent;

		//Constructor to hold necessary information to be solveable 
		public Maze(int[,] mapDetails, Tuple<int, int> startingPosition, Tuple<int, int> endingPosition)
		{
			map = mapDetails;
			startPos = startingPosition;
			endPos = endingPosition;


			m_iMaze = new int[mapDetails.GetLength(0), mapDetails.GetLength(1)];

			m_iRows = mapDetails.GetLength(0);
			m_iCols = mapDetails.GetLength(1);
		}

		public Maze(int[,] iMaze)
		{
			m_iMaze = iMaze;
			m_iRows = iMaze.GetLength(0);
			m_iCols = iMaze.GetLength(1);
		}

		public int Rows
		{
			get { return m_iRows; }
		}

		public int Cols
		{
			get { return m_iCols; }
		}
		public int[,] GetMaze
		{
			get { return m_iMaze; }
		}

		public int PathCharacter
		{
			get { return iPath; }
			set
			{
				if (value == 0)
					throw new Exception("Invalid path character specified");
				else
					iPath = value;
			}
		}

		public int this[int iRow, int iCol]
		{
			get { return m_iMaze[iRow, iCol]; }
			set
			{
				m_iMaze[iRow, iCol] = value;
				if (this.OnMazeChangedEvent != null)    // trigger event
					this.OnMazeChangedEvent(iRow, iCol);
			}
		}

		private int GetNodeContents(int[,] iMaze, int iNodeNo)
		{
			int iCols = iMaze.GetLength(1);
			return iMaze[iNodeNo / iCols, iNodeNo - iNodeNo / iCols * iCols];
		}

		private void ChangeNodeContents(int[,] iMaze, int iNodeNo, int iNewValue)
		{
			int iCols = iMaze.GetLength(1);
			iMaze[iNodeNo / iCols, iNodeNo - iNodeNo / iCols * iCols] = iNewValue;
		}

		public int[,] FindPath(int iFromY, int iFromX, int iToY, int iToX)
		{
			int iBeginningNode = iFromY * this.Cols + iFromX;
			int iEndingNode = iToY * this.Cols + iToX;
			return (Search(iBeginningNode, iEndingNode));
		}

		private enum Status
		{ Ready, Waiting, Processed }
		private int[,] Search(int iStart, int iStop)
		{
			const int empty = 0;

			int iRows = m_iRows;
			int iCols = m_iCols;
			int iMax = iRows * iCols;
			int[] Queue = new int[iMax];
			int[] Origin = new int[iMax];
			int iFront = 0, iRear = 0;

			//check if starting and ending points are valid (open)
			if (GetNodeContents(m_iMaze, iStart) != empty || GetNodeContents(m_iMaze, iStop) != empty)
			{
				return null;
			}

			//create dummy array for storing status
			int[,] iMazeStatus = map;

			//add starting node to Q
			Queue[iRear] = iStart;
			Origin[iRear] = -1;
			iRear++;
			int iCurrent, iLeft, iRight, iTop, iDown;
			while (iFront != iRear) // while Q not empty	
			{
				if (Queue[iFront] == iStop)     // maze is solved
					break;

				iCurrent = Queue[iFront];

				iLeft = iCurrent - 1;

				if (iLeft >= 0 && iLeft / iCols == iCurrent / iCols)
				{

					if (GetNodeContents(m_iMaze, iLeft) == empty)   //if left node is open(a path exists)
						if (GetNodeContents(iMazeStatus, iLeft) == (int)Status.Ready)   //if left node is ready
						{
							Queue[iRear] = iLeft; //add to Q
							Origin[iRear] = iCurrent;
							ChangeNodeContents(iMazeStatus, iLeft, (int)Status.Waiting); //change status to waiting
							iRear++;
						}
				}


				iRight = iCurrent + 1;

				if (iRight < iMax && iRight / iCols == iCurrent / iCols)
				{
					if (GetNodeContents(m_iMaze, iRight) == empty)  //if right node is open(a path exists)
						if (GetNodeContents(iMazeStatus, iRight) == (int)Status.Ready)  //if right node is ready
						{
							Queue[iRear] = iRight; //add to Q
							Origin[iRear] = iCurrent;
							ChangeNodeContents(iMazeStatus, iRight, (int)Status.Waiting); //change status to waiting
							iRear++;
						}
				}


				iTop = iCurrent - iCols;
				if (iTop >= 0)  //if top node exists
					if (GetNodeContents(m_iMaze, iTop) == empty)    //if top node is open(a path exists)
						if (GetNodeContents(iMazeStatus, iTop) == (int)Status.Ready)    //if top node is ready
						{
							Queue[iRear] = iTop; //add to Q
							Origin[iRear] = iCurrent;
							ChangeNodeContents(iMazeStatus, iTop, (int)Status.Waiting); //change status to waiting
							iRear++;
						}

				iDown = iCurrent + iCols;
				if (iDown < iMax)   //if bottom node exists
					if (GetNodeContents(m_iMaze, iDown) == empty)   //if bottom node is open(a path exists)
						if (GetNodeContents(iMazeStatus, iDown) == (int)Status.Ready)   //if bottom node is ready
						{
							Queue[iRear] = iDown; //add to Q
							Origin[iRear] = iCurrent;
							ChangeNodeContents(iMazeStatus, iDown, (int)Status.Waiting); //change status to waiting
							iRear++;
						}

				//change status of current node to processed
				ChangeNodeContents(iMazeStatus, iCurrent, (int)Status.Processed);
				iFront++;

			}

			//create an array(maze) for solution
			int[,] iMazeSolved = new int[iRows, iCols];
			for (int i = 0; i < iRows; i++)
				for (int j = 0; j < iCols; j++)
					iMazeSolved[i, j] = m_iMaze[i, j];

			//make a path in the Solved Maze
			iCurrent = iStop;
			ChangeNodeContents(iMazeSolved, iCurrent, iPath);
			for (int i = iFront; i >= 0; i--)
			{
				if (Queue[i] == iCurrent)
				{
					iCurrent = Origin[i];
					if (iCurrent == -1)     // maze is solved
						return (iMazeSolved);
					ChangeNodeContents(iMazeSolved, iCurrent, iPath);
				}
			}

			//no path exists
			return null;
		}
	}
}