using System;
using System.Collections.Generic;

namespace GenerateAst
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length != 1)
            {
                Console.Error.WriteLine("Usage: generate_ast <output directory>");
                Environment.Exit(1);
            }
            String outputDir = args[0];
            DefineAst(outputDir, "Expr", new List<string> {
                "Binary   : Expr left, Token @operator, Expr right",
                "Grouping : Expr expression",
                "Literal  : object value",
                "Unary    : Token @operator, Expr right",
            });
        }

        private static void DefineAst(String outputDir, String baseName, List<String> types)
        {
            String path = outputDir + "/" + baseName + ".cs";
            using (var writer = new System.IO.StreamWriter(path))
            {
                writer.WriteLine("using System.Collections.Generic;");
                writer.WriteLine("");
                writer.WriteLine("namespace Lox");
                writer.WriteLine("{");

                writer.WriteLine("\tpublic abstract class " + baseName + " {");

                DefineVisitor(writer, baseName, types);

                // The AST classes.
                foreach (String type in types)
                {
                    String className = type.Split(':')[0].Trim();
                    String fields = type.Split(':')[1].Trim();
                    DefineType(writer, baseName, className, fields);
                }

                // The base accept() method.
                writer.WriteLine("");
                writer.WriteLine("\t\tabstract public R Accept<R>(IVisitor<R> visitor);");

                writer.WriteLine("\t}");
                writer.WriteLine("}");
            }
        }

        private static void DefineVisitor(System.IO.StreamWriter writer, String baseName, List<String> types)
        {
            writer.WriteLine("\t\tpublic interface IVisitor<R> {");

            foreach (String type in types)
            {
                String typeName = type.Split(':')[0].Trim();
                writer.WriteLine("\t\t\tR Visit" + typeName + baseName + "(" + typeName + " " + baseName.ToLower() + ");");
            }

            writer.WriteLine("\t\t}");
        }


        private static void DefineType(System.IO.StreamWriter writer, String baseName, String className, String fieldList)
        {
            writer.WriteLine("");
            writer.WriteLine("\t\tpublic class " + className + ": " + baseName + " {");

            // Constructor.
            writer.WriteLine("\t\t\tpublic " + className + "(" + fieldList + ") {");

            // Store parameters in fields.
            String[] fields = fieldList.Split(new[] { ", " }, StringSplitOptions.None);
            foreach (String field in fields)
            {
                String name = field.Split(' ')[1];
                writer.WriteLine("\t\t\t\tthis." + name + " = " + name + ";");
            }

            writer.WriteLine("\t\t\t}");

            // Visitor pattern.
            writer.WriteLine();
            writer.WriteLine("\t\t\tpublic override R Accept<R>(IVisitor<R> visitor) {");
            writer.WriteLine("\t\t\t\treturn visitor.Visit" + className + baseName + "(this);");
            writer.WriteLine("\t\t\t}");

            // Fields.
            writer.WriteLine();
            foreach (String field in fields)
            {
                writer.WriteLine("\t\t\tpublic " + field + ";");
            }

            writer.WriteLine("\t\t}");
        }


    }
}
