using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;

namespace DAN_XXXVI_AndrejaKolesar
{
    class Program
    {
        private static object locker = new object();
        public static int[,] matrix;
        public static List<int> list = new List<int>(10000);
        public static string fileName = "OddNumbers.txt";
        static Random rnd = new Random();

        /// <summary>
        /// Generate a matrix 100x100 with two-digits numbers from a list
        /// </summary>
        public static void MatrixGenerator()
        {
            lock (locker)
            {
                matrix = new int[100, 100];

                //Wait until list count is 10000
                while (list.Count < 10000)
                {
                    Monitor.Wait(locker);
                }
                //fill the matrix with numbers from the list
                for(int i = 0; i < 100; i++)
                {
                    for(int j = 0; j < 100; j++)
                    {
                        matrix[i, j] = list[j + i*100];
                    }
                }
            }
        }

        /// <summary>
        /// Generate a 10000 random numbers and add them into list
        /// </summary>
        public static void NumberGenerator()
        {
            int num;
            lock (locker)
            {
                //add random two-digits number to the list
                for(int i=0; i<10000; i++)
                {
                   num =  rnd.Next(10, 100);
                   list.Add(num);
                }
                //pulse that you have finished your job with generating numbers
                Monitor.Pulse(locker);
            }
        }

        public static void LogIntoFile()
        {
            //get only odd numbers from the matrix
            int[] arr = list.Where(i => i % 2 == 1).ToArray();

            lock (fileName)
            {
                using (StreamWriter sw = File.CreateText(fileName))
                {
                    //write this numbers into file OddNumbers.txt
                    for (int i = 0; i<arr.Length; i++)
                    {
                       sw.WriteLine(arr[i]);
                    }
                }
                Monitor.Pulse(fileName);
            }
        }

        //When OddNumbers.txt is finished with writing, read data and print them on console
        public static void ReadFromFile()
        {
            lock (fileName)
            {
                while (!File.Exists(fileName))
                {
                    Monitor.Wait(fileName);
                }
                using (StreamReader sr = File.OpenText(fileName))
                {
                    string s;
                    while ((s = sr.ReadLine()) != null)
                    {
                        Console.Write(s + " ");
                    }
                }
            }
        }

        static void Main(string[] args)
        {
            //If File with this fileName exist, delete it.
            if (File.Exists(fileName))
            {
                File.Delete(fileName);
            }

            Thread t1 = new Thread(MatrixGenerator)
            {
                Name = "Matrix_generator"
            };
            Thread t2 = new Thread(NumberGenerator)
            {
                Name = "Two_digits_number_generator"
            };
            //start first two threads
            t1.Start();
            t2.Start();
            //finish executing first two threads
            t1.Join();
            t2.Join();

            Thread t3 = new Thread(LogIntoFile)
            {
                Name = "odd_numbers_into_file"
            };
            Thread t4 = new Thread(ReadFromFile)
            {
                Name = "read_odd"
            };
            //start third and forth thread
            t3.Start();
            t4.Start();
            //join them
            t3.Join();
            t4.Join();
            Console.ReadKey();


        }
    }
}
