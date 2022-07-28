using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace PairMatching.DomainModel.MatchingCalculations
{
    public class HungarianAlgoritem
    {   
        int[,] _matrix;
        bool _isMaxProblam;
        bool[] row_Mark;
        bool[] col_Mark;
        bool[] resRows;
        bool[] resColumns;
        int[] indexResColumns;
        int _matrixSize;

        public HungarianAlgoritem(int[,] matrix, int matrixSize, bool isMaxProblam = false)
        {
            _matrix = matrix;
            _isMaxProblam = isMaxProblam;
            _matrixSize = matrixSize;
            row_Mark = new bool[_matrixSize];
            col_Mark = new bool[_matrixSize];
            resRows = new bool[_matrixSize];
            resColumns = new bool[_matrixSize];
            indexResColumns = new int[_matrixSize];

            if (_isMaxProblam)
                isMaximum();

        }
        public void isMaximum()
        {
            var max = int.MinValue;
            for (int k = 0; k < _matrixSize; k++)
                for (int l = 0; l < _matrixSize; l++)
                    if (max < _matrix[k, l])
                    {
                        max = _matrix[k, l];
                    }
            for (int k = 0; k < _matrixSize; k++)
                for (int l = 0; l < _matrixSize; l++)
                    _matrix[k, l] = max - _matrix[k, l];
        }

        public int[] HungarianAlgorithm()
        {
            ReduceRows();
            ReduceColomns();
            var x = minimumVerticalOrHorizontalLines();
            if (x < _matrixSize)
            {
                shiftZero();//find minimum uncoverd cell and subtract all uncoverd cells
                            //and add the minimum for crossing lines
                initRowsAndColMark();
                x = minimumVerticalOrHorizontalLines();
            }
            int[] indexes = findPair();
            return indexes;
        }

        public int[] findPair()
        {
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
                if (!resRows[i])
                {

                    i = i - 1;
                    j = indexResColumns[i] + 1;
                    resRows[i] = false;
                    resColumns[indexResColumns[i]] = false;
                }
                else
                {
                    j = 0;
                    i++;
                }
            }
            return indexResColumns;

        }
        private void initRowsAndColMark()
        {
            row_Mark = new bool[_matrixSize];
            col_Mark = new bool[_matrixSize];
        }
        private void shiftZero()
        {
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

        public int minimumVerticalOrHorizontalLines()
        {
            bool flag = true;
            while (flag)
            {
                int miniRowZeroContain = int.MaxValue;
                int indexOfRow = -1;
                for (int i = 0; i < _matrixSize; i++)
                {
                    int counter = 0;
                    for (int j = 0; j < _matrixSize; j++)
                    {
                        if (_matrix[i, j] == 0 && !col_Mark[j])
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

                int miniColumZeroContain = int.MaxValue;
                int indexOfColum = -1;
                for (int i = 0; i < _matrixSize; i++)
                {
                    int counter = 0;
                    for (int j = 0; j < _matrixSize; j++)
                    {
                        if (_matrix[j, i] == 0 && !row_Mark[j])
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
                var minimu = miniRowZeroContain < miniColumZeroContain ? 0 : 1;
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
                for (int k = 0; k < _matrixSize; k++)
                    for (int l = 0; l < _matrixSize; l++)
                        if (_matrix[k, l] == 0 && !row_Mark[k] && !col_Mark[l])
                            goto MORE;
                flag = false;
            MORE:;
            }

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





        private void ReduceColomns()
        {
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

        private void ReduceRows()
        {
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
