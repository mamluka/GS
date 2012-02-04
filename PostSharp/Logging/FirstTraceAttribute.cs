using System;
using PostSharp.Aspects;

namespace Trace
{
    // This aspect is the 'Hello, world' of aspect-oriented programming. It
    // does not implemement performance best practices.
    [Serializable]
    public sealed class FirstTraceAttribute : OnMethodBoundaryAspect
    {
        // This method is before target methods.
        public override void OnEntry(
            MethodExecutionArgs args )
        {
            System.Diagnostics.Trace.WriteLine( string.Format( 
                "Entering {0}.{1}.",
                args.Method.DeclaringType.FullName,
                args.Method.Name ));
            System.Diagnostics.Trace.Indent();
        }

        // This method is after target methods exit with success.
        public override void OnExit(
            MethodExecutionArgs args )
        {
            System.Diagnostics.Trace.Unindent();
            System.Diagnostics.Trace.WriteLine(
                string.Format( 
                "Leaving {0}.{1}.",
                args.Method.DeclaringType.FullName,
                args.Method.Name) );
        }

        // This method is after target methods exit with exception.
        public override void OnException( MethodExecutionArgs args )
        {
            System.Diagnostics.Trace.Unindent();
            System.Diagnostics.Trace.WriteLine( string.Format( 
                "Leaving {0}.{1} with exception {2}.", 
                args.Method.DeclaringType.FullName, 
                args.Method.Name,
                args.Exception.GetType().Name) );
        }
    }
}