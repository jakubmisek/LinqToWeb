using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace linqtoweb.CodeGenerator.AST
{
    class TypeCastExpression : UnaryExpr
    {
        public readonly ExpressionType NewType;
        
        public TypeCastExpression(ExprPosition position, ExpressionType newType, Expression expr)
            : base(position, expr)
        {
            this.NewType = newType;
        }

        internal override ExpressionType EmitCs(EmitCodeContext codecontext)
        {
            // process the value on separated output
            MemoryStream valstr = new MemoryStream();
            StreamWriter valoutput = new StreamWriter(valstr);
            
            EmitCodeContext valcontext = new EmitCodeContext(codecontext, valoutput);
            
            ExpressionType valType = Value.EmitCs(valcontext);

            valoutput.Flush();

            // get value emitted expression
            valstr.Position = 0;
            string valvalue = "(" + new StreamReader(valstr).ReadToEnd() + ")";
            

            // check return types
            if ( valType.Equals(NewType) )
            {
                codecontext.Write(valvalue);
            }
            else
            {
                string result = null;

                switch (valType.TypeName)
                {
                    case ExpressionType.KnownTypes.TBool:
                        switch (NewType.TypeName)
                        {
                            case ExpressionType.KnownTypes.TDouble:
                                result = string.Format("{0}?(1.0):(0.0)",valvalue);
                                break;
                            case ExpressionType.KnownTypes.TInt:
                                result = string.Format("{0}?(1):(0)", valvalue);
                                break;
                            case ExpressionType.KnownTypes.TString:
                                result = string.Format("{0}?(\"true\"):(\"false\")", valvalue);
                                break;
                        }
                        break;
                    case ExpressionType.KnownTypes.TDateTime:
                        switch (NewType.TypeName)
                        {
                            case ExpressionType.KnownTypes.TString:
                                result = string.Format("{0}.ToString()", valvalue);
                                break;
                        }
                        break;
                    case ExpressionType.KnownTypes.TDouble:
                        switch (NewType.TypeName)
                        {
                            case ExpressionType.KnownTypes.TBool:
                                result = string.Format("{0}!=0.0", valvalue);
                                break;
                            case ExpressionType.KnownTypes.TInt:
                                result = string.Format("(int){0}", valvalue);
                                break;
                            case ExpressionType.KnownTypes.TString:
                                result = string.Format("{0}.ToString()", valvalue);
                                break;
                        }
                        break;
                    case ExpressionType.KnownTypes.TInt:
                        switch (NewType.TypeName)
                        {
                            case ExpressionType.KnownTypes.TBool:
                                result = string.Format("{0}!=0.0", valvalue);
                                break;
                            case ExpressionType.KnownTypes.TDouble:
                                result = string.Format("(double){0}", valvalue);
                                break;
                            case ExpressionType.KnownTypes.TString:
                                result = string.Format("{0}.ToString()", valvalue);
                                break;
                        }
                        break;
                    case ExpressionType.KnownTypes.TString:
                        switch (NewType.TypeName)
                        {
                            case ExpressionType.KnownTypes.TBool:
                                result = string.Format("({0}.ToLower()==\"true\")?true:false", valvalue);
                                break;
                            case ExpressionType.KnownTypes.TInt:
                                result = string.Format("int.Parse{0}", valvalue);
                                break;
                            case ExpressionType.KnownTypes.TDouble:
                                result = string.Format("double.Parse{0}", valvalue);
                                break;
                            case ExpressionType.KnownTypes.TDateTime:
                                result = string.Format("DateTime.Parse{0}", valvalue);
                                break;
                        }
                        break;
                    default:
                        if (NewType.TypeName == ExpressionType.KnownTypes.TString)
                            result = string.Format("{0}.ToString()", valvalue); // anything to string
                        break;

                }

                if (string.IsNullOrEmpty(result))
                    throw new Exception("Unable to explicitly type the expression from " + valType.ToString() + " to " + NewType.ToString() + ".");

                codecontext.Write("(" + result + ")");
            }

            //
            return NewType;
        }
    }
}
