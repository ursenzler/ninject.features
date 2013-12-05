using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FeatureDumper
{
    using System.Diagnostics;
    using System.IO;
    using System.Management.Instrumentation;
    using System.Reflection;

    using Ninject.Features;

    class Program
    {
        private static string assemblyFolder;

        private static Assembly MyResolveEventHandler(object sender, ResolveEventArgs args)
        {
            var assembly = Assembly.LoadFrom(Path.Combine(assemblyFolder, args.Name.Substring(0, args.Name.IndexOf(",")) + ".dll"));

            return assembly;
        }

        static void Main(string[] args)
        {
            if (args.Length != 2)
            {
                Console.WriteLine("usage: FeatureDumper <outputFile .tgf> <folder with assemblies>");
                return;
            }

            string outputPath = args[0];
            assemblyFolder = args[1];

            Console.WriteLine("loading assemblies from " + assemblyFolder);

            var assemblyPaths = Directory.EnumerateFiles(assemblyFolder, "*.exe").Union(Directory.EnumerateFiles(assemblyFolder, "*.dll"));

            Console.WriteLine("loading assemblies;");
            foreach (var assemblyPath in assemblyPaths)
            {
                Console.WriteLine(assemblyPath);
            }

            AppDomain currentDomain = AppDomain.CurrentDomain;
            currentDomain.AssemblyResolve += new ResolveEventHandler(MyResolveEventHandler);


            IEnumerable<Assembly> assemblies = assemblyPaths.Select(Assembly.LoadFile).ToList();



            foreach (Assembly assembly in assemblies)
            {
                AddAssemblies(assembly, loadedAssemblies);
            }

            foreach (Assembly assembly in assemblies)
            {
                ProcessAssembly(assembly);
            }

            using (StreamWriter writer = new StreamWriter(outputPath))
            {
                List<Type> list = new List<Type>(allFeatures);
                foreach (var type in list)
                {
                    writer.WriteLine(list.IndexOf(type) + " " + type.Name);
                }

                writer.WriteLine("#");

                foreach (var dependency in chain)
                {
                    writer.WriteLine(list.IndexOf(dependency.Item1) + " " + list.IndexOf(dependency.Item2));
                }
            }

            Process yEd = new Process();
            yEd.StartInfo.FileName = @"C:\Program Files (x86)\yWorks\yEd\yEd.exe";
            yEd.StartInfo.Arguments = outputPath;
            yEd.Start();

            Console.WriteLine("done output file is " + outputPath);
        }

        static HashSet<Tuple<Type, Type>> chain = new HashSet<Tuple<Type, Type>>();
        static HashSet<Type> allFeatures = new HashSet<Type>();

        private static void ProcessAssembly(Assembly assembly)
        {
            var features = assembly.GetExportedTypes().Where(type => type.IsSubclassOf(typeof(Feature)));

            foreach (Type feature in features)
            {
                ProcessFeature(feature);
            }
        }

        private static void ProcessFeature(Type feature)
        {
            Console.WriteLine("found feature " + feature.Name);

            var constructor = feature.GetConstructors().OrderBy(c => c.GetParameters().Length).First();

            object[] arguments = new object[constructor.GetParameters().Length];

            var instance = (Feature)Activator.CreateInstance(feature, arguments);

            allFeatures.Add(feature);

            IList<Feature> neededFeatures = instance.NeededFeatures.ToList();
            foreach (Feature neededFeature in neededFeatures)
            {
                Console.WriteLine("needed features:");
                Console.WriteLine("- " + neededFeature.GetType().Name);

                chain.Add(new Tuple<Type, Type>(feature, neededFeature.GetType()));

                ProcessFeature(neededFeature.GetType());
            }
        }

        static List<Assembly> loadedAssemblies = new List<Assembly>();

        static void AddAssemblies(Assembly current, List<Assembly> assemblies)
        {
            if (assemblies.Contains(current))
            {
                return;
            }

            assemblies.Add(current);

            foreach (var assemblyName in current.GetReferencedAssemblies())
            {
                try
                {
                    var assembly = Assembly.Load(assemblyName);

                    Console.WriteLine("loaded assembly " + current.FullName);

                    AddAssemblies(assembly, assemblies);
                }
                catch
                {
                    Console.WriteLine("skipping assembly because it is not found: " + assemblyName.Name);
                }
            }
        }
    }
}
