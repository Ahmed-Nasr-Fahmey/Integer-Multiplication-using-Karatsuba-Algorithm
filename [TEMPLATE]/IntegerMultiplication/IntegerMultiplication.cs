using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Problem
{
    // *****************************************
    // DON'T CHANGE CLASS OR FUNCTION NAME
    // YOU CAN ADD FUNCTIONS IF YOU NEED TO
    // *****************************************
    public static class IntegerMultiplication
    {
        #region YOUR CODE IS HERE

        //Your Code is Here:
        //==================
        /// <summary>
        /// Multiply 2 large integers of N digits in an efficient way [Karatsuba's Method]
        /// </summary>
        /// <param name="X">First large integer of N digits [0: least significant digit, N-1: most signif. dig.]</param>
        /// <param name="Y">Second large integer of N digits [0: least significant digit, N-1: most signif. dig.]</param>
        /// <param name="N">Number of digits (power of 2)</param>
        /// <returns>Resulting large integer of 2xN digits (left padded with 0's if necessarily) [0: least signif., 2xN-1: most signif.]</returns>
        /// 
        
        static public byte[] IntegerMultiply(byte[] X, byte[] Y, int N)
        {
            //REMOVE THIS LINE BEFORE START CODING
            //throw new NotImplementedException();

            
            if (N <= 128)                                  
            {   
                return NaiveIntegerMultiply(X,Y);
            }
            
            if(X.Length != Y.Length)  // Make sure that two arrays are even and the same size
            {
                ResizeAndMakeTwoArraysEven(ref X, ref Y); // resize all arrays except base case
            }

            if(N % 2 != 0)  // Make sure that two arrays and N are even
            {
                N++;
                ResizeAndMakeTwoArraysEven(ref X, ref Y);
            }
            int mid = N / 2;

            byte[] A = new byte[mid];

            byte[] B = new byte[mid];

            byte[] C = new byte[mid];

            byte[] D = new byte[mid];
            
            for (int i = 0; i < mid; i++) // Copy values from X and Y arrays to A, B, C, D arrays 
            {
                A[i] = X[i];
                B[i] = X[i + mid];
                C[i] = Y[i];
                D[i] = Y[i + mid];
            }
            

            Task<byte[]> M1 = new Task<byte[]>(() => IntegerMultiply(A, C, mid)); // Calcolat A x C
            Task<byte[]> M2 = new Task<byte[]>(() => IntegerMultiply(B, D, mid)); // Calcolat B x D

            Task<byte[]> taskAPlusB = new Task<byte[]>(() => AddTwoArrays(ref A, B)); // Calcolat A + B
            Task<byte[]> taskCPlusD = new Task<byte[]>(() => AddTwoArrays(ref C, D)); // Calcolat C + D

            M1.Start(); M2.Start(); taskAPlusB.Start(); taskCPlusD.Start();
            M1.Wait(); M2.Wait(); taskAPlusB.Wait(); taskCPlusD.Wait();
            
            byte[] APlusB = taskAPlusB.Result; // actually return even
            byte[] CPlusD = taskCPlusD.Result;
            
            if (APlusB.Length > mid || CPlusD.Length > mid || APlusB.Length != CPlusD.Length) 
            {
                ResizeAndMakeTwoArraysEven(ref APlusB, ref CPlusD); // Make sure that two arrays are even and the same size
            }
            
            byte[] Z = IntegerMultiply(APlusB, CPlusD, APlusB.Length); // Calcolat (A + B) x (C + D)


            byte[] AMultiDPlusBMultiC = SubtractTwoArrays(SubtractTwoArrays(Z,  M1.Result), M2.Result); // Calcolat Z - M1 - M2 ==> (A x D + B x C)
            
            byte [] multiplicationArray = new byte[2 * N];
            
             
            int carry = 0;
            int sumation = 0;
            for (int i = 0; i < 2 * N; i++)
            {
                sumation = carry;
                if (i < M1.Result.Length)  // Fill M1 
                {
                    sumation += M1.Result[i];
                }
                if (i < mid + AMultiDPlusBMultiC.Length && i >= mid) // shift AMultiDPlusBMultiC bt N / 2 digits And sum it  to multiplicationArray
                {
                    sumation += AMultiDPlusBMultiC[i - mid];
                }
                if (i <= 2 * N && i >= N)           // shift M2 bt N digits And sum it  to multiplicationArray
                {
                    sumation += M2.Result[i - mid - mid];
                }
                multiplicationArray[i] = (byte)(sumation % 10);
                carry = sumation / 10;
            }

            return multiplicationArray; 
        }
        
        // ------------------ Better in small inputs -----------------
        
        static public byte[] NaiveIntegerMultiply(byte[] firstArray, byte[] secoundArray)
        {
            int length = Math.Max(firstArray.Length, secoundArray.Length);
            byte[] MultiplicationArray = new byte[2 * length]; 
            int carry = 0;
            int counter = 0;
            int multiplication = 0;
            for (int j = 0; j < length; j++)
            {
                
                carry = 0;
                counter = j; 
                multiplication = 0;
                
                for (int i = 0; i < length; i++)
                {
                    multiplication = firstArray[i] * secoundArray[j] + carry + MultiplicationArray[counter]; // Multiply and sum
                    MultiplicationArray[counter++] = (byte)(multiplication % 10);
                    carry = multiplication / 10;
                }
                MultiplicationArray[j + length] = (byte)carry;
            }
            
            return MultiplicationArray;
        }

        
        static public void ResizeAndMakeTwoArraysEven(ref byte[] firstArray, ref byte[] secoundArray)
        {
            
            if (firstArray.Length < secoundArray.Length)  // Make sure that secoundArray is largest and even 
            {
                if (secoundArray.Length % 2 != 0)
                {
                    Array.Resize(ref secoundArray, secoundArray.Length + 1); // make Y even
                    Array.Resize(ref firstArray, secoundArray.Length);
                }
                else
                {
                    Array.Resize(ref firstArray, secoundArray.Length);
                }
            }
            else if (firstArray.Length > secoundArray.Length) // Make sure that firstArray is largest and even 
            {
                if (firstArray.Length % 2 != 0)
                {
                    Array.Resize(ref firstArray, firstArray.Length + 1);
                    Array.Resize(ref secoundArray, firstArray.Length);
                }
                else
                {
                    Array.Resize(ref secoundArray, firstArray.Length);
                }
            }
            else if (firstArray.Length == secoundArray.Length) // Make sure that firstArray is largest and even
            {
                if (firstArray.Length % 2 != 0)
                {
                    Array.Resize(ref firstArray, firstArray.Length + 1);
                    Array.Resize(ref secoundArray, secoundArray.Length + 1);
                }
            }
        }
        
        public static byte[] AddTwoArrays(ref byte[] firstArray, byte[] secoundArray)
        {
            int length = Math.Max(firstArray.Length, secoundArray.Length);
            byte[] sumationArray = new byte[length];
            int carry = 0;
            int sumation = 0;
            
            for (int i = 0; i < length; i++)  
            {
                sumation = carry + firstArray[i] + secoundArray[i];  // total sum
                
                sumationArray[i] = (byte)(sumation % 10);
                carry = sumation / 10;
            }
            if (carry != 0) // make sure handleing carry
            {
                byte[] carryDigit = new byte[1];

                carryDigit[0] = (byte)carry;
                return sumationArray.Concat(carryDigit).ToArray(); // resive the result and set carry
            }
            return sumationArray;
        }

        public static byte[] SubtractTwoArrays(byte[] firstArray, byte[] secoundArray)
        {
            int length = Math.Max(firstArray.Length, secoundArray.Length);
            byte[] subtractionArray = new byte[length];
            int subtraction = 0;
            byte borrow = 0;
            if(firstArray.Length > secoundArray.Length)  // resize secound array
            {
                Array.Resize(ref secoundArray, firstArray.Length);
            }
            else if (firstArray.Length < secoundArray.Length) // resize firest array
            {
                Array.Resize(ref firstArray, secoundArray.Length);
            }
            for (int i = 0; i < length; i++)
            {
                subtraction = firstArray[i] - secoundArray[i] - borrow;  // total sub
                
                if (firstArray[i] < secoundArray[i] + borrow) // handleing borrow 
                {
                    subtractionArray[i] = (byte)(subtraction + 10);
                    borrow = 1;
                }
                else
                {
                    subtractionArray[i] = (byte)(subtraction);
                    borrow = 0;
                }
            }
            return subtractionArray;
        }

        #endregion
    }
}
