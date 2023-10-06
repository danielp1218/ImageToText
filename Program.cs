using System.Drawing;
using System.Runtime.InteropServices;

class imageProcesser
{
    static void Main(string[] args)
    {
        if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            Console.WriteLine("Only supports Windows :(");
            return;
        }

        Console.WriteLine("Insert image file path:");

        string path = Console.ReadLine();
        if (path[0] == '"')
        {
            path = path[1..^1];
        }
        using Bitmap image = new(Bitmap.FromFile(path));
        
        //check if worked
        Console.WriteLine(image.Height + " "+ image.Width);

        int height = 0;
        int width = 0;
        bool accepted = true;
        do
        {
            try
            {
                Console.Write("Width: ");
                width = int.Parse(Console.ReadLine());
                accepted = true;
            }
            catch (FormatException)
            {
                Console.WriteLine("bruh");
                accepted = false;
            }
        } while (!accepted);
        height = (image.Height * width)/image.Width;
        double[,] ascii = normalize(getShadeValue(image, height, width), height, width);

        image.Dispose();
        for(int y = 0; y < height; ++y)
        {
            string line = "";
            for(int x = 0; x < width; ++x)
            {
                if (ascii[y,x] >= 255)
                {
                    line += "  ";
                } else if(ascii[y,x] > 200)
                {
                    line += "..";
                } else if (ascii[y,x] > 175)
                {
                    line += "::";
                }
                else if(ascii[y,x] >150)
                {
                    line += "--";
                }
                else if (ascii[y,x] > 175)
                {
                    line += "==";
                }
                else if (ascii[y,x] > 100)
                {
                    line += "++";
                } else if (ascii[y,x] > 75)
                {
                    line += "II";
                } else if (ascii[y,x] > 50)
                {
                    line += "##";
                }
                else
                {
                    line += "@@";
                }
                
            }
            Console.WriteLine(line);
        }
        
        
    }
    static double[,] getShadeValue(Bitmap image, int height, int width)
    {
        double[,] arr = new double[height, width];
        int charWidth = image.Width / width;
        for(int y = 0; y < height; ++y)
        {
            for(int x = 0; x < width; ++x)
            {
                double totalGreyValue = 0.0;
                for(int i = 0; i < charWidth; ++i)
                {
                    for (int j = 0; j < charWidth; ++j)
                    {
                        totalGreyValue += greyValue(image.GetPixel(x*charWidth+j, y*charWidth+i));
                        
                    }
                }
                arr[y, x] = totalGreyValue/(charWidth*charWidth);
            }
        }
        return arr;
    }
    static double greyValue(Color color)
    {
        float R = color.R;
        float B = color.B;
        float G = color.G;
        float A = color.A;
        return (R+G+B)/3*(1-A)/255;
    }

    static double[,] normalize(double[,] arr, int height, int width)
    {
        double maxS = 0;
        double minS = Int32.MaxValue;
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                maxS = Math.Max(maxS, arr[y,x]);
                minS = Math.Min(minS, arr[y,x]);
            }
        }

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                arr[y, x] += 240-maxS;
                arr[y,x] *= 210 / (maxS - minS);
            }
        }
        return arr;
        
    }
}
