using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace linqtoweb.CodeGenerator.AST
{
    public class ClassDecl : Expression
    {
        public string ClassName { get; private set; }

        public List<VariableDecl> ClassProperties { get; private set; }

        public ClassDecl(ExprPosition position, string classname, List<VariableDecl> properties)
            : base(position)
        {
            Debug.Assert(!String.IsNullOrEmpty(classname));

            this.ClassName = classname;
            this.ClassProperties = properties;
        }

        public override string ToString()
        {
            StringBuilder str = new StringBuilder();

            str.Append("class " + ClassName + "{");

            foreach (var v in ClassProperties)
                str.Append(v.ToString() + ";");

            str.Append("}");

            return str.ToString();
        }

        public ExpressionType   ContainsProperty(string propertyName)
        {
            foreach (var p in ClassProperties)
                if (p.VariableName == propertyName)
                    return p.VariableType;

            return null;
        }

        private void EmitCsInnerClass(EmitCodeContext codecontext)
        {
            // 
            List<string> propsInit = new List<string>();

            // properties
            codecontext.WriteLine("#region Properties");

            foreach (var x in ClassProperties)
            {
                if (x.VariableName.StartsWith("_"))
                    throw new GeneratorException(Position, "Class property cannot start with _. It is reserved system name.");

                // comment
                codecontext.WriteLine("// " + x.ToString());

                if (x.VariableType.IsExtractionObject)
                {
                    codecontext.WriteLine("public readonly " + x.VariableType.CsPropertyTypeName + " " + x.VariableName + ";");
                    propsInit.Add(x.VariableName + " = " + x.VariableType.CsPropertyInitValue + ";");
                }
                else
                {
                    string privatePropName = "_" + x.VariableName;

                    // extracting on request (public property)
                    // public property
                    string format = "public " + x.VariableType.CsPropertyTypeName + " " + x.VariableName +
                        "{get{while(" + privatePropName + "==" + x.VariableType.CsPropertyDefaultValue + "){if (!DoNextAction<object>(null))throw new NotExtractedDataException(\"" + x.VariableName + " cannot reach any data.\");} return " + privatePropName + ";}set{" + privatePropName + "=value;}}";

                    codecontext.WriteLine(format);

                    // private property value
                    string decl = "private " + x.VariableType.CsPropertyTypeName + " " + privatePropName;
                    codecontext.WriteLine(decl + " = " + x.VariableType.CsPropertyDefaultValue + ";");

                }

                codecontext.WriteLine("");

            }

            codecontext.WriteLine("#endregion");

            // ctor
            codecontext.WriteLine("#region Constructors");

            // no-param ctor
            codecontext.WriteLine("public " + ClassName + "():this(null){}");
            // ctor with parent given
            codecontext.WriteLine("public " + ClassName + "(ExtractionObjectBase parent):base(parent)");
            codecontext.WriteLine("{");
            codecontext.Level++;
            foreach (string strline in propsInit)
                codecontext.WriteLine(strline);
            codecontext.Level--;
            codecontext.WriteLine("}");
            //
            codecontext.WriteLine("#endregion");
        }

        internal override ExpressionType EmitCs(EmitCodeContext codecontext)
        {
            codecontext.WriteLine("public partial class " + ClassName + " : ExtractionObjectBase");
            codecontext.WriteLine("{");
            EmitCsInnerClass(codecontext.NewScope());
            codecontext.WriteLine("}");

            return ExpressionType.VoidType;
        }
    }
}
