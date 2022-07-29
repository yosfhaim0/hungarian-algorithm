using System;


//minimum problam
namespace PairMatching.DomainModel.MatchingCalculations
{

    //rows=from israel colum = from world
    public class HungarianAlgoritem
    {
        //metrix whit the cost of each assignment
        int[,] _matrix;
        int _matrixSize;
        //If the need is maximum assignment
        bool _isMaxProblam;
        //for mark lines
        bool[] row_Mark;
        bool[] col_Mark;

        public HungarianAlgoritem(int[,] matrix, int matrixSize, bool isMaxProblam = false)
        {
            if (_matrix == null)
                throw new ArgumentNullException(nameof(matrix));
            if (matrixSize <= 0)
                throw new ArgumentOutOfRangeException(nameof(matrixSize));
            _matrix = matrix;
            _isMaxProblam = isMaxProblam;
            _matrixSize = matrixSize;
            row_Mark = new bool[_matrixSize];
            col_Mark = new bool[_matrixSize];

            if (_isMaxProblam)
                InitializeMatrixForMaxAssignment();

        }


        public int[] HungarianAlgorithm()
        {
            ReduceRows();
            ReduceColomns();
            int x = minimumVerticalOrHorizontalLines();
            while (x < _matrixSize)
            {
                shiftZeros();//find minimum uncoverd cell and subtract all uncoverd cells
                             //and add the minimum for crossing lines
                initRowsAndColMark();
                x = minimumVerticalOrHorizontalLines();
            }
            int[] indexes = makingTheFinalAssignment();
            return indexes;
        }


        //if you need maximum assignment Problem
        public void InitializeMatrixForMaxAssignment()
        {
            //find the maximum value
            var max = int.MinValue;
            for (int k = 0; k < _matrixSize; k++)
                for (int l = 0; l < _matrixSize; l++)
                    if (max < _matrix[k, l])
                    {
                        max = _matrix[k, l];
                    }
            //foreach cell in _metrix do max - cell
            for (int k = 0; k < _matrixSize; k++)
                for (int l = 0; l < _matrixSize; l++)
                    _matrix[k, l] = max - _matrix[k, l];
        }


        //The function finds a solution,
        //a solution is represented by an array of
        //column indexes meaning a solution is [i, indexResColumns[i]]
        public int[] makingTheFinalAssignment()
        {
            int[] indexResColumns = new int[_matrixSize];
            bool[] resRows = new bool[_matrixSize];
            bool[] resColumns = new bool[_matrixSize];
            int i = 0, j = 0;
            for (i = 0; i < _matrixSize;)
            {
                for (; j < _matrixSize; j++)
                {
                    if (!resRows[i] && !resColumns[j] && _matrix[i, j] == 0)
                    {
                        resColumns[j] = true;
                        resRows[i] = true;
                        indexResColumns[i] = j;
                    }
                }
                //If we reached the end of the row and did not
                //mark it as having found a solution, go back to the previous row
                //and mark a different zero (the one after it)
                if (!resRows[i])
                {

                    i = i - 1;
                    j = indexResColumns[i] + 1;
                    resRows[i] = false;
                    resColumns[indexResColumns[i]] = false;
                }
                //else continue the next row
                else
                {
                    j = 0;
                    i++;
                }
            }
            return indexResColumns;

        }

        //restart the line Marker
        private void initRowsAndColMark()
        {
            row_Mark = new bool[_matrixSize];
            col_Mark = new bool[_matrixSize];
        }

        //make another cell in the matrix to be 0
        private void shiftZeros()
        {
            //find minimun number uncover by line
            var min = int.MaxValue;
            for (int i = 0; i < _matrixSize; i++)
            {
                for (int j = 0; j < _matrixSize; j++)
                {
                    if (min > _matrix[i, j] && !row_Mark[i] && !col_Mark[j])
                    {
                        min = _matrix[i, j];
                    }
                }
            }
            //foreach cell: if uncover then subtract
            //              if cover by two line then add
            for (int i = 0; i < _matrixSize; i++)
            {
                for (int j = 0; j < _matrixSize; j++)
                {
                    if (!row_Mark[i] && !col_Mark[j])
                    {
                        _matrix[i, j] -= min;
                    }
                    else if (row_Mark[i] && col_Mark[j])
                    {
                        _matrix[i, j] += min;
                    }
                }
            }

        }

