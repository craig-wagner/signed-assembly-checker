using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace SignedAssemblyChecker
{
    class Program
    {
        static void Main( string[] args )
        {
            if( args.Length == 0 || !Directory.Exists( args[0] ) )
            {
                Console.WriteLine( "You must provide a valid folder path on the command line." );
            }
            else
            {
                try
                {
                    CheckAssembliesInFolder( args[0] );
                }
                catch( Exception ex )
                {
                    Console.WriteLine( ex.ToString() );
                }
            }

            Console.WriteLine();
            Console.Write( "Press any key to continue..." );
            Console.ReadKey();
        }

        private static void CheckAssembliesInFolder( string folderToCheck )
        {
            string[] files = Directory.GetFiles( folderToCheck, "*.dll" );
            List<string> signedAssemblies = new List<string>();
            List<string> unsignedAssemblies = new List<string>();
            List<string> invalidAssemblies = new List<string>();

            Console.WriteLine( "Checking assemblies in {0}", folderToCheck );
            Console.WriteLine();

            foreach( string file in files )
            {
                Assembly assembly = null;

                try
                {
                    assembly = Assembly.LoadFrom( file );
                }
                catch( BadImageFormatException )
                {
                    invalidAssemblies.Add( file );
                }

                if( assembly != null )
                {
                    AssemblyName assemblyName = assembly.GetName();
                    byte[] key = assemblyName.GetPublicKey();
                    bool isSigned = key.Length > 0;

                    if( isSigned )
                    {
                        signedAssemblies.Add( file );
                    }
                    else
                    {
                        unsignedAssemblies.Add( file );
                    }
                }
            }

            if( invalidAssemblies.Count > 0 )
            {
                Console.WriteLine( "Invalid Assemblies" );
                Console.WriteLine();

                foreach( string file in invalidAssemblies.OrderBy( s => s ) )
                {
                    Console.WriteLine( Path.GetFileName( file ) );
                }

                Console.WriteLine();
            }

            if( signedAssemblies.Count > 0 )
            {
                Console.WriteLine( "Signed Assemblies" );
                Console.WriteLine();

                foreach( string file in signedAssemblies.OrderBy( s => s ) )
                {
                    Console.WriteLine( Path.GetFileName( file ) );
                }

                Console.WriteLine();
            }

            if( unsignedAssemblies.Count > 0 )
            {
                Console.WriteLine( "Unsigned Assemblies" );
                Console.WriteLine();

                foreach( string file in unsignedAssemblies.OrderBy( s => s ) )
                {
                    Console.WriteLine( Path.GetFileName( file ) );
                }
            }
        }
    }
}
