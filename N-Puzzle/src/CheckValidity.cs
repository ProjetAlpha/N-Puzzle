using System.Collections;

namespace NPuzzle.src
{
    public static class CheckValidity
    {

        /* This method sorts the input array and returns the 
       number of inversions in the array */
        static int mergeSort(int[] arr, int array_size)
        {
            int[] temp = new int[array_size];
            return _mergeSort(arr, temp, 0, array_size - 1);
        }

        /* An auxiliary recursive method that sorts the input array and 
          returns the number of inversions in the array. */
        static int _mergeSort(int[] arr, int[] temp, int left, int right)
        {
            int mid, inv_count = 0;
            if (right > left)
            {
                /* Divide the array into two parts and call _mergeSortAndCountInv() 
               for each of the parts */
                mid = (right + left) / 2;

                /* Inversion count will be the sum of inversions in left-part, right-part 
              and number of inversions in merging */
                inv_count += _mergeSort(arr, temp, left, mid);
                inv_count += _mergeSort(arr, temp, mid + 1, right);

                /*Merge the two parts*/
                inv_count += merge(arr, temp, left, mid + 1, right);
            }
            return inv_count;
        }

        /* This method merges two sorted arrays and returns inversion count in 
           the arrays.*/
        static int merge(int[] arr, int[] temp, int left, int mid, int right)
        {
            int i, j, k;
            int inv_count = 0;

            i = left; /* i is index for left subarray*/
            j = mid; /* j is index for right subarray*/
            k = left; /* k is index for resultant merged subarray*/
            while ((i <= mid - 1) && (j <= right))
            {
                if (arr[i] <= arr[j])
                {
                    temp[k++] = arr[i++];
                }
                else
                {
                    temp[k++] = arr[j++];

                    /*this is tricky -- see above explanation/diagram for merge()*/
                    inv_count = inv_count + (mid - i);
                }
            }

            /* Copy the remaining elements of left subarray 
           (if there are any) to temp*/
            while (i <= mid - 1)
                temp[k++] = arr[i++];

            /* Copy the remaining elements of right subarray 
           (if there are any) to temp*/
            while (j <= right)
                temp[k++] = arr[j++];

            /*Copy back the merged elements to original array*/
            for (i = left; i <= right; i++)
                arr[i] = temp[i];

            return inv_count;
        }

        // Tell how far (or close) the array is from being sorted.
        private static int GetInvCount(int[][] puzzle, int size)
        {
            int[] arrayMap = new int[size * size];
            for (int i = 0; i < size; i++)
            {
                for (int j = 0; j < size; j++)
                {
                    // if (puzzle[i][j] != 0)
                    //if (puzzle[i][j] != 0)
                    arrayMap[size * i + j] = puzzle[i][j];
                }
            }

            return mergeSort(arrayMap, size * size);
        }

        private static int FindZeroPosX(int[][] puzzle, int size)
        {
            for (int i = size - 1; i >= 0; i--)
                for (int j = size - 1; j >= 0; j--)
                    if (puzzle[i][j] == 0)
                        return size - i;
            return 0;
        }

        public static bool IsSolvable(int [][] puzzle, int size)
        {
            // Count inversions in given puzzle
            int invCount = GetInvCount(puzzle, size);

            return (invCount & 1) == 0;
            // If grid is odd, return true if inversion
            // count is even.
            /*if ((size & 1) != 0)
                return (invCount & 1) == 0; // i.e. invCount % 2 == 0
            else     // grid is even 
            {
                int pos = FindZeroPosX(puzzle, size);
                if ((pos & 1) != 0)
                    return (invCount & 1) == 0;
                else
                    return (invCount & 1) != 0;
            }*/
        }
    }
}
