using System;
using UnityEngine;

public class Matrix
{
    // Start is called before the first frame update
    //public ArticulationJacobian matrix;
    float[] matrix;
    public int columnsNb;
    public int rowsNb;

    public float this[int row, int col]
    {
        get
        {
            if (row < 0 || row >= rowsNb || col < 0 || col >= columnsNb)
            {
                throw new IndexOutOfRangeException();
            }

            return matrix[row * columnsNb + col];
        }
        set
        {
            if (row < 0 || row >= rowsNb || col < 0 || col >= columnsNb)
            {
                throw new IndexOutOfRangeException();
            }

            matrix[row * columnsNb + col] = value;
        }
    }

    public Matrix(int row, int column)
    {
        matrix = new float[row * column];
        columnsNb = column;
        rowsNb = row;
    }

    static public Matrix GetIdentityMatrix()
    {
        Matrix Identity = new Matrix(3, 3);
        Identity[0, 0] = 1.0f;
        Identity[1, 1] = 1.0f;
        Identity[2, 2] = 1.0f;
        return Identity;
    }

    static public Matrix MultiplyMatrices(Matrix A, Matrix B)
    {
        if (A.columnsNb != B.rowsNb) return new Matrix(0,0);

        Matrix Result = new Matrix(A.rowsNb, B.columnsNb);
        for (int row = 0; row < Result.rowsNb; row++)
        {
            for (int col = 0; col < Result.columnsNb; col++)
            {
                for (int i = 0; i < A.columnsNb; i++)
                {
                    Result[row, col] += A[row, i] * B[i, col];
                }
            }
        }

        return Result;
    }

    static public float[] MultiplyVector(Matrix A, Vector3 B)
    {
        if (A.columnsNb != 3)
        {
            throw new IndexOutOfRangeException();
        }

        float[] Result = new float[A.rowsNb];
        for(int row = 0;row < A.rowsNb; row++)
        {
            for(int col = 0; col < A.columnsNb; col++)
            {
                Result[row] += A[row, col] * B[col];
            }
        }

        return Result;

    }

    static public Matrix GetTranspose(Matrix Mat)
    {
        Matrix Result = new Matrix(Mat.columnsNb, Mat.rowsNb);

        for (int row = 0; row < Mat.rowsNb; row++)
        {
            for (int col = 0; col < Mat.columnsNb; col++)
            {
                Result[col, row] = Mat[row, col];
            }
        }

        return Result;
    }
}

