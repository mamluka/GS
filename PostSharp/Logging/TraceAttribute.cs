using System;
using System.Reflection;
using System.Text;
using PostSharp.Aspects;

namespace Trace
{
    // This aspect traces the execution of methods including the value of parameters and return value,
    // implementing performance best practices. Note that reflection is not used at runtime.
    [Serializable]
    public sealed class TraceAttribute : OnMethodBoundaryAspect
    {
        // This field is assigned and serialized at build time, then deserialized and read at runtime.
        private string methodName;

        // This method is executed at build-time, inside PostSharp.
        public override void CompileTimeInitialize( MethodBase method, AspectInfo aspectInfo )
        {
            // Computes the field value at build-time so that reflection is not necessary at runtime.
            this.methodName = method.DeclaringType.FullName + "." + method.Name;
        }

        // This method is executed at runtime inside your application,
        // before target methods.
        public override void OnEntry( MethodExecutionArgs args )
        {
            // Create the string based on the current context.
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.Append( "+ Entering " );
            AppendContext( stringBuilder, args.Instance, args.Arguments );

            // Trace the string.
            System.Diagnostics.Trace.WriteLine( stringBuilder.ToString() );
            System.Diagnostics.Trace.Indent();

            // Store the StringBuilder in a 'magic' location so that it can be used later in 
            // the same target method.
            args.MethodExecutionTag = stringBuilder;
        }

        // This method is executed at runtime inside your application,
        // when target methods exit with success.
        public override void OnSuccess( MethodExecutionArgs args )
        {
            // Take the StringBuilder from where we stored it in OnEntry,
            // and initialize it.
            StringBuilder stringBuilder = (StringBuilder) args.MethodExecutionTag;
            stringBuilder.Length = 0;

            // Create the trace string.
            stringBuilder.Append( "- Exiting " );
            AppendContext( stringBuilder, args.Instance, args.Arguments );
            stringBuilder.Append( " with value " );
            AppendObject( stringBuilder, args.ReturnValue );

            // Add the string to the trace.
            System.Diagnostics.Trace.Unindent();
            System.Diagnostics.Trace.WriteLine( stringBuilder.ToString() );
        }

        // This method is executed at runtime inside your application,
        // when target methods exit with an exception.
        public override void OnException( MethodExecutionArgs args )
        {
            // Take the StringBuilder from where we stored it in OnEntry,
            // and initialize it.
            StringBuilder stringBuilder = (StringBuilder) args.MethodExecutionTag;
            stringBuilder.Length = 0;

            // Create the trace string.
            stringBuilder.Append( "! Exiting " );
            AppendContext( stringBuilder, args.Instance, args.Arguments );
            stringBuilder.Append( " with exception " );
            stringBuilder.Append( args.Exception.GetType().Name );
            stringBuilder.Append( ": " );
            stringBuilder.Append( args.Exception.Message );

            // Add the string to the trace.
            System.Diagnostics.Trace.Unindent();
            System.Diagnostics.Trace.WriteLine( stringBuilder.ToString() );
        }

        private void AppendContext( StringBuilder builder, object instance, Arguments arguments )
        {
            builder.Append( this.methodName );
            builder.Append( '(' );

            bool comma = false;
            if ( instance != null )
            {
                builder.Append( "this=" );
                AppendObject( builder, instance );
                comma = true;
            }

            for ( int i = 0; i < arguments.Count; i++ )
            {
                if ( comma )
                    builder.Append( ", " );
                else
                    comma = true;

                AppendObject( builder, arguments[i] );
            }
            builder.Append( ')' );
        }

        private static void AppendObject( StringBuilder builder, object obj )
        {
            if ( obj == null )
            {
                builder.Append( "null" );
            }
            else
            {
                builder.Append( '{' );
                builder.Append( obj.ToString() );
                builder.Append( '}' );
            }
        }
    }
}