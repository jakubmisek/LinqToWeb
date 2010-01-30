%namespace linqtoweb.CodeGenerator
%using linqtoweb.CodeGenerator.AST
%parsertype Parser
%YYLTYPE ExprPosition
%tokentype Tokens

%token CLASS FOREACH 
%token IDENTIFIER DOTTEDIDENTIFIER
%token LPAREN RPAREN LBRACE RBRACE LBRACKET RBRACKET
%token TSTRING, TINT, TDOUBLE, TDATETIME, TBOOL
%token STRINGVAL INTEGERVAL DOUBLEVAL BOOLVAL
%token OP_ADD OP_SUB OP_MUL OP_DIV OP_ASSIGN
%token OP_ADD1, OP_SUB1
%token OP_LOGIC_OR OP_LOGIC_AND OP_LOGIC_XOR OP_LOGIC_NOT
%token OP_EQ OP_NOTEQ OP_LESS OP_GREATER OP_LESSEQ OP_GREATEREQ
%token OP_QUESTION OP_COLON
%token COMMA SEMICOLON
%token WHITESPACE
%token COMMENT

%left OP_ADD
%left OP_SUB
%left OP_MUL
%left OP_DIV
%left OP_ADD1
%left OP_SUB1
%left OP_ASSIGN
%right OP_QUESTION 
%left OP_COLON
%left OP_EQ	
%left OP_NOTEQ
%left OP_LESS
%left OP_GREATER
%left OP_LESSEQ
%left OP_GREATEREQ
%left OP_LOGIC_OR 
%left OP_LOGIC_AND
%left OP_LOGIC_XOR
%right OP_LOGIC_NOT
%left LPAREN
%right RPAREN

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
singlebasetype:	TSTRING		{ $$.obj = ExpressionType.StringType; }
			|	TINT		{ $$.obj = ExpressionType.IntType; }
			|	TDOUBLE		{ $$.obj = ExpressionType.DoubleType; }
			|	TDATETIME	{ $$.obj = ExpressionType.DateTimeType; }
			|	TBOOL		{ $$.obj = ExpressionType.BoolType; }
			;
singletype:		singlebasetype {$$.obj = $1.obj;}	
			|	IDENTIFIER	{ $$.obj = new ExpressionType((string)$1.obj); }
			;
						
methoddecl:		IDENTIFIER LPAREN argslist RPAREN contextstatement { $$.obj = new MethodDecl( @1.Merge(@4), (string)$1.obj, (List<VariableDecl>)$3.obj, (Expression)$5.obj ); }
			;
argslist:		typename IDENTIFIER COMMA argslist { $$.obj = VariableDecls($4.obj, new VariableDecl(@1.Merge(@2),(ExpressionType)$1.obj,(string)$2.obj)); }
			|	typename IDENTIFIER { $$.obj = VariableDecls(null, new VariableDecl(@1.Merge(@2),(ExpressionType)$1.obj,(string)$2.obj)); }
			|	{ $$.obj = null; }
			;

contextdef:		LBRACKET methodcall RBRACKET	{ $$.obj = $2.obj; }
			;

contextstatement:	contextdef contextstatement { $$.obj = $2.obj; AddDataContext(ref $$.obj,(MethodCall)$1.obj); }
			|		statement { $$.obj = $1.obj; }
			;
statementlist:	contextstatement statementlist { $$.obj = ExpressionList($2.obj, (Expression)$1.obj); }
			|	contextstatement { $$.obj = ExpressionList(null, (Expression)$1.obj); }
			;
statement:		SEMICOLON	{ $$.obj = null; /*empty statement*/ }
			|	singletype IDENTIFIER OP_ASSIGN expr SEMICOLON { $$.obj = new VariableDecl(@1.Merge(@5),(ExpressionType)$1.obj,(string)$2.obj,(Expression)$4.obj);/* declare and initialize variable */ }
			|	expr SEMICOLON { $$.obj = $1.obj; }
			|	LBRACE RBRACE	{ $$.obj = null; }
			|	LBRACE statementlist RBRACE { $$.obj = new CodeBlock( @1.Merge(@3), (List<Expression>)$2.obj ); }
			|	FOREACH LPAREN expr RPAREN contextstatement { $$.obj = new Foreach(@1.Merge(@5),(Expression)$3.obj,(Expression)$5.obj); }
			;
