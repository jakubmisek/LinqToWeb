using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace linqtoweb.CodeGenerator.AST
{
    /// <summary>
    /// The type of a variable.
    /// </summary>
    public class ExpressionType
    {
        #region predefined types

        public static ExpressionType StringType = new ExpressionType(KnownTypes.TString);
        public static ExpressionType IntType = new ExpressionType(KnownTypes.TInt);
        public static ExpressionType DoubleType = new ExpressionType(KnownTypes.TDouble);
        public static ExpressionType DateTimeType = new ExpressionType(KnownTypes.TDateTime);
        public static ExpressionType VoidType = new ExpressionType(KnownTypes.TVoid);

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

            if (obj == null)
            {
                return false;
            }
            else if ((objtype = obj as ExpressionType) != null)
            {
                if (this.TypeName != objtype.TypeName)
                    return false;

                if (this.TypeName == KnownTypes.TList)
                    return (this.ListOf == objtype.ListOf);
                else if (this.TypeName == KnownTypes.TUserType)
                    return (this.UserTypeName == objtype.UserTypeName);

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
            switch (TypeName)
            {
                case KnownTypes.TList:
                    return ListOf.ToString() + "[]";
                case KnownTypes.TUserType:
                    return UserTypeName;
                default:
                    return TypeName.ToString();
            }
        }

        #endregion

        /// <summary>
        /// Known types of language variables.
        /// </summary>
        public enum KnownTypes
        {
            TVoid, TString, TInt, TDouble, TDateTime, TList, TUserType
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

        public ExpressionType( KnownTypes name )
        {
            this.TypeName = name;
        }
        public ExpressionType( string usertype )
        {
            this.TypeName = KnownTypes.TUserType;
            this.UserTypeName = usertype;
        }
    }

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
    }
}
