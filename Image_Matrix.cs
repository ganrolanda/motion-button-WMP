using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace WindowsFormsApplication2
{
    static class Extensions
    {
        static public string ToThreeDigits(this byte integer)
        {
            int digits = 0;
            int temp = (int)integer;
            do
            {
                temp /= 10;
                digits++;
            }
            while (temp != 0);
            string ThreeDigits = "";
            if (digits == 1)
            {
                ThreeDigits = "00"+Convert.ToString(integer);
            }
            else if (digits == 2)
            {
                ThreeDigits = "0" + Convert.ToString(integer);
            }
            else
            {
                ThreeDigits = Convert.ToString(integer);
            }
            return ThreeDigits;
        }
    }
    class ImageMatrix
    {
        public byte[,] function;
        int width;
        int height;

        public ImageMatrix(Bitmap image)//constructor
        {
            this.width = image.Width;
            this.height = image.Height;

            function = new byte[width, height];//define bounds of function

            //unsafe bitmap to iterate through the image
            unSafeBitmap usb = new unSafeBitmap(image);
            usb.LockImage();

            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    /* Reading average (gray scale) value for each of the pixel
                     * and storing it in function*/
                    this.function[i, j] = usb.GetAverage(i, j);
                }
            }
        }

        public ImageMatrix(byte[,] matrix)//constructor
        {
            //storing value to member variables
            this.width = matrix.GetLength(0);
            this.height = matrix.GetLength(1);

            //initializing function[,]
            function = new byte[width, height];
            
            //copy this matrix to function array
            Array.Copy(matrix, this.function, matrix.Length);
        }

        public Bitmap ToImage()//to convert imagematrix to 
        {
            Bitmap image = new Bitmap(width, height);

            unSafeBitmap usb = new unSafeBitmap(image);//making object for unsafe bitmapping
            usb.LockImage();
            
            //making a grayscale image according to function array
            for(int i=0;i<width;i++)
            {
                for(int j=0;j<height;j++)
                {
                    //writign each pixel on image bitmap.
                    byte Value = function[i, j];
                    Color c = Color.FromArgb(Value, Value, Value);//gray scale image
                    usb.SetPixel(i, j, c);
                }
            }

            //unlock image before returning 
            usb.UnlockImage();

            return image;
        }

        public override string ToString()
        {
            //writing an image to a text or in matrix format
            StringBuilder sb = new StringBuilder();//to concatinate string to eeach other

            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    //getting function and making it a string
                    if (j != height - 1)
                    {
                        sb.Append(function[i, j].ToThreeDigits() + ", ");
                    }
                    else
                    {
                        sb.Append(function[i, j].ToThreeDigits());
                    }
                }
                sb.Append("\n\n");
            }
            return sb.ToString();
        }

        public ImageMatrix HorizontalSqueeze(int SqueezeFactor)
        {
            // assignimg new width for squeezing the image horizontally
            int NewWidth = width/SqueezeFactor;
            if(NewWidth*SqueezeFactor != width)
            {
                NewWidth++;// if it's fully divisible than add another horizontal row
            }

            // defining new function this will be new image(horizontally squeezed)
            byte[,] NewFunction = new byte[NewWidth, height];
            
            //iterating through this.function[,] and adding value to newFunction[,];
            for (int i = 0; i < NewWidth; i++)
            {
                int startingPoint = i * SqueezeFactor;
                int horizontalBound = SqueezeFactor + startingPoint;
                if (i == NewWidth - 1)//in the end make sure it's not out of the bound of an defined array!
                {
                    horizontalBound = width;
                }
                for (int j = 0; j < height; j++)
                {
                    double averageFunction = 0;
                    for (int h = startingPoint; h < horizontalBound; h++)
                    {
                        averageFunction += function[h, j];
                    }
                    averageFunction /= SqueezeFactor;
                    NewFunction[i, j] = (byte)averageFunction;
                }
            }

            //return iamgematrix
            ImageMatrix IM = new ImageMatrix(NewFunction);
            return IM;
        }

        public ImageMatrix VerticalSqueeze(int SqueezeFactor)
        {
            // assignimg new width for squeezing the image horizontally
            int NewHeight = height / SqueezeFactor;
            if (NewHeight * SqueezeFactor != height)
            {
                NewHeight++;// if it's fully divisible than add another horizontal row
            }

            // defining new function this will be new image(horizontally squeezed)
            byte[,] NewFunction = new byte[width, NewHeight];

            //iterating through this.function[,] and adding value to newFunction[,];
            for (int i = 0; i < NewHeight; i++)
            {
                int startingPoint = i * SqueezeFactor;
                int verticalBound = SqueezeFactor + startingPoint;
                if (i == NewHeight - 1)//in the end make sure it's not out of the bound of an defined array!
                {
                    verticalBound = height;
                }
                for (int j = 0; j < width; j++)
                {
                    double averageFunction = 0;
                    for (int h = startingPoint; h < verticalBound; h++)
                    {
                        averageFunction += function[j, h];
                    }
                    averageFunction /= SqueezeFactor;
                    NewFunction[j, i] = (byte)averageFunction;
                }
            }

            //return iamgematrix
            ImageMatrix IM = new ImageMatrix(NewFunction);
            return IM;
        }

    }
}
