using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace linqtoweb.CodeGenerator.AST
{
    #region abstract ExpressionType

    /// <summary>
    /// The type of a variable.
    /// </summary>
    public abstract class ExpressionType
    {
        #region predefined types

        public static ExpressionType StringType = new ExpressionStringType();
        public static ExpressionType IntType = new ExpressionIntType();
        public static ExpressionType DoubleType = new ExpressionDoubleType();
        public static ExpressionType DateTimeType = new ExpressionDateTimeType();
        public static ExpressionType VoidType = new ExpressionVoidType();
        public static ExpressionType BoolType = new ExpressionBoolType();

        #endregion

        #region Object

        /// <summary>
        /// Tests if two instances of ExpressionType are describing the same type.
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object obj)
        {
            ExpressionType objtype;

            if (obj != null && (objtype = obj as ExpressionType) != null)
            {
                if (this.TypeName != objtype.TypeName)
                    return false;

                if (this.TypeName == KnownTypes.TList)
                    return (this.ListOf.Equals(objtype.ListOf));
                else if (this.TypeName == KnownTypes.TUserType)
                    return (this.UserTypeName == objtype.UserTypeName);
                else
                    return true;
            }
            else
            {
                return base.Equals(obj);
            }
        }
        
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public override string ToString()
        {
            return CsArgumentTypeName;
        }

        #endregion

        /// <summary>
        /// Type name when used in class property declaration.
        /// </summary>
        public virtual string CsPropertyTypeName { get { throw new NotImplementedException(); } }

        /// <summary>
        /// Property declaration initial value.
        /// </summary>
        public virtual string CsPropertyInitValue { get { throw new NotImplementedException(); } }

        /// <summary>
        /// Property declaration initial value.
        /// </summary>
        public virtual string CsPropertyRootInitValue { get { return CsPropertyInitValue; } }

        /// <summary>
        /// Property default value when uninitialized.
        /// </summary>
        public virtual string CsPropertyDefaultValue { get { throw new NotImplementedException(); } }
        
        /// <summary>
        /// Type name when it's passed in method argument.
        /// </summary>
        public virtual string CsArgumentTypeName { get { throw new NotImplementedException(); } }

        /*/// <summary>
        /// Get the C# equivalent type name.
        /// </summary>
        public string CsTypeName
        {
            get
            {
                switch (TypeName)
                {
                    case KnownTypes.TBool:
                        return "bool";
                    case KnownTypes.TDateTime:
                        return "DateTime";
                    case KnownTypes.TDouble:
                        return "double";
                    case KnownTypes.TInt:
                        return "int";
                    case KnownTypes.TList:
                        return "ExtractionListBase<" + ListOf.CsTypeName + ">";
                    case KnownTypes.TString:
                        return "string";
                    case KnownTypes.TUserType:
                        return UserTypeName;
                    case KnownTypes.TVoid:
                        return "void";
                    default:
                        throw new NotImplementedException("this type is not implemented");
                }
            }
        }*/

        /// <summary>
        /// True if the type represents user defined object, that is able to be filled by extraction method.
        /// </summary>
        public bool IsExtractionObject
        {
            get
            {
                return (TypeName == KnownTypes.TList || TypeName == KnownTypes.TUserType);
            }
        }

        /// <summary>
        /// Known types of language variables.
        /// </summary>
        public enum KnownTypes
        {
            TVoid, TBool, TString, TInt, TDouble, TDateTime, TList, TUserType
        }

        /// <summary>
        /// Expression type name.
        /// </summary>
        public KnownTypes TypeName { get; private set; }

        /// <summary>
        /// In case of TList type, this contains the type of list elements.
        /// </summary>
        public ExpressionType ListOf { get; protected set; }

        /// <summary>
        /// In case of TUserType type, this contains the name of user defined class.
        /// </summary>
        public string UserTypeName { get; private set; }

        protected ExpressionType(KnownTypes name)
        {
            this.TypeName = name;
        }
        protected ExpressionType( string usertype )
        {
            this.TypeName = KnownTypes.TUserType;
            this.UserTypeName = usertype;
        }
    }

    #endregion

    #region defined expression types

    /// <summary>
    /// The list type of a variable.
    /// </summary>
    public class ExpressionListType : ExpressionType
    {
        public ExpressionListType(ExpressionType listelement)
            :base( KnownTypes.TList )
        {
            this.ListOf = listelement;
        }

        public override string CsArgumentTypeName
        {
            get
            {
                return "ExtractionListBase<" + ListOf.CsArgumentTypeName + ">";
            }
        }
        public override string CsPropertyTypeName
        {
            get
            {
                return "ExtractionList<" + ListOf.CsArgumentTypeName + ">";
            }
        }
        public override string CsPropertyInitValue
        {
            get
            {
                return "new " + CsPropertyTypeName + "(this as ExtractionObjectBase)";
            }
        }
        public override string CsPropertyRootInitValue
        {
            get
            {
                return "new " + CsPropertyTypeName + "()";
            }
        }
    }

    public class ExpressionBoolType : ExpressionType
    {
        public ExpressionBoolType():base(KnownTypes.TBool){}

        public override string CsArgumentTypeName
        {
            get
            {
                return "bool";
            }
        }
        public override string CsPropertyTypeName
        {
            get
            {
                return "bool?";
            }
        }
        public override string CsPropertyInitValue
        {
            get
            {
                return "null";
            }
        }
        public override string CsPropertyDefaultValue
        {
            get
            {
                return CsPropertyInitValue;
            }
        }
    }
    public class ExpressionIntType : ExpressionType
    {
        public ExpressionIntType() : base(KnownTypes.TInt) { }

        public override string CsArgumentTypeName
        {
            get
            {
                return "int";
            }
        }
        public override string CsPropertyTypeName
        {
            get
            {
                return "int?";
            }
        }
        public override string CsPropertyInitValue
        {
            get
            {
                return "null";
            }
        }
        public override string CsPropertyDefaultValue
        {
            get
            {
                return CsPropertyInitValue;
            }
        }
    }
    public class ExpressionDoubleType : ExpressionType
    {
        public ExpressionDoubleType() : base(KnownTypes.TDouble) { }

        public override string CsArgumentTypeName
        {
            get
            {
                return "double";
            }
        }
        public override string CsPropertyTypeName
        {
            get
            {
                return "double?";
            }
        }
        public override string CsPropertyInitValue
        {
            get
            {
                return "null";
            }
        }
        public override string CsPropertyDefaultValue
        {
            get
            {
                return CsPropertyInitValue;
            }
        }
    }
    public class ExpressionStringType : ExpressionType
    {
        public ExpressionStringType() : base(KnownTypes.TString) { }

        public override string CsArgumentTypeName
        {
            get
            {
                return "string";
            }
        }
        public override string CsPropertyTypeName
        {
            get
            {
                return CsArgumentTypeName;
            }
        }
        public override string CsPropertyInitValue
        {
            get
            {
                return "null";
            }
        }
        public override string CsPropertyDefaultValue
        {
            get
            {
                return CsPropertyInitValue;
            }
        }
    }
    public class ExpressionDateTimeType : ExpressionType
    {
        public ExpressionDateTimeType() : base(KnownTypes.TDateTime) { }

        public override string CsArgumentTypeName
        {
            get
            {
                return "DateTime";
            }
        }
        public override string CsPropertyTypeName
        {
            get
            {
                return CsArgumentTypeName;
            }
        }
        public override string CsPropertyInitValue
        {
            get
            {
                return "DateTime.MinValue";
            }
        }
        public override string CsPropertyDefaultValue
        {
            get
            {
                return CsPropertyInitValue;
            }
        }
    }
    public class ExpressionUserType : ExpressionType
    {
        public ExpressionUserType(string usertype) : base(usertype) { }

        public override string CsArgumentTypeName
        {
            get
            {
                return UserTypeName;
            }
        }
        public override string CsPropertyTypeName
        {
            get
            {
                return CsArgumentTypeName;
            }
        }
        public override string CsPropertyInitValue
        {
            get
            {
                return "new " + CsPropertyTypeName + "(this as ExtractionObjectBase)";
            }
        }
        public override string CsPropertyRootInitValue
        {
            get
            {
                return "new " + CsPropertyTypeName + "()";
            }
        }
    }
    public class ExpressionVoidType : ExpressionType
    {
        public ExpressionVoidType():base(KnownTypes.TVoid){}

        public override string CsArgumentTypeName
        {
            get
            {
                return "void";
            }
        }
    }

    #endregion
}