        //find the minimum  number of lines
        //vertical or horizontal that cover all the zero in the _metrix 
        public int minimumVerticalOrHorizontalLines()
        {
            bool flag = true;
            while (flag)
            {
                //find minimum zero uncover in each row
                int miniRowZeroContain = int.MaxValue;
                int indexOfRow = -1;
                for (int i = 0; i < _matrixSize; i++)
                {
                    int counter = 0;
                    for (int j = 0; j < _matrixSize && counter < miniRowZeroContain; j++)
                    {
                        if (_matrix[i, j] == 0 && !col_Mark[j] && !row_Mark[i])
                        {
                            counter++;
                        }
                    }
                    if (counter < miniRowZeroContain && counter != 0)
                    {
                        miniRowZeroContain = counter;
                        indexOfRow = i;
                    }
                }
                //find minimum zero uncover in each colum
                int miniColumZeroContain = int.MaxValue;
                int indexOfColum = -1;

                for (int i = 0; i < _matrixSize; i++)
                {
                    int counter = 0;
                    for (int j = 0; j < _matrixSize && counter < miniColumZeroContain; j++)
                    {
                        if (_matrix[j, i] == 0 && !row_Mark[j] && !col_Mark[i])
                        {
                            counter++;
                        }
                    }
                    if (counter < miniColumZeroContain && counter != 0)
                    {
                        miniColumZeroContain = counter;
                        indexOfColum = i;
                    }
                }
                //choose the minimum between miniRow and miniColum
                var minimu = miniRowZeroContain < miniColumZeroContain ? 0 : 1;
                // mark  a line foreach zero in row\colum vertical\horizontal
                if (minimu == 0)
                {
                    for (int i = 0; i < _matrixSize; i++)
                        if (_matrix[indexOfRow, i] == 0)
                            col_Mark[i] = true;
                }
                else
                {
                    for (int i = 0; i < _matrixSize; i++)
                        if (_matrix[i, indexOfColum] == 0)
                            row_Mark[i] = true;
                }
                //chack if there is zero uncover by line
                //O(n^2)
                flag = false;
                for (int k = 0; k < _matrixSize && !flag; k++)
                    for (int l = 0; l < _matrixSize && !flag; l++)
                        if (_matrix[k, l] == 0 && !row_Mark[k] && !col_Mark[l])
                            flag = true;
            }
            //Check how many vertical or horizontal lines were
            //needed to cover all the zeros
            var sumLines = 0;
            foreach (var i in row_Mark)
            {
                if (i)
                    sumLines++;
            }
            foreach (var i in col_Mark)
            {
                if (i)
                    sumLines++;
            }
            return sumLines;


        }




        //make a zero in each colomns
        private void ReduceColomns()
        {
            //For each column, reduce all the cells in it by the minimum number in that column
            for (int i = 0; i < _matrixSize; i++)
            {
                int minCol = _matrix[0, i];
                for (int j = 1; j < _matrixSize; j++)
                {
                    if (_matrix[j, i] < minCol)
                    {
                        minCol = _matrix[j, i];
                    }
                }
                for (int j = 0; j < _matrixSize; j++)
                {
                    _matrix[j, i] -= minCol;
                }
            }
        }
        //make a zero in each row
        private void ReduceRows()
        {
            //For each row, reduce all the cells in it by the minimum number in that row
            for (int i = 0; i < _matrixSize; i++)
            {
                int minRow = _matrix[i, 0];
                for (int j = 1; j < _matrixSize; j++)
                {
                    if (_matrix[i, j] < minRow)
                    {
                        minRow = _matrix[i, j];
                    }
                }
                for (int j = 0; j < _matrixSize; j++)
                {
                    _matrix[i, j] -= minRow;
                }
            }
        }



    }
}