expr:			IDENTIFIER	{ $$.obj = new VariableUse( @1, (string)$1.obj ); }
			|	DOTTEDIDENTIFIER	{ $$.obj = new VariableUse( @1, (string)$1.obj ); }
			|	STRINGVAL	{ $$.obj = new StringLiteral(@1, (string)$1.obj); }
			|	INTEGERVAL	{ $$.obj = new IntLiteral(@1, (int)$1.obj); }
			|	DOUBLEVAL	{ $$.obj = new DoubleLiteral(@1, (double)$1.obj); }
			|	BOOLVAL		{ $$.obj = new BoolLiteral(@1, (bool)$1.obj); }
			|	IDENTIFIER LBRACKET RBRACKET OP_ASSIGN expr	{ $$.obj = new AddElementExpression(@1.Merge(@5),(VariableUse)$1.obj, (Expression)$5.obj); }
			|	OP_SUB expr	{  $$.obj = new UnaryMinusExpression(@1.Merge(@2),(Expression)$2.obj); }
			|	OP_ADD expr	{  $$.obj = new UnaryPlusExpression(@1.Merge(@2),(Expression)$2.obj); }
			|	OP_SUB1 expr	{  $$.obj = new BinaryAddExpression(@1.Merge(@2),(Expression)$2.obj, new IntLiteral(@1,-1)); }
			|	OP_ADD1 expr	{  $$.obj = new BinaryAddExpression(@1.Merge(@2),(Expression)$2.obj, new IntLiteral(@1,+1)); }
			|	expr OP_SUB1	{  $$.obj = new BinaryAddExpression(@1.Merge(@2),(Expression)$1.obj, new IntLiteral(@1,-1)); }
			|	expr OP_ADD1	{  $$.obj = new BinaryAddExpression(@1.Merge(@2),(Expression)$1.obj, new IntLiteral(@1,+1)); }
			|	expr OP_ADD expr	{ $$.obj = new BinaryAddExpression(@1.Merge(@3), (Expression)$1.obj, (Expression)$3.obj); }
			|	expr OP_SUB expr	{ $$.obj = new BinarySubExpression(@1.Merge(@3), (Expression)$1.obj, (Expression)$3.obj); }
			|	expr OP_MUL expr	{ $$.obj = new BinaryMulExpression(@1.Merge(@3), (Expression)$1.obj, (Expression)$3.obj); }
			|	expr OP_DIV expr	{ $$.obj = new BinaryDivExpression(@1.Merge(@3), (Expression)$1.obj, (Expression)$3.obj); }
			|	expr OP_ASSIGN expr	{ $$.obj = new BinaryAssignExpression(@1.Merge(@3), (Expression)$1.obj, (Expression)$3.obj); }
			|	expr OP_QUESTION expr OP_COLON expr	{$$.obj = new TernaryCondExpression(@1.Merge(@5),(Expression)$1.obj,(Expression)$3.obj,(Expression)$5.obj);}
			|	expr OP_EQ expr				{$$.obj = new EqExpression(@1.Merge(@3),(Expression)$1.obj, (Expression)$3.obj);}
			|	expr OP_NOTEQ expr			{$$.obj = new NotEqExpression(@1.Merge(@3),(Expression)$1.obj, (Expression)$3.obj);}
			|	expr OP_LESS expr			{$$.obj = new BinaryLessExpression(@1.Merge(@3),(Expression)$1.obj, (Expression)$3.obj);}
			|	expr OP_GREATER expr		{$$.obj = new BinaryGreaterExpression(@1.Merge(@3),(Expression)$1.obj, (Expression)$3.obj);}
			|	expr OP_LESSEQ expr			{$$.obj = new BinaryLessEqExpression(@1.Merge(@3),(Expression)$1.obj, (Expression)$3.obj);}
			|	expr OP_GREATEREQ expr		{$$.obj = new BinaryGreaterEqExpression(@1.Merge(@3),(Expression)$1.obj, (Expression)$3.obj);}
			|	expr OP_LOGIC_OR expr		{$$.obj = new LogicalOrExpression(@1.Merge(@3),(Expression)$1.obj, (Expression)$3.obj);}
			|	expr OP_LOGIC_AND expr		{$$.obj = new LogicalAndExpression(@1.Merge(@3),(Expression)$1.obj, (Expression)$3.obj);}
			|	expr OP_LOGIC_XOR expr		{$$.obj = new LogicalXorExpression(@1.Merge(@3),(Expression)$1.obj, (Expression)$3.obj);}
			|	OP_LOGIC_NOT expr			{$$.obj = new LogicalNotExpression(@1.Merge(@2),(Expression)$2.obj); }
			|	LPAREN expr RPAREN	{ $$.obj = $2.obj; }
			|	LPAREN singlebasetype RPAREN expr	{ $$.obj = new TypeCastExpression( @1.Merge(@4), (ExpressionType)$2.obj, (Expression)$4.obj ); }
			|	methodcall	{ $$.obj = $1.obj; }
			;
