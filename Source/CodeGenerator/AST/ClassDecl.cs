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

        private string PropertyType( ExpressionType decl )
        {
            if (decl.ListOf != null)
            {
                return "ExtractionList<" + decl.ListOf.CsName + ">";
            }
            else
            {
                switch ( decl.TypeName )
                {
                    case ExpressionType.KnownTypes.TBool:
                        return "bool?";
                    case ExpressionType.KnownTypes.TDouble:
                        return "double?";
                    case ExpressionType.KnownTypes.TInt:
                        return "int?";
                    default:
                        return decl.CsName;
                        //throw new InvalidOperationException();
                }
            } 
        }

        private void EmitCsInnerClass(EmitCodeContext codecontext)
        {
            foreach (var x in ClassProperties)
            {
                string protype= PropertyType(x.VariableType);
                // private property value
                
                if (x.VariableType.IsExtractionObject)
                {
                    string initvalue = "new " + protype + "()";

                    string decl = "public readonly " + PropertyType(x.VariableType) + " " + x.VariableName;
                    codecontext.WriteLine(decl + " = " + initvalue + ";");
                }
                else
                {
                    string privatePropName = "_" + x.VariableName;

                    string defaultvalue;
                    switch (x.VariableType.TypeName)
                    {
                        case ExpressionType.KnownTypes.TDateTime:
                            defaultvalue = "DateTime.MinValue";
                            break;
                        default:
                            defaultvalue = "null";
                            break;
                    }
                    string decl = "private " + protype + " " + privatePropName;
                    codecontext.WriteLine(decl + " = " + defaultvalue + ";");

                    // extracting on request (public property)
                    string format = "public " + protype + " " + x.VariableName +
                        "{set{while(" + privatePropName + "==" + defaultvalue + "){if (!DoNextAction(null))throw new NotExtractedDataException(\"SampleProperty cannot reach any data.\");}" + privatePropName + "=value;}get{return " + privatePropName + ";}}";

                    codecontext.WriteLine(format);
                }

                codecontext.WriteLine("");
                
            }
        }
        internal override ExpressionType EmitCs(EmitCodeContext codecontext)
        {
            codecontext.WriteLine("public class " + ClassName + " : ExtractionObjectBase");
            codecontext.WriteLine("{");
            EmitCsInnerClass(codecontext.NewScope());
            codecontext.WriteLine("}");

            return ExpressionType.VoidType;
        }
    }
}
