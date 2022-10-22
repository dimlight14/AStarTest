using System;

namespace PathFinding
{
    public static class PathFindingUtility
    {
        public const int MoveStraightCost = 10;
        public const int MoveDiagonalCost = 14;
        
        public static int CalculateDistanceCost(int aPositionX,int aPositionY,int bPositionX,int bPositionY)
        {
            var xDistance = Math.Abs(aPositionX - bPositionX);
            var yDistance = Math.Abs(aPositionY - bPositionY);

            if (xDistance >= yDistance)
            {
                return MoveDiagonalCost * yDistance + (xDistance - yDistance) * MoveStraightCost;
            }
            else
            {
                return MoveDiagonalCost * xDistance + (yDistance - xDistance) * MoveStraightCost;
            }
        }
        public static int GetIndex(int xPos, int yPos, int gridWidth)
        {
            return xPos + yPos * gridWidth;
        }
        
        public static bool IsPositionInsideGrid(int posX, int posY, int gridSize)
        {
            return
                posX >= 0 &&
                posY >= 0 &&
                posX < gridSize &&
                posY < gridSize;
        }
    }
}