methodcall:		IDENTIFIER LPAREN callargs RPAREN	{ $$.obj = new MethodCall( @1.Merge(@4), (string)$1.obj, (List<Expression>)$3.obj ); }
			;

callargs:		nextcallargs { $$.obj = ExpressionList($1.obj,null); }
			|	{ $$.obj = ExpressionList(null,null); }
			;
nextcallargs:	expr	{ $$.obj = ExpressionList(null,(Expression)$1.obj); }
			|	expr COMMA nextcallargs	{ $$.obj = ExpressionList($3.obj,(Expression)$1.obj); }
			;
%%

	/* creates new list of variables declaration from old List<VariableDecl> and new VariableDecl */
	private List<VariableDecl> VariableDecls( object decls, VariableDecl vardecl )
	{
		var newdecls = (decls!=null)?(new List<VariableDecl>( (List<VariableDecl>)decls )):(new List<VariableDecl>());
		if(vardecl!=null)newdecls.Add(vardecl);		
		return newdecls;
	}
	
	/* creates new list of expressions from old List<Expression> and new Expression */
	private List<Expression> ExpressionList( object exprs, Expression expr )
	{
		var newexprs = (exprs!=null)?(new List<Expression>( (List<Expression>)exprs )):(new List<Expression>());
		
		if(expr!=null)
		{
			CodeBlock exprbl;
			if ( (exprbl = expr as CodeBlock) != null )
			{	// reduces the tree (empty CodeBlock or CodeBlock with only one expression is reduced)
				if (exprbl.Statements.Count > 0)
				{
					newexprs.Add( (exprbl.Statements.Count==1 && exprbl.DataContexts.Count == 0)?exprbl.Statements[0]:exprbl );
				}
			}
			else
				newexprs.Add( expr );
		}
		return newexprs;
	}
	
	private void AddDataContext( ref object statement, MethodCall contextcreate )
	{
		if (statement == null || contextcreate == null)	return;
		Expression expr = statement as Expression;
		CodeBlock exprbl = statement as CodeBlock;
		
		if ( expr == null )	return;
		if ( exprbl == null )	statement = exprbl = new CodeBlock( expr.Position, new List<Expression>(){ expr } );
		
		exprbl.DataContexts.AddFirst(contextcreate);
	}

	/* initialization of the parser object */
    public Parser(Scanner scanner)
     :base(scanner)
    {
    
    }
    
    /* The result of the Parse() operation. */
    public GlobalCode Ast { get; private set; }

