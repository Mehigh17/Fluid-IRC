using System;

namespace FluidIrc.Tooling
{
    class Program
    {

        /************************************************************************************************************
         * The sole purpose of this project is to make the EF Core Tooling library work.                            *
         * It doesn't fully support UWP yet so we need a dummy project to create the migrations.                    *
         *                                                                                                          *
         * !!! Other than that this project servers no purpose and shouldn't be deployed with the final build !!!    *
         ************************************************************************************************************/

        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");
        }
    }
}
