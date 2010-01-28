%namespace linqtoweb.CodeGenerator
%using linqtoweb.CodeGenerator.AST
%parsertype Parser
%YYLTYPE ExprPosition
%tokentype Tokens

%token CLASS IDENTIFIER DOTTEDIDENTIFIER FOREACH 
%token LPAREN RPAREN LBRACE RBRACE LBRACKET RBRACKET
%token TSTRING, TINT, TDOUBLE, TDATETIME
%token STRINGVAL INTEGERVAL DOUBLEVAL
%token OP_PLUS OP_MINUS OP_MUL OP_DIV OP_ASSIGN
%token COMMA SEMICOLON
%token WHITESPACE
%token COMMENT

%left OP_PLUS
%left OP_MINUS
%left OP_MUL
%left OP_DIV
%left OP_ASSIGN

%union
{
	public Object obj;
}

%start init

%%

init:			globaldecls	{ Ast = new GlobalCode((DeclarationsList)$1.obj); }
			;
			
globaldecls:	classdecl globaldecls { $$.obj = new DeclarationsList( @1.Merge(@2), (DeclarationsList)$2.obj, (ClassDecl)$1.obj ); }
			|	methoddecl globaldecls { $$.obj = new DeclarationsList( @1.Merge(@2), (DeclarationsList)$2.obj, (MethodDecl)$1.obj ); }
			|	EOF { $$.obj = new DeclarationsList( new ExprPosition() ); }
			;

classdecl:		CLASS IDENTIFIER LBRACE propertylist RBRACE	{ $$.obj = new ClassDecl( @1.Merge(@5), (string)$2.obj, (List<VariableDecl>)$4.obj ); }
			;
propertylist:	typename IDENTIFIER SEMICOLON propertylist { $$.obj = VariableDecls($4.obj, new VariableDecl(@1.Merge(@2),(ExpressionType)$1.obj,(string)$2.obj)); }
			|	{ $$.obj = null; }
			;
typename:		singletype { $$.obj = $1.obj; }
			|	singletype LBRACKET RBRACKET { $$.obj = new ExpressionListType( (ExpressionType)$1.obj ); }
			;
singletype:		TSTRING		{ $$.obj = ExpressionType.StringType; }
			|	TINT		{ $$.obj = ExpressionType.IntType; }
			|	TDOUBLE		{ $$.obj = ExpressionType.DoubleType; }
			|	TDATETIME	{ $$.obj = ExpressionType.DateTimeType; }
			|	IDENTIFIER	{ $$.obj = new ExpressionType((string)$1.obj); }
			;
						
methoddecl:		IDENTIFIER LPAREN argslist RPAREN { $$.obj = new MethodDecl( @1.Merge(@4), (string)$1.obj, (List<VariableDecl>)$3.obj ); }
			;
argslist:		typename IDENTIFIER COMMA argslist { $$.obj = VariableDecls($4.obj, new VariableDecl(@1.Merge(@2),(ExpressionType)$1.obj,(string)$2.obj)); }
			|	typename IDENTIFIER { $$.obj = VariableDecls(null, new VariableDecl(@1.Merge(@2),(ExpressionType)$1.obj,(string)$2.obj)); }
			|	{ $$.obj = null; }
			;

%%

	/* creates new list of variables declaration from old List<VariableDecl> and new VariableDecl */
	private List<VariableDecl> VariableDecls( object decls, VariableDecl vardecl )
	{
		var newdecls = (decls!=null)?(new List<VariableDecl>( (List<VariableDecl>)decls )):(new List<VariableDecl>());
		if(vardecl!=null)newdecls.Add(vardecl);		
		return newdecls;
	}

	/* initialization of the parser object */
    public Parser(Scanner scanner)
     :base(scanner)
    {
    
    }
    
    /* The result of the Parse() operation. */
    public GlobalCode Ast { get; private set; }

