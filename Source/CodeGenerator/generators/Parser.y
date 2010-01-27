%namespace linqtoweb.CodeGenerator
%using linqtoweb.CodeGenerator.AST
%parsertype Parser
%tokentype Tokens

%token CLASS IDENTIFIER FOREACH 
%token LPAREN RPAREN LBRACE RBRACE LBRACKET RBRACKET
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
	public Expression node;
}

%start init

%%

init	:	classdecl
				{
					Ast = new GlobalCode();
				}
			
		;
		

classdecl	:	CLASS IDENTIFIER LBRACE RBRACE {  }
			;

%%

    public Parser(Scanner scanner) : base(scanner) { }
    
    public GlobalCode Ast {get;private set;}

