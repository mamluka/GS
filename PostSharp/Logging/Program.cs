using System;
using System.Diagnostics;
using System.Threading;

namespace Trace
{
    internal class Program
    {
        private static void Main( string[] args )
        {
            System.Diagnostics.Trace.Listeners.Add( new TextWriterTraceListener( Console.Out ) );

            Method1();
            Factorial( 5 );
            Factorial( -5 );
        }


        [FirstTrace]
        private static void Method1()
        {
            Method2();
        }

        [FirstTrace]
        private static void Method2()
        {
            Method3();
        }

        [FirstTrace]
        private static void Method3()
        {
            Thread.Sleep( 10 );
        }

        [Trace]
        private static int Factorial( int i )
        {
            if ( i <= 1 && i >= -1 ) return i;
            if ( i < 0 )
            {
                return i*Factorial( i + 1 );
            }
            else
            {
                return i*Factorial( i - 1 );
            }
        }
    }
}