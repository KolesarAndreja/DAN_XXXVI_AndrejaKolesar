using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace DAN_XXXVI_AndrejaKolesar
{
    class Program
    {
        private static object locker = new object();
        public static int[,] matrix;
        public static List<int> list = new List<int>(10000);
        static Random rnd = new Random();

        public static void MatrixGenerator()
        {
            lock (locker)
            {
                matrix = new int[100, 100];
                while (list.Count < 10000)
                {
                    Monitor.Wait(locker);
                }
                for(int i = 0; i < 100; i++)
                {
                    for(int j = 0; j < 100; j++)
                    {
                        matrix[i, j] = list[j + i*100];
                    }
                }
            }
        }

        public static void NumberGenerator()
        {
            int num;
            lock (locker)
            {
                for(int i=0; i<10000; i++)
                {
                   num =  rnd.Next(10, 100);
                   list.Add(num);
                }
                Monitor.Pulse(locker);
            }
        }

        static void Main(string[] args)
        {
            Thread t1 = new Thread(MatrixGenerator)
            {
                Name = "Matrix_generator"
            };
            Thread t2 = new Thread(NumberGenerator)
            {
                Name = "Two_digits_number_generator"
            };

            t1.Start();
            t2.Start();

            t1.Join();
            t2.Join();

            for(int i=0; i<60; i++)
            {
                for(int j = 0; j<60; j++)
                {
                    Console.Write( matrix[i,j] + " ");
                }
                Console.WriteLine();
            }

            Console.ReadKey();


        }
    }
}
